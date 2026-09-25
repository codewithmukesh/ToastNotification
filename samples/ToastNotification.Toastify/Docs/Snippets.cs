namespace ToastNotification.Toastify.Docs;

/// <summary>A code block shown on the docs page.</summary>
public sealed record CodeSample(string Id, string File, string Language, string Code);

/// <summary>
/// Every snippet on the page. Kept in one place so the docs never drift from the code in this sample.
/// </summary>
public static class Snippets
{
    public static readonly CodeSample Install = new("install", "Terminal", "bash",
        "dotnet add package AspNetCoreHero.ToastNotification");

    public static readonly CodeSample Register = new("register", "Program.cs", "csharp",
        """
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddToastify(config =>
        {
            config.DurationInSeconds = 5;
            config.Gravity = Gravity.Bottom;
            config.Position = Position.Right;
        });

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseToastify(); // toasts for fetch / AJAX / htmx requests

        app.MapRazorPages();
        app.Run();
        """);

    public static readonly CodeSample Layout = new("layout", "Pages/Shared/_Layout.cshtml", "html",
        """
            @RenderBody()

            @* Just before </body>. No jQuery needed. *@
            @await Component.InvokeAsync("Toastify")
        </body>
        """);

    public static readonly CodeSample Inject = new("inject", "Pages/Orders/Create.cshtml.cs", "csharp",
        """
        public class CreateModel(IToastifyService toastify) : PageModel
        {
            public IActionResult OnPost()
            {
                // ... save the order
                toastify.Success("Order placed!");
                return RedirectToPage("Index");
            }
        }
        """);

    public static readonly CodeSample Types = new("types", "Any page model or controller", "csharp",
        """
        toastify.Success("Order placed!");
        toastify.Error("Payment failed.");
        toastify.Warning("Stock is running low.");
        toastify.Information("Shipping starts Monday.");
        toastify.Custom("Deployed to production", 5, "linear-gradient(135deg, #5b30d6, #d63085)");
        """);

    public static readonly CodeSample Duration = new("duration", "Duration", "csharp",
        """
        toastify.Success("Gone in 2 seconds", 2);      // seconds
        toastify.Warning("Uses the global default");    // null = DurationInSeconds from AddToastify
        toastify.Error("Stays until you close it", 0);  // 0 = sticky, with a close button
        """);

    public static readonly CodeSample Redirect = new("redirect", "Pages/Index.cshtml.cs", "csharp",
        """
        public IActionResult OnPostSave()
        {
            toastify.Success("Saved! This survived a redirect.");
            return RedirectToPage(); // stored in TempData until the next page renders
        }

        public IActionResult OnPostValidate()
        {
            toastify.Error("Please fix the highlighted fields.");
            return Page(); // no redirect needed either
        }
        """);

    public static readonly CodeSample Fetch = new("fetch", "wwwroot/js/site.js", "javascript",
        """
        // Nothing to wire up - the library adds X-Requested-With to same-origin
        // fetch / XHR calls and shows the toasts from the response header.
        await fetch("?handler=Archive", { method: "POST", headers: { RequestVerificationToken: token } });

        // jQuery and htmx work the same way.
        $.post("?handler=Archive", { __RequestVerificationToken: token });
        """);

    public static readonly CodeSample Htmx = new("htmx", "Index.cshtml", "html",
        """
        <button hx-post="?handler=Htmx" hx-target="#result">Refresh stock</button>
        <div id="result"></div>
        """);

    public static readonly CodeSample MinimalApi = new("minimal-api", "Program.cs", "csharp",
        """
        // Minimal APIs can raise toasts too - handy for fetch-heavy pages.
        app.MapPost("/api/orders/{id:int}/cancel", (int id, IToastifyService toastify) =>
        {
            toastify.Warning($"Order #{id} cancelled.");
            return Results.NoContent();
        });
        """);

    public static readonly CodeSample Customize = new("customize", "Program.cs", "csharp",
        """
        builder.Services.AddToastify(config =>
        {
            config.Gravity = Gravity.Top;           // Top or Bottom
            config.Position = Position.Left;        // Left or Right
            config.IsDismissable = true;            // close button on every toast
            config.ClassName = "brand-toast";       // added to every toast

            // Your brand colours, per type (any CSS background works, gradients too)
            config.Success.BackgroundColor = "#0f766e";
            config.Error.BackgroundColor = "linear-gradient(to right, #b91c1c, #ef4444)";
            config.Warning.ClassName = "is-loud";
        });
        """);

    public static readonly CodeSample CustomClass = new("custom-class", "Custom toast with a CSS class", "csharp",
        """
        toastify.Custom("Welcome back, <b>Mukesh</b>!", 5, "#1b1712", className: "is-greeting");
        """);

    public static readonly CodeSample Csp = new("csp", "Pages/Shared/_Layout.cshtml", "html",
        """
        @* v2 renders no inline JavaScript. With a nonce-based CSP, pass your nonce: *@
        @await Component.InvokeAsync("Toastify", new { nonce = Context.Items["csp-nonce"] })
        """);

    public static readonly CodeSample Encode = new("encode", "Messages are HTML", "csharp",
        """
        // Messages render as HTML so you can use <b>, <br>, links...
        // Never pass raw user input - encode it first.
        toastify.Error($"Could not save {HtmlEncoder.Default.Encode(Input.Name)}");
        """);

    public static readonly CodeSample StaticAssets = new("static-assets", "Program.cs", "csharp",
        """
        // Running (not published) with ASPNETCORE_ENVIRONMENT=Production?
        // Package assets under /_content/ need static web assets enabled:
        builder.WebHost.UseStaticWebAssets();
        """);
}
