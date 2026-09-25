# ToastNotification - Elegant Notifications For ASP.NET Core Applications

ToastNotification is a Minimal & Elegant Toast Notification Package for ASP.NET Core Web Applications that can be invoked via C#. Compatilble with ASP.NET Core 3.1 and .NET 5.

## Features

- 📱 Elegant & Responsive
- 🐣 Global Configuration to Set the Toast Position, Duration.
- 🎸 Easily integration with ASP.NET Core 3.1 and .NET 5 Applications.
- 🎃 Support to render custom HTML content within the toasts
- 🐣 Simple and Customizable. Create your own custom toast with your favorite color and icons with ease!
- 👴🏽 Works with TempData internally.
- 📱 Currently Supports 2 Popuplar JS Library.
- 📱 Supports AJAX / XHR out of the box.

More Features Coming Soon.
> Not Compatible with Blazor SDK. Intended only for ASP.NET Core Web Applications.

## Installation

```
Install-Package AspNetCoreHero.ToastNotification
```
Or

```
dotnet add package AspNetCoreHero.ToastNotification
```

Or

Get it directly from NuGet - https://www.nuget.org/packages/AspNetCoreHero.ToastNotification/


As mentioned earlier, this project / package is an ASP.NET Core Abstraction of popular Javascript libraries that are responsible for toast notifications. Currently, 2 popuplar libraries are abstracted , Notyf and ToastifyJs. You can choose to use either of them based on their look and feel. Follow the guide below for each of the toast notification library. Cheers!

# Notyf

## Usage - Notyf

Once the package is installed, open your Startup.cs and add in the following to the ConfigureServices method.

```csharp
services.AddNotyf(config=> { config.DurationInSeconds = 10;config.IsDismissable = true;config.Position = NotyfPosition.BottomRight; });
```

> Available Positions are TopRight,BottomRight,BottomLeft,TopLeft,TopCenter,BottomCenter.
Set the isDismissible bool to false, to remove the close icon from your toasts! Pretty handy.

### From v1.1.0 - AJAX / XHR is fully supported

To enable toast notification while working with AJAX Requests, you will have to add the middleware into the Service Container. Open up Startup.cs and add the following line of code under the Configure method.

```csharp
app.UseNotyf();
```

> More settings will be added in the upcoming releases

Next, open up your _Layout.cshml file and add in the following

```
 @await Component.InvokeAsync("Notyf")
```
> Make sure that you add this line after loading jquery. It is usually ideal to place this code below the essential scripts and above the  @await RenderSectionAsync("Scripts", required: false) line.

Let's add the Constructor Injection. Add the following in your controllers / razor classes to invoke the toast notifications as required.

```
public INotyfService _notifyService { get; }
public HomeController( INotyfService notifyService)
{
    _notifyService = notifyService;
}
```
Once the Injection is done, you can call the toast notification as you need. Currently 5 Types are supported.

### Success
```csharp
_notifyService.Success("This is a Success Notification");
```

### Error
```csharp
_notifyService.Error("This is an Error Notification");
```

### Warning
```csharp
_notifyService.Warning("This is a Warning Notification");
```

### Information
```csharp
_notifyService.Information("This is an Information Notification");
```
#### Set Toast Duration
By default, the toast gets dismissed in 5 seconds. You can set the duration(in seconds) after which the toast will be dismissed.
```csharp
_notifyService.Success("This toast will be dismissed in 10 seconds.",10);
```
## Custom
As the previous 4 Modes are very static in terms of color and icon, I have added a 5th type that let's you customize everything.

```csharp
_notifyService.Custom("Custom Notification - closes in 5 seconds.", 5, "whitesmoke", "fa fa-gear");
_notifyService.Custom("Custom Notification - closes in 5 seconds.", 10, "#135224", "fa fa-gear");
            
```
Here, you add the class of the icon as required. Font Awesome icons are supported by default. You would just have to pass the icon class name. The color of the text and icon is automatically set based on the color of the notification. Supports HEXR Color Codes too!

## Demo - Notyf

A Demo Implementation using ASP.NET Core MVC can be found here - https://github.com/codewithmukesh/ToastNotification/tree/master/ToastNotification.Notyf

# Toastify-Js

## Usage 

Once the package is installed, open your Startup.cs and add in the following to the ConfigureServices method.

```csharp
services.AddToastify(config=> { config.DurationInSeconds = 1000; config.Position = Position.Right; config.Gravity = Gravity.Bottom; });
```

> Available Positions are Right and Left.
> Available Gravity is Top and Bottom


> More settings will be added in the upcoming releases

Next, open up your _Layout.cshml file and add in the following

```
 @await Component.InvokeAsync("Toastify")
```
> Make sure that you add this line after loading jquery. It is usually ideal to place this code below the essential scripts and above the  @await RenderSectionAsync("Scripts", required: false) line.

Let's add the Constructor Injection. Add the following in your controllers / razor classes to invoke the toast notifications as required.

```
public IToastifyService _notifyService { get; }
public HomeController( IToastifyService notifyService)
{
    _notifyService = notifyService;
}
```
Once the Injection is done, you can call the toast notification as you need. Currently 5 Types are supported.

### Success
```csharp
_notifyService.Success("This is a Success Notification");
```

### Error
```csharp
_notifyService.Error("This is an Error Notification");
```

### Warning
```csharp
_notifyService.Warning("This is a Warning Notification");
```

### Information
```csharp
_notifyService.Information("This is an Information Notification");
```
#### Set Toast Duration
By default, the toast gets dismissed in 5 seconds. You can set the duration(in seconds) after which the toast will be dismissed.
```csharp
_notifyService.Success("This toast will be dismissed in 10 seconds.",10);
```
## Custom
As the previous 4 Modes are very static in terms of color and icon, I have added a 5th type that let's you customize everything.

```csharp
_notifyService.Custom("Custom Notification - closes in 5 seconds.", 5, "whitesmoke");
_notifyService.Custom("Custom Notification - closes in 5 seconds.", 10, "#135224");
            
```
## Demo - Toastify

A Demo Implementation using ASP.NET Core MVC can be found here - https://github.com/codewithmukesh/ToastNotification/tree/master/ToastNotification.Toastify

# Mentions

The Javascript libraries used in this project are https://github.com/caroso1222/notyf and https://apvarun.github.io/toastify-js/.

# Support
Has this Project helped you learn something New? or Helped you at work? Do Consider Supporting.

<a href="https://www.buymeacoffee.com/codewithmukesh" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" width="200"  ></a>

# About the Author
### Mukesh Murugan
- Blogs at [codewithmukesh.com](https://www.codewithmukesh.com)
- Facebook - [codewithmukesh](https://www.facebook.com/codewithmukesh)
- Twitter - [Mukesh Murugan](https://www.twitter.com/iammukeshm)
- Twitter - [codewithmukesh](https://www.twitter.com/codewithmukesh)
- Linkedin - [Mukesh Murugan](https://www.linkedin.com/in/iammukeshm/)
