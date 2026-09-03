using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Interfaces;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Infrastructure.Authentication.Entity;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortUrlsController : ControllerBase
{
    private readonly IShortUrlService _shortUrlService;
    private readonly UserManager<Person> _userManager;

    public ShortUrlsController(IShortUrlService shortUrlService, UserManager<Person> userManager)
    {
        _shortUrlService = shortUrlService;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateShortUrl([FromBody] string originalUrl)
    {
        var userIdStr = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
        {
            return Unauthorized();
        }

        var (isSuccess, errorMessage, result) = await _shortUrlService.CreateShortUrlAsync(originalUrl, userId);

        if (!isSuccess)
        {
            return BadRequest(errorMessage);
        }

        return Ok(result);
    }

    [HttpGet("/{shortCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> RedirectToOriginal(string shortCode)
    {
        var originalUrl = await _shortUrlService.GetOriginalUrlAndRecordClickAsync(shortCode);

        if (originalUrl == null)
        {
            return NotFound();
        }

        return Redirect(originalUrl);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllShortUrls()
    {
        var urls = await _shortUrlService.GetAllUrlsAsync();
        return Ok(urls);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteShortUrl(int id)
    {
        var result = await _shortUrlService.DeleteShortUrlAsync(id);

        if (result.IsFailure)
        {
            return result.Error switch
            {
                "Url not found" => NotFound(new { error = result.Error }),
                "Forbidden" => Forbid(),
                _ => BadRequest(new { error = result.Error })
            };
        }

        return NoContent();
    }
}
