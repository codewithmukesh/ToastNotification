using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Enums;

namespace AspNetCoreHero.ToastNotification.Tests.Unit;

public class NotificationTests
{
    [Fact]
    public void Null_duration_means_use_the_global_default()
    {
        new Notification(ToastNotificationType.Success, "hi", null).Duration.ShouldBeNull();
    }

    [Fact]
    public void Zero_duration_means_sticky_and_is_kept_as_zero()
    {
        // Issue #15: v1 turned 0 into null, so sticky toasts were impossible.
        new Notification(ToastNotificationType.Success, "hi", 0).Duration.ShouldBe(0);
    }

    [Theory]
    [InlineData(1, 1000)]
    [InlineData(5, 5000)]
    [InlineData(30, 30000)]
    public void Duration_is_converted_to_milliseconds(int seconds, int expected)
    {
        new Notification(ToastNotificationType.Error, "hi", seconds).Duration.ShouldBe(expected);
    }

    [Fact]
    public void Parameterless_constructor_gives_safe_defaults()
    {
        var notification = new Notification();

        notification.Message.ShouldBe(string.Empty);
        notification.Duration.ShouldBeNull();
        notification.BackgroundColor.ShouldBeNull();
        notification.ClassName.ShouldBeNull();
    }
}
