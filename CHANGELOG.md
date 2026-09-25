# Changelog

## 2.0.0

The first release in five years. Rebuilt for .NET 8 and .NET 10, working through every open issue.

### New

- Targets **.NET 8 and .NET 10**.
- **No jQuery required.** The client scripts are plain JavaScript. (#13, #23)
- **AJAX works automatically** for `fetch`, `XMLHttpRequest`, jQuery and **htmx** - on success and error responses. (#2, #10, #17)
- **Toastify supports AJAX** too, through `app.UseToastify()`. (#2)
- **Sticky toasts**: pass `0` as the duration. (#15)
- **Per-type styles**: `BackgroundColor`, `ClassName` and `IconClassName` for Success, Error, Warning, Information and Custom. (#7)
- **Custom CSS classes**: `ClassName` for every toast, per type, or per `Custom(...)` call. (#16)
- **Font Awesome is optional**: `IncludeFontAwesome` and `FontAwesomeUrl`. (#9, #22)
- **RTL** support with `IsRtl`. (#5)
- **Full-width positions**: `NotyfPosition.TopFullWidth` and `BottomFullWidth`. (#3)
- **Accessibility**: close buttons have an `aria-label`, and Toastify's close button works from the keyboard. (#11, #12)
- **CSP friendly**: no inline JavaScript. Optional `nonce` parameter on both view components.
- `AddNotyf()` / `AddToastify()` work without a configure callback and return `IServiceCollection`.
- Assets are cache-busted with `asp-append-version`.
- Package: SourceLink, symbol package (`.snupkg`), README and icon, nullable annotations, package validation.

### Fixed

- Messages with quotes (like `O'Brien`) broke the page, and a message could break out of the `<script>` block. Messages are now passed as properly encoded JSON. (#4)
- Toastify shared one config object across requests, so concurrent requests could overwrite each other's toast text.
- `HasRippleEffect` was ignored.
- Warning/Information icons and text colours relied on Bootstrap's `text-dark` / `text-white`. They now work without Bootstrap.
- A `+` in an AJAX toast message turned into a space.
- The response header was added with `Headers.Add`, which throws if the header already exists.
- Custom toasts with a background that wasn't a hex or named colour (gradients, `rgb()`) threw a JavaScript error.
- The package couldn't be built on other machines (hard-coded icon path).
- AJAX and htmx requests that redirect no longer lose their toasts - they are handed over through TempData.
- htmx `hx-boost` navigation keeps showing Notyf toasts.
- Toast headers from other origins are ignored, so a third-party API can't inject a toast into your page.
- The AJAX response header is size-capped (4 KB); extra toasts wait for the next page instead of breaking proxies.
- Rendering the view component twice no longer shows toasts twice.
- Documented the static web assets 404 when running an unpublished app outside Development. (#14)

### Breaking changes

- Internet Explorer 11 is not supported. (#1)
- Dropped .NET Core 3.1 and .NET 5. Stay on 1.1.0 if you need them.
- Duration `0` now means sticky. It used to mean "use the default".
- Newtonsoft.Json was replaced with System.Text.Json, and the `JsonSerialization` helper is now internal.
- `AddNotyf` / `AddToastify` moved to `Microsoft.Extensions.DependencyInjection` and return `IServiceCollection`. `UseNotyf` / `UseToastify` moved to `Microsoft.AspNetCore.Builder`. Existing call sites still compile.
- `INotyfService.Custom` and `IToastifyService.Custom` gained an optional `className` parameter.
- `NotyfViewModel` and `ToastifyViewModel` changed shape. This only matters if you override the view component views.
- With `AutoHandleAjax` on (the default), `getResponseHeaders(xhr)` does nothing, since toasts are already shown automatically.
- `AddToastify` with `DurationInSeconds = 0` now defaults to 5 seconds instead of never closing.
- Notyf and Toastify use separate TempData keys, so toasts queued by v1 right before an upgrade are dropped.

## 1.1.0 - 2020-12-11

- AJAX / XHR support for Notyf.

## 1.0.0

- First release: Notyf and Toastify for ASP.NET Core 3.1 and .NET 5.
