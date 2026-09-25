using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Enums;
using AspNetCoreHero.ToastNotification.Notyf;
using AspNetCoreHero.ToastNotification.Notyf.Models;

namespace AspNetCoreHero.ToastNotification.Tests.Unit;

public class NotyfServiceTests
{
    private readonly InMemoryNotificationContainer<NotyfNotification> _container = new();
    private readonly NotyfService _service;

    public NotyfServiceTests()
    {
        _service = new NotyfService(new FixedContainerFactory(_container));
    }

    [Fact]
    public void Each_method_queues_the_matching_type()
    {
        _service.Success("s");
        _service.Error("e");
        _service.Warning("w");
        _service.Information("i");
        _service.Custom("c");

        _container.GetAll().Select(n => n.Type).ShouldBe(
        [
            ToastNotificationType.Success,
            ToastNotificationType.Error,
            ToastNotificationType.Warning,
            ToastNotificationType.Information,
            ToastNotificationType.Custom
        ]);
        _container.GetAll().Select(n => n.Message).ShouldBe(["s", "e", "w", "i", "c"]);
    }

    [Fact]
    public void Duration_is_passed_through_in_milliseconds()
    {
        _service.Success("s", 3);

        _container.GetAll().Single().Duration.ShouldBe(3000);
    }

    [Fact]
    public void Custom_keeps_colour_icon_and_class()
    {
        _service.Custom("c", 4, "#123456", "fa fa-gear", "my-class");

        var notification = _container.GetAll().Single();
        notification.BackgroundColor.ShouldBe("#123456");
        notification.Icon.ShouldBe("fa fa-gear");
        notification.ClassName.ShouldBe("my-class");
        notification.Duration.ShouldBe(4000);
    }

    [Fact]
    public void Custom_defaults_match_v1()
    {
        _service.Custom("c");

        var notification = _container.GetAll().Single();
        notification.BackgroundColor.ShouldBe("black");
        notification.Icon.ShouldBe("home");
        notification.ClassName.ShouldBeNull();
    }

    [Fact]
    public void GetNotifications_does_not_clear_but_ReadAllNotifications_does()
    {
        _service.Success("s");

        _service.GetNotifications().Count().ShouldBe(1);
        _service.GetNotifications().Count().ShouldBe(1);
        _service.ReadAllNotifications().Count().ShouldBe(1);
        _service.GetNotifications().ShouldBeEmpty();
    }

    [Fact]
    public void RemoveAll_clears_the_queue()
    {
        _service.Success("s");
        _service.Error("e");

        _service.RemoveAll();

        _service.GetNotifications().ShouldBeEmpty();
    }
}
