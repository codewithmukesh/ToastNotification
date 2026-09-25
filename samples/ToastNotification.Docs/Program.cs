using AspNetCoreHero.ToastNotification;
using ToastNotification.Docs;

var builder = WebApplication.CreateBuilder(args);

// This docs app shows every workload: MVC controllers, Razor Pages and Minimal APIs.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// 1. Register the toast library.
//    This app registers BOTH so the page can compare them. Your app only needs one.
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 5;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

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
//    One call covers both libraries (UseToastify() is the same middleware).
app.UseNotyf();

app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages();

// Minimal API endpoints behind the playground and the AJAX demos.
app.MapDemoEndpoints();

app.Run();

public partial class Program;
