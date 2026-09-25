using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ToastNotification.Toastify.Pages;

/// <summary>
/// The docs page. The form demos post to the handlers below; everything else goes through <c>DemoEndpoints</c>.
/// </summary>
public class IndexModel(IToastifyService toastify) : PageModel
{
    public void OnGet()
    {
        // Greet first-time visitors - a toast straight from the page model.
        if (!Request.Cookies.ContainsKey("toast-docs-welcomed"))
        {
            toastify.Information("Welcome! Every button on this page raises a toast from C#.", 8);
            Response.Cookies.Append("toast-docs-welcomed", "1", new CookieOptions { MaxAge = TimeSpan.FromDays(30), HttpOnly = true });
        }
    }

    // Post-Redirect-Get: the toast waits in TempData for the next page.
    public IActionResult OnPostSave()
    {
        toastify.Success("Saved! This survived a redirect.");
        return Redirect(Url.Page("/Index") + "#redirects");
    }

    // Re-render the page without redirecting (e.g. validation errors).
    public IActionResult OnPostValidate()
    {
        toastify.Error("Please fix the highlighted fields.");
        return Page();
    }
}
