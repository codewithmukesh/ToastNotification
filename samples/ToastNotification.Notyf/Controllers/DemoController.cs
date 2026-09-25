using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ToastNotification.Notyf.Controllers;

/// <summary>
/// Classic MVC form posts from the docs page. Each action queues a toast with <see cref="INotyfService"/>.
/// </summary>
[AutoValidateAntiforgeryToken]
public class DemoController(INotyfService notyf) : Controller
{
    // Post-Redirect-Get: the toast waits in TempData for the next page.
    [HttpPost]
    public IActionResult Save()
    {
        notyf.Success("Saved! This survived a redirect.");
        return Redirect(Url.Action("Index", "Home") + "#redirects");
    }

    // Re-render the page without redirecting (e.g. validation errors).
    [HttpPost]
    public IActionResult Validate()
    {
        notyf.Error("Please fix the highlighted fields.");
        return View("~/Views/Home/Index.cshtml");
    }
}
