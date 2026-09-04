using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Interfaces.Services;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortUrlsController(IShortUrlService shortUrlService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateShortUrl([FromBody] string originalUrl)
    {
        var result = await shortUrlService.CreateShortUrlAsync(originalUrl);

        if (result.IsFailure)
        {
            return StatusCode(result.Error!.Code switch
            {
                "Url.AlreadyExists" => StatusCodes.Status400BadRequest,
                "Auth.Unauthorized" => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status400BadRequest
            }, new { code = result.Error.Code, error = result.Error.Message });
        }

        return Ok(result.Value);
    }

    [HttpGet("/{shortCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> RedirectToOriginal(string shortCode)
    {
        var originalUrl = await shortUrlService.GetOriginalUrlAndRecordClickAsync(shortCode);
        return originalUrl is null ? NotFound() : Redirect(originalUrl);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllShortUrls()
    {
        var urls = await shortUrlService.GetAllUrlsAsync();
        return Ok(urls);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteShortUrl(int id)
    {
        var result = await shortUrlService.DeleteShortUrlAsync(id);

        if (result.IsFailure)
        {
            return result.Error!.Code switch
            {
                "Url.NotFound" => NotFound(new { code = result.Error.Code, error = result.Error.Message }),
                "Auth.Forbidden" => Forbid(),
                "Auth.Unauthorized" => Unauthorized(new { code = result.Error.Code, error = result.Error.Message }),
                _ => BadRequest(new { code = result.Error.Code, error = result.Error.Message })
            };
        }

        return NoContent();
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetShortUrlDetails(int id)
    {
        var result = await shortUrlService.GetShortUrlDetailsAsync(id);

        if (result.IsFailure)
        {
            return result.Error!.Code switch
            {
                "Url.NotFound" => NotFound(new { code = result.Error.Code, error = result.Error.Message }),
                "Auth.Unauthorized" => Unauthorized(new { code = result.Error.Code, error = result.Error.Message }),
                _ => BadRequest(new { code = result.Error.Code, error = result.Error.Message })
            };
        }

        return Ok(result.Value);
    }
}