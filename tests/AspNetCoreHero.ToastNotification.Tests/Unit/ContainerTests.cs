using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Enums;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using AspNetCoreHero.ToastNotification.Toastify.Models;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreHero.ToastNotification.Tests.Unit;

public class InMemoryNotificationContainerTests
{
    [Fact]
    public void ReadAll_returns_a_copy_and_clears()
    {
        var container = new InMemoryNotificationContainer<NotyfNotification>();
        container.Add(new NotyfNotification(ToastNotificationType.Success, "a", null));
        container.Add(new NotyfNotification(ToastNotificationType.Error, "b", null));

        var read = container.ReadAll();

        read.Count.ShouldBe(2);
        container.GetAll().ShouldBeEmpty();
    }
}

public class TempDataToastNotificationContainerTests
{
    [Fact]
    public void Notifications_round_trip_through_TempData_json()
    {
        var (tempDataService, _, _) = TempDataFixture.Create();
        var container = new TempDataToastNotificationContainer<NotyfNotification>(tempDataService);

        container.Add(new NotyfNotification(ToastNotificationType.Custom, "<b>Hi</b> it's me", 0)
        {
            BackgroundColor = "#fff",
            Icon = "fa fa-gear",
            ClassName = "extra"
        });

        var stored = container.GetAll().Single();
        stored.Type.ShouldBe(ToastNotificationType.Custom);
        stored.Message.ShouldBe("<b>Hi</b> it's me");
        stored.Duration.ShouldBe(0);
        stored.BackgroundColor.ShouldBe("#fff");
        stored.Icon.ShouldBe("fa fa-gear");
        stored.ClassName.ShouldBe("extra");
    }

    [Fact]
    public void Multiple_adds_append_in_order()
    {
        var (tempDataService, _, _) = TempDataFixture.Create();
        var container = new TempDataToastNotificationContainer<NotyfNotification>(tempDataService);

        container.Add(new NotyfNotification(ToastNotificationType.Success, "1", null));
        container.Add(new NotyfNotification(ToastNotificationType.Success, "2", null));
        container.Add(new NotyfNotification(ToastNotificationType.Success, "3", null));

        container.GetAll().Select(n => n.Message).ShouldBe(["1", "2", "3"]);
    }

    [Fact]
    public void Notifications_survive_to_the_next_request_until_read()
    {
        // Request 1 queues and saves (e.g. then redirects).
        var provider = new InMemoryTempDataProvider();
        var (request1, tempData1, _) = TempDataFixture.Create(provider);
        new TempDataToastNotificationContainer<NotyfNotification>(request1)
            .Add(new NotyfNotification(ToastNotificationType.Success, "saved", null));
        tempData1.Save();

        // Request 2 renders them and they are consumed.
        var (request2, tempData2, _) = TempDataFixture.Create(provider);
        var container2 = new TempDataToastNotificationContainer<NotyfNotification>(request2);
        container2.ReadAll().Single().Message.ShouldBe("saved");
        tempData2.Save();

        // Request 3 sees nothing.
        var (request3, _, _) = TempDataFixture.Create(provider);
        new TempDataToastNotificationContainer<NotyfNotification>(request3).GetAll().ShouldBeEmpty();
    }

    [Fact]
    public void Notyf_and_Toastify_use_separate_TempData_keys()
    {
        var (tempDataService, _, _) = TempDataFixture.Create();
        new TempDataToastNotificationContainer<NotyfNotification>(tempDataService)
            .Add(new NotyfNotification(ToastNotificationType.Success, "notyf", null));

        new TempDataToastNotificationContainer<ToastifyNotification>(tempDataService).GetAll().ShouldBeEmpty();
        TempDataToastNotificationContainer<NotyfNotification>.Key
            .ShouldNotBe(TempDataToastNotificationContainer<ToastifyNotification>.Key);
    }
}

public class MessageContainerFactoryTests
{
    private static IMessageContainerFactory CreateFactory(HttpContext? httpContext)
    {
        var (tempDataService, _, _) = TempDataFixture.Create();
        return new MessageContainerFactory(new HttpContextAccessor { HttpContext = httpContext }, tempDataService);
    }

    [Fact]
    public void Regular_requests_use_TempData()
    {
        CreateFactory(new DefaultHttpContext()).Create<NotyfNotification>()
            .ShouldBeOfType<TempDataToastNotificationContainer<NotyfNotification>>();
    }

    [Theory]
    [InlineData("X-Requested-With", "XMLHttpRequest")]
    [InlineData("x-requested-with", "Notyf")]
    [InlineData("HX-Request", "true")]
    public void Ajax_and_htmx_requests_use_memory(string header, string value)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[header] = value;

        CreateFactory(httpContext).Create<NotyfNotification>()
            .ShouldBeOfType<InMemoryNotificationContainer<NotyfNotification>>();
    }

    [Fact]
    public void No_HttpContext_falls_back_to_memory_instead_of_throwing()
    {
        CreateFactory(null).Create<NotyfNotification>()
            .ShouldBeOfType<InMemoryNotificationContainer<NotyfNotification>>();
    }
}
