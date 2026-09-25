namespace ToastNotification.Docs;

/// <summary>A code block shown on the docs page.</summary>
public sealed record CodeSample(string Id, string File, string Language, string Code);

/// <summary>The same snippet for each library. The docs page shows the one matching the library toggle.</summary>
public sealed record CodePair(CodeSample Notyf, CodeSample Toastify);

/// <summary>
/// Every snippet on the docs page, kept in one place so the docs never drift from the code in this app.
/// </summary>
public static class Snippets
{
    // ---------------------------------------------------------------- shared

    public static readonly CodeSample Install = new("install", "Terminal", "bash",
        "dotnet add package AspNetCoreHero.ToastNotification");

    public static readonly CodeSample Fetch = new("fetch", "wwwroot/js/site.js", "javascript",
        """
        // Nothing to wire up - the library adds X-Requested-With to same-origin
        // fetch / XHR calls and shows the toasts from the response header.
        await fetch("/orders/archive", { method: "POST" });

        // jQuery works the same way.
        $.post("/orders/archive");
        """);

    public static readonly CodeSample Htmx = new("htmx", "Index.cshtml", "html",
        """
        <button hx-post="/orders/refresh" hx-target="#result">Refresh stock</button>
        <div id="result"></div>
        """);

    public static readonly CodeSample StaticAssets = new("static-assets", "Program.cs", "csharp",
        """
        // Running (not published) with ASPNETCORE_ENVIRONMENT=Production?
        // Package assets under /_content/ need static web assets enabled:
        builder.WebHost.UseStaticWebAssets();
        """);

    public static readonly CodeSample CheckoutPage = new("checkout", "Pages/Checkout.cshtml.cs", "csharp",
        """
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
        """);

    // ---------------------------------------------------------------- per library

    public static readonly CodePair Register = new(
        new("notyf-register", "Program.cs", "csharp",
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
            """),
        new("toastify-register", "Program.cs", "csharp",
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
            """));

    public static readonly CodePair Layout = new(
        new("notyf-layout", "Views/Shared/_Layout.cshtml", "html",
            """
                @RenderBody()

                @* Just before </body>. No jQuery needed. *@
                @await Component.InvokeAsync("Notyf")
            </body>
            """),
        new("toastify-layout", "Pages/Shared/_Layout.cshtml", "html",
            """
                @RenderBody()

                @* Just before </body>. No jQuery needed. *@
                @await Component.InvokeAsync("Toastify")
            </body>
            """));

    public static readonly CodePair Types = new(
        new("notyf-types", "Any controller, page or endpoint", "csharp",
            """
            notyf.Success("Order placed!");
            notyf.Error("Payment failed.");
            notyf.Warning("Stock is running low.");
            notyf.Information("Shipping starts Monday.");
            notyf.Custom("Deployed to production", 5, "#4c33d8", "fa fa-bolt");
            """),
        new("toastify-types", "Any controller, page or endpoint", "csharp",
            """
            toastify.Success("Order placed!");
            toastify.Error("Payment failed.");
            toastify.Warning("Stock is running low.");
            toastify.Information("Shipping starts Monday.");
            toastify.Custom("Deployed to production", 5, "linear-gradient(135deg, #5b30d6, #d63085)");
            """));

    public static readonly CodePair Duration = new(
        new("notyf-duration", "Duration", "csharp",
            """
            notyf.Success("Gone in 2 seconds", 2);      // seconds
            notyf.Warning("Uses the global default");    // null = DurationInSeconds from AddNotyf
            notyf.Error("Stays until you close it", 0);  // 0 = sticky
            """),
        new("toastify-duration", "Duration", "csharp",
            """
            toastify.Success("Gone in 2 seconds", 2);      // seconds
            toastify.Warning("Uses the global default");    // null = DurationInSeconds from AddToastify
            toastify.Error("Stays until you close it", 0);  // 0 = sticky, with a close button
            """));

    public static readonly CodePair Redirect = new(
        new("notyf-redirect", "Controllers/OrdersController.cs", "csharp",
            """
            [HttpPost]
            public IActionResult Save()
            {
                notyf.Success("Saved! This survived a redirect.");
                return RedirectToAction("Index"); // stored in TempData until the next page renders
            }

            [HttpPost]
            public IActionResult Validate()
            {
                notyf.Error("Please fix the highlighted fields.");
                return View("Index"); // no redirect needed either
            }
            """),
        new("toastify-redirect", "Controllers/OrdersController.cs", "csharp",
            """
            [HttpPost]
            public IActionResult Save()
            {
                toastify.Success("Saved! This survived a redirect.");
                return RedirectToAction("Index"); // stored in TempData until the next page renders
            }

            [HttpPost]
            public IActionResult Validate()
            {
                toastify.Error("Please fix the highlighted fields.");
                return View("Index"); // no redirect needed either
            }
            """));

    public static readonly CodePair MinimalApi = new(
        new("notyf-minimal-api", "Program.cs", "csharp",
            """
            app.MapPost("/api/orders/{id:int}/cancel", (int id, INotyfService notyf) =>
            {
                notyf.Warning($"Order #{id} cancelled.");
                return Results.NoContent();
            });
            """),
        new("toastify-minimal-api", "Program.cs", "csharp",
            """
            app.MapPost("/api/orders/{id:int}/cancel", (int id, IToastifyService toastify) =>
            {
                toastify.Warning($"Order #{id} cancelled.");
                return Results.NoContent();
            });
            """));

    public static readonly CodePair Customize = new(
        new("notyf-customize", "Program.cs", "csharp",
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

            // Or a class on a single toast
            notyf.Custom("Welcome back, <b>Mukesh</b>!", 5, "#1b1712", "fa fa-hand-peace-o", className: "is-greeting");
            """),
        new("toastify-customize", "Program.cs", "csharp",
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

            // Or a class on a single toast
            toastify.Custom("Welcome back, <b>Mukesh</b>!", 5, "#1b1712", className: "is-greeting");
            """));

    public static readonly CodePair Csp = new(
        new("notyf-csp", "_Layout.cshtml", "html",
            """
            @* v2 renders no inline JavaScript. With a nonce-based CSP, pass your nonce: *@
            @await Component.InvokeAsync("Notyf", new { nonce = Context.Items["csp-nonce"] })
            """),
        new("toastify-csp", "_Layout.cshtml", "html",
            """
            @* v2 renders no inline JavaScript. With a nonce-based CSP, pass your nonce: *@
            @await Component.InvokeAsync("Toastify", new { nonce = Context.Items["csp-nonce"] })
            """));

    public static readonly CodePair Encode = new(
        new("notyf-encode", "Messages are HTML", "csharp",
            """
            // Messages render as HTML so you can use <b>, <br>, links...
            // Never pass raw user input - encode it first.
            notyf.Error($"Could not save {HtmlEncoder.Default.Encode(request.Name)}");
            """),
        new("toastify-encode", "Messages are HTML", "csharp",
            """
            // Messages render as HTML so you can use <b>, <br>, links...
            // Never pass raw user input - encode it first.
            toastify.Error($"Could not save {HtmlEncoder.Default.Encode(request.Name)}");
            """));
}
