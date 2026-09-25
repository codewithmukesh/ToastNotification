using AspNetCoreHero.ToastNotification;
using ToastNotification.Notyf;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 1. Register Notyf. Every setting is optional - AddNotyf() alone works too.
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

// This sample also registers Toastify so the page can compare the two side by side.
// Your app only needs one of them.
builder.Services.AddToastify(config =>
{
    config.DurationInSeconds = 5;
    config.Gravity = Gravity.Bottom;
    config.Position = Position.Left;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 2. Show toasts raised during fetch / AJAX / htmx requests.
app.UseNotyf();

app.UseAuthorization();

app.MapDefaultControllerRoute();

// Minimal API endpoints behind the playground and the AJAX demos.
app.MapDemoEndpoints();

app.Run();

public partial class Program;
