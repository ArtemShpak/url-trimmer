using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Dto;
using UrlShortener.Application.Interfaces.Services;
using UrlShortener.Core.Entity.Error;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortUrlsController(IShortUrlService shortUrlService) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateShortUrl([FromBody] CreateShortUrlRequestDto request)
    {
        var result = await shortUrlService.CreateShortUrlAsync(request.OriginalUrl);

        if (result.IsFailure)
        {
            return result.Error!.Code switch
            {
                var code when code == DomainErrors.ShortUrl.AlreadyExists.Code => BadRequest(new { code, error = result.Error.Message }),
                var code when code == DomainErrors.Auth.Unauthorized.Code => Unauthorized(new { code, error = result.Error.Message }),
                var code when code == DomainErrors.ShortUrl.InvalidFormat.Code => BadRequest(new { code, error = result.Error.Message }),
                _ => BadRequest(new { code = result.Error.Code, error = result.Error.Message })
            };
        }

        return Ok(result.Value);
    }

    [HttpGet("{shortCode}")]
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
                var code when code == DomainErrors.ShortUrl.NotFound.Code => NotFound(new { code, error = result.Error.Message }),
                var code when code == DomainErrors.Auth.Forbidden.Code => Forbid(),
                var code when code == DomainErrors.Auth.Unauthorized.Code => Unauthorized(new { code, error = result.Error.Message }),
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
                var code when code == DomainErrors.ShortUrl.NotFound.Code => NotFound(new { code, error = result.Error.Message }),
                var code when code == DomainErrors.Auth.Unauthorized.Code => Unauthorized(new { code, error = result.Error.Message }),
                _ => BadRequest(new { code = result.Error.Code, error = result.Error.Message })
            };
        }

        return Ok(result.Value);
    }
}