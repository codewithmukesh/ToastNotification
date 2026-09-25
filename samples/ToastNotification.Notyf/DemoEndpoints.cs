using AspNetCoreHero.ToastNotification.Abstractions;
using System.Text.Encodings.Web;

namespace ToastNotification.Notyf;

/// <summary>
/// Minimal API endpoints behind the playground, compare and fetch/jQuery/htmx demos.
/// Each one raises a toast with the library's service - the library shows it from the response header.
/// </summary>
public static class DemoEndpoints
{
    public sealed record ToastRequest(string Library, string Type, string? Message, int? Duration, string? Color);

    public static void MapDemoEndpoints(this IEndpointRouteBuilder app)
    {
        var demo = app.MapGroup("/demo").DisableAntiforgery();

        demo.MapPost("/toast", (ToastRequest request, INotyfService notyf, IToastifyService toastify) =>
        {
            // The message comes from the playground input, i.e. user input - encode it.
            var message = HtmlEncoder.Default.Encode(string.IsNullOrWhiteSpace(request.Message) ? DefaultMessage(request.Type) : request.Message);
            var duration = request.Duration is >= 0 and <= 60 ? request.Duration : null;
            var color = IsSafeColor(request.Color) ? request.Color : "#8b6cff";

            if (request.Library == "toastify")
            {
                switch (request.Type)
                {
                    case "Success": toastify.Success(message, duration); break;
                    case "Error": toastify.Error(message, duration); break;
                    case "Warning": toastify.Warning(message, duration); break;
                    case "Information": toastify.Information(message, duration); break;
                    default: toastify.Custom(message, duration, color); break;
                }
            }
            else
            {
                switch (request.Type)
                {
                    case "Success": notyf.Success(message, duration); break;
                    case "Error": notyf.Error(message, duration); break;
                    case "Warning": notyf.Warning(message, duration); break;
                    case "Information": notyf.Information(message, duration); break;
                    default: notyf.Custom(message, duration, color, "fa fa-bolt"); break;
                }
            }
            return Results.NoContent();
        });

        demo.MapPost("/archive", (string client, INotyfService notyf) =>
        {
            notyf.Success($"Archived via {client}.");
            return Results.NoContent();
        });

        demo.MapPost("/fail", (INotyfService notyf) =>
        {
            notyf.Error("The server said no (HTTP 400).");
            return Results.BadRequest();
        });

        demo.MapPost("/htmx", (INotyfService notyf) =>
        {
            notyf.Information("Stock refreshed.");
            return Results.Content($"Updated at {DateTime.Now:HH:mm:ss}", "text/html");
        });
    }

    private static string DefaultMessage(string type) => type switch
    {
        "Success" => "Order placed!",
        "Error" => "Payment failed.",
        "Warning" => "Stock is running low.",
        "Information" => "Shipping starts Monday.",
        _ => "Deployed to production"
    };

    // Only allow simple hex colours from the browser.
    private static bool IsSafeColor(string? color)
        => color is { Length: 4 or 7 } && color[0] == '#' && color.Skip(1).All(Uri.IsHexDigit);
}
