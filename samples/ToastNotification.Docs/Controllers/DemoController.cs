using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace ToastNotification.Docs.Controllers;

/// <summary>
/// Classic MVC form posts from the docs page. Each action queues a toast with the selected library.
/// </summary>
[AutoValidateAntiforgeryToken]
public class DemoController(INotyfService notyf, IToastifyService toastify) : Controller
{
    // Post-Redirect-Get: the toast waits in TempData for the next page.
    [HttpPost]
    public IActionResult Save(string? library)
    {
        const string message = "Saved! This survived a redirect.";
        if (library == "toastify") toastify.Success(message); else notyf.Success(message);
        return Redirect(Url.Action("Index", "Home") + "#redirects");
    }

    // Re-render the page without redirecting (e.g. validation errors).
    [HttpPost]
    public IActionResult Validate(string? library)
    {
        const string message = "Please fix the highlighted fields.";
        if (library == "toastify") toastify.Error(message); else notyf.Error(message);
        return View("~/Views/Home/Index.cshtml");
    }
}
