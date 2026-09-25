using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToastNotification.Notyf.Models;

namespace ToastNotification.Notyf.Controllers;

public class HomeController(INotyfService notyf) : Controller
{
    public IActionResult Index()
    {
        // Greet first-time visitors - a toast straight from the controller.
        if (!Request.Cookies.ContainsKey("toast-docs-welcomed"))
        {
            notyf.Information("Welcome! Every button on this page raises a toast from C#.", 8);
            Response.Cookies.Append("toast-docs-welcomed", "1", new CookieOptions { MaxAge = TimeSpan.FromDays(30), HttpOnly = true });
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
