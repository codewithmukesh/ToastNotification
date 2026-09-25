using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Encodings.Web;

namespace ToastNotification.Docs.Pages;

/// <summary>
/// A real Razor Page using Toastify - the Razor Pages side of this docs app.
/// </summary>
public class CheckoutModel(IToastifyService toastify) : PageModel
{
    [BindProperty]
    public string? Name { get; set; }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            toastify.Error("Please enter your name.");
            return Page(); // no redirect needed
        }

        toastify.Success($"Order placed for {HtmlEncoder.Default.Encode(Name)}!");
        return RedirectToPage(); // survives the redirect via TempData
    }
}
