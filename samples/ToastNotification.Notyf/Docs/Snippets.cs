namespace ToastNotification.Notyf.Docs;

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

        builder.Services.AddControllersWithViews();
        builder.Services.AddNotyf(config =>
        {
            config.DurationInSeconds = 5;
            config.IsDismissable = true;
            config.Position = NotyfPosition.BottomRight;
        });

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseNotyf(); // toasts for fetch / AJAX / htmx requests

        app.MapDefaultControllerRoute();
        app.Run();
        """);

    public static readonly CodeSample Layout = new("layout", "Views/Shared/_Layout.cshtml", "html",
        """
            @RenderBody()

            @* Just before </body>. No jQuery needed. *@
            @await Component.InvokeAsync("Notyf")
        </body>
        """);

    public static readonly CodeSample Inject = new("inject", "Controllers/OrdersController.cs", "csharp",
        """
        public class OrdersController(INotyfService notyf) : Controller
        {
            [HttpPost]
            public IActionResult Create(CreateOrderRequest request)
            {
                // ... save the order
                notyf.Success("Order placed!");
                return RedirectToAction(nameof(Index));
            }
        }
        """);

    public static readonly CodeSample Types = new("types", "Any controller or page", "csharp",
        """
        notyf.Success("Order placed!");
        notyf.Error("Payment failed.");
        notyf.Warning("Stock is running low.");
        notyf.Information("Shipping starts Monday.");
        notyf.Custom("Deployed to production", 5, "#5b30d6", "fa fa-rocket");
        """);

    public static readonly CodeSample Duration = new("duration", "Duration", "csharp",
        """
        notyf.Success("Gone in 2 seconds", 2);      // seconds
        notyf.Warning("Uses the global default");    // null = DurationInSeconds from AddNotyf
        notyf.Error("Stays until you close it", 0);  // 0 = sticky
        """);

    public static readonly CodeSample Redirect = new("redirect", "Controllers/DemoController.cs", "csharp",
        """
        [HttpPost]
        public IActionResult Save()
        {
            notyf.Success("Saved! This survived a redirect.");
            return RedirectToAction("Index"); // stored in TempData until the next page renders
        }
        """);

    public static readonly CodeSample Fetch = new("fetch", "wwwroot/js/site.js", "javascript",
        """
        // Nothing to wire up - the library adds X-Requested-With to same-origin
        // fetch / XHR calls and shows the toasts from the response header.
        await fetch("/orders/archive", { method: "POST" });

        // jQuery and htmx work the same way.
        $.post("/orders/archive");
        """);

    public static readonly CodeSample MinimalApi = new("minimal-api", "Program.cs", "csharp",
        """
        // Minimal APIs can raise toasts too - handy for fetch-heavy pages.
        app.MapPost("/api/orders/{id:int}/cancel", (int id, INotyfService notyf) =>
        {
            notyf.Warning($"Order #{id} cancelled.");
            return Results.NoContent();
        });
        """);

    public static readonly CodeSample Htmx = new("htmx", "Index.cshtml", "html",
        """
        <button hx-post="/demo/htmx" hx-target="#result">Refresh stock</button>
        <div id="result"></div>
        """);

    public static readonly CodeSample Customize = new("customize", "Program.cs", "csharp",
        """
        builder.Services.AddNotyf(config =>
        {
            config.Position = NotyfPosition.TopFullWidth;   // 8 positions, incl. full width
            config.IsRtl = true;                            // right-to-left layouts
            config.ClassName = "brand-toast";               // added to every toast

            // Your brand colours and icons, per type
            config.Success.BackgroundColor = "#0f766e";
            config.Error.ClassName = "shake";
            config.Warning.IconClassName = "bi bi-exclamation-triangle";

            // Bring your own icon font (or none at all)
            config.IncludeFontAwesome = false;
        });
        """);

    public static readonly CodeSample CustomClass = new("custom-class", "Custom toast with a CSS class", "csharp",
        """
        notyf.Custom("Welcome back, <b>Mukesh</b>!", 5, "#1b1712", "fa fa-hand-peace-o", className: "is-greeting");
        """);

    public static readonly CodeSample Csp = new("csp", "Views/Shared/_Layout.cshtml", "html",
        """
        @* v2 renders no inline JavaScript. With a nonce-based CSP, pass your nonce: *@
        @await Component.InvokeAsync("Notyf", new { nonce = Context.Items["csp-nonce"] })
        """);

    public static readonly CodeSample Encode = new("encode", "Messages are HTML", "csharp",
        """
        // Messages render as HTML so you can use <b>, <br>, links...
        // Never pass raw user input - encode it first.
        notyf.Error($"Could not save {HtmlEncoder.Default.Encode(request.Name)}");
        """);

    public static readonly CodeSample StaticAssets = new("static-assets", "Program.cs", "csharp",
        """
        // Running (not published) with ASPNETCORE_ENVIRONMENT=Production?
        // Package assets under /_content/ need static web assets enabled:
        builder.WebHost.UseStaticWebAssets();
        """);
}
