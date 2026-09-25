using AspNetCoreHero.ToastNotification.Containers;
using AspNetCoreHero.ToastNotification.Enums;
using AspNetCoreHero.ToastNotification.Toastify;
using AspNetCoreHero.ToastNotification.Toastify.Models;

namespace AspNetCoreHero.ToastNotification.Tests.Unit;

public class ToastifyServiceTests
{
    private readonly InMemoryNotificationContainer<ToastifyNotification> _container = new();

    [Fact]
    public void Each_method_queues_the_matching_type()
    {
        var service = new ToastifyService(_container);

        service.Success("s");
        service.Error("e");
        service.Warning("w");
        service.Information("i");
        service.Custom("c");

        _container.GetAll().Select(n => n.Type).ShouldBe(
        [
            ToastNotificationType.Success,
            ToastNotificationType.Error,
            ToastNotificationType.Warning,
            ToastNotificationType.Information,
            ToastNotificationType.Custom
        ]);
    }

    [Fact]
    public void Custom_keeps_background_and_class()
    {
        var service = new ToastifyService(_container);

        service.Custom("c", 2, "red", "my-class");

        var notification = _container.GetAll().Single();
        notification.BackgroundColor.ShouldBe("red");
        notification.ClassName.ShouldBe("my-class");
        notification.Duration.ShouldBe(2000);
    }

    [Fact]
    public void Custom_default_background_matches_v1()
    {
        var service = new ToastifyService(_container);

        service.Custom("c");

        _container.GetAll().Single().BackgroundColor.ShouldBe("linear-gradient(to right, #00b09b, #96c93d)");
    }

    [Fact]
    public void Factory_constructor_uses_the_container_from_the_factory()
    {
        var service = new ToastifyService(new FixedContainerFactory(_container));

        service.Success("s");

        _container.GetAll().Count.ShouldBe(1);
        service.ReadAllNotifications().Count().ShouldBe(1);
        service.GetNotifications().ShouldBeEmpty();
    }
}
