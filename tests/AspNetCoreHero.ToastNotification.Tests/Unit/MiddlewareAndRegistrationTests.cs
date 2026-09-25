using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Middlewares;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using AspNetCoreHero.ToastNotification.Toastify.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace AspNetCoreHero.ToastNotification.Tests.Unit;

public class ToastNotificationMiddlewareTests
{
    private static DefaultHttpContext CreateContext(bool ajax, Action<INotyfService>? notyf = null, Action<IToastifyService>? toastify = null)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddNotyf();
        services.AddToastify();
        services.AddSingleton<ITempDataDictionaryFactory>(_ => new SingleTempDataFactory(new TempDataDictionary(new DefaultHttpContext(), new InMemoryTempDataProvider())));
        var provider = services.BuildServiceProvider();

        var httpContext = new DefaultHttpContext { RequestServices = provider.CreateScope().ServiceProvider };
        if (ajax)
        {
            httpContext.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
        }
        provider.GetRequiredService<IHttpContextAccessor>().HttpContext = httpContext;

        notyf?.Invoke(httpContext.RequestServices.GetRequiredService<INotyfService>());
        toastify?.Invoke(httpContext.RequestServices.GetRequiredService<IToastifyService>());
        return httpContext;
    }

    [Fact]
    public void Ajax_response_gets_url_encoded_json_header()
    {
        var context = CreateContext(ajax: true, notyf: n => n.Success("Saved + done & it's 100%"));

        ToastNotificationMiddleware.AppendNotificationHeaders(context);

        var header = context.Response.Headers["X-Notyf-Notifications"].ToString();
        header.ShouldNotContain(" ");
        var json = JsonDocument.Parse(Uri.UnescapeDataString(header)).RootElement;
        json[0].GetProperty("message").GetString().ShouldBe("Saved + done & it's 100%");
        json[0].GetProperty("type").GetInt32().ShouldBe(0);
        context.Response.Headers["Access-Control-Expose-Headers"].ToString().ShouldContain("X-Notyf-Notifications");
    }

    [Fact]
    public void Toastify_notifications_use_their_own_header()
    {
        var context = CreateContext(ajax: true, toastify: t => t.Error("nope"));

        ToastNotificationMiddleware.AppendNotificationHeaders(context);

        context.Response.Headers.ContainsKey("X-Toastify-Notifications").ShouldBeTrue();
        context.Response.Headers.ContainsKey("X-Notyf-Notifications").ShouldBeFalse();
    }

    [Fact]
    public void No_header_when_nothing_was_queued()
    {
        var context = CreateContext(ajax: true);

        ToastNotificationMiddleware.AppendNotificationHeaders(context);

        context.Response.Headers.ContainsKey("X-Notyf-Notifications").ShouldBeFalse();
        context.Response.Headers.ContainsKey("Access-Control-Expose-Headers").ShouldBeFalse();
    }

    [Fact]
    public void Regular_requests_are_left_alone()
    {
        var context = CreateContext(ajax: false, notyf: n => n.Success("for the next page"));

        ToastNotificationMiddleware.AppendNotificationHeaders(context);

        context.Response.Headers.ContainsKey("X-Notyf-Notifications").ShouldBeFalse();
    }

    [Fact]
    public async Task Registering_the_middleware_twice_hooks_the_response_once()
    {
        var context = CreateContext(ajax: true);
        var calls = 0;
        var first = new ToastNotificationMiddleware(_ => { calls++; return Task.CompletedTask; });

        await first.InvokeAsync(context);
        await first.InvokeAsync(context);

        calls.ShouldBe(2);
        context.Items.Count.ShouldBe(1);
    }
}

public class ServiceRegistrationTests
{
    private static ServiceProvider Build(Action<IServiceCollection> configure)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllersWithViews();
        configure(services);
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }

    [Fact]
    public void AddNotyf_works_without_a_configure_callback()
    {
        using var provider = Build(s => s.AddNotyf());
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<INotyfService>().ShouldNotBeNull();
        provider.GetRequiredService<NotyfEntity>().duration.ShouldBe(5000);
        provider.GetRequiredService<NotyfConfig>().ShouldNotBeNull();
    }

    [Fact]
    public void AddNotyf_applies_the_configuration()
    {
        using var provider = Build(s => s.AddNotyf(c =>
        {
            c.DurationInSeconds = 9;
            c.Position = NotyfPosition.TopLeft;
        }));

        var entity = provider.GetRequiredService<NotyfEntity>();
        entity.duration.ShouldBe(9000);
        entity.position.x.ShouldBe("left");
    }

    [Fact]
    public void Calling_AddNotyf_twice_keeps_the_last_configuration_and_one_registration()
    {
        var services = new ServiceCollection();
        services.AddNotyf(c => c.DurationInSeconds = 1);
        services.AddNotyf(c => c.DurationInSeconds = 2);

        services.Count(d => d.ServiceType == typeof(INotyfService)).ShouldBe(1);
        services.Count(d => d.ServiceType == typeof(NotyfEntity)).ShouldBe(1);
        services.Count(d => d.ServiceType == typeof(IMessageContainerFactory)).ShouldBe(1);
        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<NotyfEntity>().duration.ShouldBe(2000);
    }

    [Fact]
    public void Notyf_and_Toastify_can_be_registered_together()
    {
        using var provider = Build(s =>
        {
            s.AddNotyf();
            s.AddToastify(c => c.IsDismissable = true);
        });
        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetRequiredService<INotyfService>().ShouldNotBeNull();
        scope.ServiceProvider.GetRequiredService<IToastifyService>().ShouldNotBeNull();
        provider.GetRequiredService<ToastifyEntity>().close.ShouldBeTrue();
    }

    [Fact]
    public void Null_services_throw()
    {
        Should.Throw<ArgumentNullException>(() => ToastNotificationServiceCollectionExtensions.AddNotyf(null!));
        Should.Throw<ArgumentNullException>(() => ToastNotificationServiceCollectionExtensions.AddToastify(null!));
    }
}
