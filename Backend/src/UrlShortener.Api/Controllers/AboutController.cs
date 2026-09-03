using UrlShortener.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Infrastructure.Authentication.Entity;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Controllers;

public class AboutController : Controller
{
    private readonly IAboutService _aboutService;
    private readonly UserManager<Person> _userManager;

    public AboutController(IAboutService aboutService, UserManager<Person> userManager)
    {
        _aboutService = aboutService;
        _userManager = userManager;
    }

    [HttpGet("About")]
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var content = await _aboutService.GetContentAsync();
        var model = new AboutViewModel
        {
            Content = content,
            IsAdmin = User.IsInRole("Admin")
        };

        return View(model);
    }

    [HttpPost("About")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index(string content)
    {
        var userIdStr = _userManager.GetUserId(User);
        if (int.TryParse(userIdStr, out var userId))
        {
            await _aboutService.UpdateContentAsync(content, userId);
        }

        return RedirectToAction(nameof(Index));
    }
}
