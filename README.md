# ToastNotification for ASP.NET Core

[![NuGet](https://img.shields.io/nuget/v/AspNetCoreHero.ToastNotification.svg)](https://www.nuget.org/packages/AspNetCoreHero.ToastNotification)
[![Downloads](https://img.shields.io/nuget/dt/AspNetCoreHero.ToastNotification.svg)](https://www.nuget.org/packages/AspNetCoreHero.ToastNotification)
[![Build](https://github.com/codewithmukesh/ToastNotification/actions/workflows/ci.yml/badge.svg)](https://github.com/codewithmukesh/ToastNotification/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Toast notifications for ASP.NET Core MVC and Razor Pages - straight from your C# code.

```csharp
_notyf.Success("Order placed!");
```

That's it. The toast shows up on the page, whether you redirect, return a view, or call the endpoint with `fetch`, jQuery or htmx.

- ✅ .NET 8 and .NET 10
- ✅ MVC, Razor Pages and Minimal APIs
- ✅ Works after redirects (TempData), form posts, `fetch`, jQuery AJAX and htmx
- ✅ No jQuery required
- ✅ No inline scripts - works with a strict Content Security Policy
- ✅ Two JS libraries to pick from: [Notyf](https://github.com/caroso1222/notyf) and [Toastify](https://github.com/apvarun/toastify-js)
- ✅ Custom colours, icons, CSS classes, positions, RTL and sticky toasts

## Getting Started

### 1. Install the package

```bash
dotnet add package AspNetCoreHero.ToastNotification
```

### 2. Register it in `Program.cs`

```csharp
builder.Services.AddNotyf();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseNotyf(); // shows toasts raised during fetch / AJAX / htmx calls
```

You don't need any extra `using` lines for these two.

### 3. Add one line to your layout

Open `Views/Shared/_Layout.cshtml` (or `Pages/Shared/_Layout.cshtml`) and add this just before `</body>`:

```cshtml
@await Component.InvokeAsync("Notyf")
```

### 4. Show a toast

Inject `INotyfService` and call it:

```csharp
public class OrdersController(INotyfService notyf) : Controller
{
    [HttpPost]
    public IActionResult Create(CreateOrderRequest request)
    {
        // save the order...
        notyf.Success("Order placed!");
        return RedirectToAction(nameof(Index));
    }
}
```

Run the app and you'll see the toast after the redirect. That's the whole setup.

## Notification Types

```csharp
notyf.Success("Order placed!");
notyf.Error("Payment failed.");
notyf.Warning("Stock is running low.");
notyf.Information("Shipping starts Monday.");
notyf.Custom("Deployed to production", 5, "#5b30d6", "fa fa-rocket");
```

## Duration and Sticky Toasts

```csharp
notyf.Success("Gone in 2 seconds", 2);      // duration in seconds
notyf.Warning("Uses the global default");    // null = the DurationInSeconds you configured
notyf.Error("Stays until you close it", 0);  // 0 = sticky
```

Sticky toasts always get a close button.

## AJAX, fetch and htmx

This works out of the box once `app.UseNotyf()` is in place. Raise the toast in your endpoint as usual:

```csharp
[HttpPost]
public IActionResult Archive(int id)
{
    notyf.Success("Order archived.");
    return NoContent();
}
```

Then call it however you like - the toast shows up on its own:

```js
await fetch("/orders/archive/42", { method: "POST" });
```

The same goes for jQuery (`$.post(...)`), plain `XMLHttpRequest` and htmx. Error responses (400, 500...) carry toasts too.

How it works: for same-origin requests the library adds an `X-Requested-With` header, the server sends the toasts back in a response header, and the script shows them. For cross-origin calls, add `X-Requested-With: XMLHttpRequest` to the request yourself.

Minimal APIs work the same way:

```csharp
app.MapPost("/api/orders/{id:int}/cancel", (int id, INotyfService notyf) =>
{
    notyf.Warning($"Order #{id} cancelled.");
    return Results.NoContent();
});
```

## Configuration

Every setting is optional.

```csharp
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.Position = NotyfPosition.BottomRight;
    config.IsDismissable = true;

    // Per-type colours, classes and icons
    config.Success.BackgroundColor = "#0f766e";
    config.Error.ClassName = "shake";
    config.Warning.IconClassName = "bi bi-exclamation-triangle";

    // Everything else
    config.IsRtl = true;
    config.ClassName = "brand-toast";
    config.IncludeFontAwesome = false;
});
```

| Setting | Default | What it does |
|---|---|---|
| `DurationInSeconds` | `5` | Default duration for every toast. |
| `Position` | `BottomRight` | `TopRight`, `BottomRight`, `BottomLeft`, `TopLeft`, `TopCenter`, `BottomCenter`, `TopFullWidth`, `BottomFullWidth`. |
| `IsDismissable` | `false` | Shows a close button on every toast. |
| `HasRippleEffect` | `true` | Notyf's ripple animation. |
| `IsRtl` | `false` | Right-to-left layout. |
| `ClassName` | - | Extra CSS class on every toast. |
| `Success`, `Error`, `Warning`, `Information`, `Custom` | v1 colours | `BackgroundColor`, `ClassName` and `IconClassName` for each type. |
| `IncludeFontAwesome` | `true` | Loads Font Awesome 4.7 for the Warning / Information icons. Turn it off if you already load your own icons. |
| `FontAwesomeUrl` | CDN | Point it to a local copy if you don't want the CDN. |
| `AutoHandleAjax` | `true` | Shows toasts from fetch / AJAX / htmx responses automatically. |

You can also pass a CSS class to a single toast:

```csharp
notyf.Custom("Welcome back, <b>Mukesh</b>!", 5, "#1b1712", "fa fa-hand-peace-o", className: "is-greeting");
```

## Using Toastify Instead of Notyf

Prefer Toastify's look? Swap the three lines:

```csharp
builder.Services.AddToastify(config =>
{
    config.DurationInSeconds = 5;
    config.Gravity = Gravity.Bottom;   // Top or Bottom
    config.Position = Position.Right;  // Left or Right
});

app.UseToastify();
```

```cshtml
@await Component.InvokeAsync("Toastify")
```

Then inject `IToastifyService` instead of `INotyfService`. The methods are the same. `Custom` takes any CSS background, gradients included:

```csharp
toastify.Custom("Deployed!", 5, "linear-gradient(135deg, #5b30d6, #d63085)");
```

## Security

**Messages are rendered as HTML.** That's what lets you use `<b>` or `<br>` in a toast. It also means you should never pass raw user input. Encode it first:

```csharp
notyf.Error($"Could not save {HtmlEncoder.Default.Encode(request.Name)}");
```

**Content Security Policy.** v2 doesn't render any inline JavaScript. The toasts travel in a JSON data block, so `script-src 'self'` works as is. If your CSP uses nonces, pass yours in:

```cshtml
@await Component.InvokeAsync("Notyf", new { nonce = Context.Items["csp-nonce"] })
```

## Samples

The [samples](samples) folder has two runnable apps, and each one doubles as the documentation site. You get a live playground (build a toast, see the C# update, fire it from the server), Notyf and Toastify side by side, a setup wizard that gives you the exact code for MVC, Razor Pages or Minimal APIs, and every docs section with a button to try it.

- [`ToastNotification.Notyf`](samples/ToastNotification.Notyf) - Notyf with ASP.NET Core MVC and Minimal API endpoints.
- [`ToastNotification.Toastify`](samples/ToastNotification.Toastify) - Toastify with Razor Pages and Minimal API endpoints.

```bash
cd samples/ToastNotification.Notyf
dotnet run
```

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| 404 on `/_content/AspNetCoreHero.ToastNotification/...` | You're running an unpublished app outside the Development environment, so static web assets are off. | Publish the app, or add `builder.WebHost.UseStaticWebAssets();`. |
| No toast after a fetch / AJAX call | `app.UseNotyf()` is missing, or the call goes to another origin. | Add `UseNotyf()`. For cross-origin calls, send `X-Requested-With: XMLHttpRequest`. |
| Toast shows twice after upgrading from v1 | Your code still calls `getResponseHeaders(xhr)`. | Remove the call. v2 handles AJAX on its own. |
| Warning / Information icons are missing | `IncludeFontAwesome = false` and no other icon font on the page. | Load Font Awesome yourself, or set `IconClassName` for those types. |

## Upgrading from v1

Most apps only need to bump the package version. Here's what changed:

- **Targets .NET 8 and .NET 10.** .NET Core 3.1 and .NET 5 are no longer supported. Stay on 1.1.0 if you need them.
- **jQuery is no longer needed**, and the component can sit anywhere before `</body>`.
- **Duration `0` now means sticky.** In v1 it meant "use the default". Pass `null` (or nothing) for the default.
- **AJAX works automatically**, including `fetch` and htmx. Remove any manual `getResponseHeaders(xhr)` calls.
- **Newtonsoft.Json is gone.** The library uses System.Text.Json now, and the internal `JsonSerialization` helper is no longer public.
- `AddNotyf()` / `AddToastify()` now return `IServiceCollection` and live in the `Microsoft.Extensions.DependencyInjection` namespace. `UseNotyf()` / `UseToastify()` live in `Microsoft.AspNetCore.Builder`. Your existing code still compiles.
- `INotyfService.Custom` and `IToastifyService.Custom` have a new optional `className` parameter. If you wrote your own implementation of these interfaces, add the parameter.
- Toastify now supports AJAX too. Add `app.UseToastify()`.

The full list is in the [CHANGELOG](CHANGELOG.md).

## Contributing

Issues and pull requests are welcome. To build and test locally:

```bash
dotnet build
dotnet test --solution AspNetCoreHero.ToastNotification.slnx
```

The browser tests use Playwright. Install Chromium once with:

```bash
pwsh tests/AspNetCoreHero.ToastNotification.E2ETests/bin/Debug/net10.0/playwright.ps1 install chromium
```

## Credits

This package wraps two great open-source libraries: [Notyf](https://github.com/caroso1222/notyf) by Carlos Roso and [Toastify](https://github.com/apvarun/toastify-js) by Varun A P.

## Support

If this package saves you time, consider supporting it.

<a href="https://www.buymeacoffee.com/codewithmukesh" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" width="200"></a>

## About the Author

I'm Mukesh Murugan. I write about .NET at [codewithmukesh.com](https://codewithmukesh.com) - free courses, deep dives and a weekly newsletter for .NET developers.

- LinkedIn - [Mukesh Murugan](https://www.linkedin.com/in/iammukeshm/)
- X - [@iammukeshm](https://x.com/iammukeshm)

## License

MIT
