using AspNetCoreHero.ToastNotification;
using ToastNotification.Toastify;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// 1. Register Toastify. Every setting is optional - AddToastify() alone works too.
builder.Services.AddToastify(config =>
{
    config.DurationInSeconds = 5;
    config.Gravity = Gravity.Bottom;
    config.Position = Position.Right;
});

// This sample also registers Notyf so the page can compare the two side by side.
// Your app only needs one of them.
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomLeft;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 2. Show toasts raised during fetch / AJAX / htmx requests.
app.UseToastify();

app.UseAuthorization();

app.MapRazorPages();

// Minimal API endpoints behind the playground and the AJAX demos.
app.MapDemoEndpoints();

app.Run();

public partial class Program;
