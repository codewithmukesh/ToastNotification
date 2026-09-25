using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreHero.ToastNotification.TestApp.Controllers;

public class HomeController(INotyfService notyf, IToastifyService toastify) : Controller
{
    // Tries to break out of the JSON data block. Messages are rendered as HTML by design, but must never escape the script context.
    public const string XssMessage = "</script><script>window.__xss = true;</script>'\"";

    public IActionResult Index() => View();

    public IActionResult NoJquery()
    {
        ViewData["NoJquery"] = true;
        notyf.Success("Works without jQuery");
        return View(nameof(Index));
    }

    public IActionResult Nonce()
    {
        ViewData["Nonce"] = "test-nonce-123";
        return View(nameof(Index));
    }

    // ----- Notyf -----

    public IActionResult NotyfAll()
    {
        notyf.Success("Notyf success");
        notyf.Error("Notyf error");
        notyf.Warning("Notyf warning");
        notyf.Information("Notyf information", 5);
        notyf.Custom("Notyf custom", 5, "#4f46e5", "fa fa-gear", "my-custom-class");
        return View(nameof(Index));
    }

    public IActionResult NotyfRedirect()
    {
        notyf.Success("Survived the redirect");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [IgnoreAntiforgeryToken]
    public IActionResult NotyfPost()
    {
        notyf.Warning("Posted without redirect");
        return View(nameof(Index));
    }

    public IActionResult NotyfAjax(string client = "ajax")
    {
        notyf.Success($"Ajax via {client}");
        return Json(new { ok = true });
    }

    public IActionResult NotyfAjaxError()
    {
        notyf.Error("Ajax failed");
        return BadRequest();
    }

    public IActionResult NotyfHtmx()
    {
        notyf.Information("Hello htmx");
        return Content("<span id=\"htmx-result\">htmx done</span>", "text/html");
    }

    public IActionResult NotyfSticky()
    {
        notyf.Success("I am sticky", 0);
        return View(nameof(Index));
    }

    public IActionResult NotyfXss()
    {
        notyf.Success(XssMessage);
        return View(nameof(Index));
    }

    public IActionResult NotyfQuotes()
    {
        notyf.Success("It's \"quoted\" + plus & ampersand");
        return View(nameof(Index));
    }

    public IActionResult NotyfLoop()
    {
        foreach (var name in new[] { "O'Brien", "D'Angelo", "N'Golo" })
        {
            notyf.Error($"{name} is invalid");
        }
        return View(nameof(Index));
    }

    // AJAX request that redirects: the toast must survive the (invisible) redirect.
    public IActionResult NotyfAjaxRedirect()
    {
        notyf.Success("Survived an ajax redirect");
        return RedirectToAction(nameof(AjaxTarget));
    }

    public IActionResult AjaxTarget() => Json(new { ok = true });

    public IActionResult NotyfHtmlAjax()
    {
        notyf.Success(XssMessage);
        return Json(new { ok = true });
    }

    public IActionResult NotyfMany()
    {
        for (var i = 1; i <= 40; i++)
        {
            notyf.Information($"Toast {i}: " + new string('x', 150));
        }
        return Json(new { ok = true });
    }

    // ----- Toastify -----

    public IActionResult ToastifyAll()
    {
        toastify.Success("Toastify success");
        toastify.Error("Toastify error");
        toastify.Warning("Toastify warning");
        toastify.Information("Toastify information");
        toastify.Custom("Toastify custom", 5, "#4f46e5", "my-custom-class");
        return View(nameof(Index));
    }

    public IActionResult ToastifyRedirect()
    {
        toastify.Success("Toastify survived the redirect");
        return RedirectToAction(nameof(Index));
    }

    public IActionResult ToastifyAjax()
    {
        toastify.Success("Toastify ajax");
        return Json(new { ok = true });
    }

    public IActionResult ToastifySticky()
    {
        toastify.Success("Toastify sticky", 0);
        return View(nameof(Index));
    }

    // ----- Both on one page -----

    public IActionResult Both()
    {
        notyf.Success("From Notyf");
        toastify.Success("From Toastify");
        return View(nameof(Index));
    }
}
