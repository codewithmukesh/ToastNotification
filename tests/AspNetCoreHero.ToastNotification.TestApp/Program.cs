var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 2;
    config.IsDismissable = true;
    config.IncludeFontAwesome = false; // keep tests offline
});
builder.Services.AddToastify(config => config.DurationInSeconds = 2);

var app = builder.Build();

// Strict CSP for any request with ?csp=1 - proves the library needs no inline script.
app.Use(async (context, next) =>
{
    if (context.Request.Query.ContainsKey("csp"))
    {
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'";
    }
    await next(context);
});

app.UseStaticFiles();
app.UseRouting();
app.UseNotyf();
app.UseToastify();

// A "foreign" API that tries to inject a toast through CORS. The client script must ignore it.
app.MapGet("/cors-evil", (HttpContext context) =>
{
    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
    context.Response.Headers["Access-Control-Expose-Headers"] = "X-Notyf-Notifications";
    context.Response.Headers["X-Notyf-Notifications"] = Uri.EscapeDataString("[{\"type\":0,\"message\":\"evil toast\"}]");
    return Results.Ok();
});

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program;
