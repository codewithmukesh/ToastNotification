using AspNetCoreHero.ToastNotification.Helpers;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using AspNetCoreHero.ToastNotification.Toastify.Models;
using AspNetCoreHero.ToastNotification.Views.Shared.Components.Notyf;
using AspNetCoreHero.ToastNotification.Views.Shared.Components.Toastify;
using System.Text.Json;

namespace AspNetCoreHero.ToastNotification.Tests.Unit;

public class NotyfEntityTests
{
    [Theory]
    [InlineData(NotyfPosition.TopRight, "right", "top")]
    [InlineData(NotyfPosition.BottomRight, "right", "bottom")]
    [InlineData(NotyfPosition.BottomLeft, "left", "bottom")]
    [InlineData(NotyfPosition.TopLeft, "left", "top")]
    [InlineData(NotyfPosition.TopCenter, "center", "top")]
    [InlineData(NotyfPosition.BottomCenter, "center", "bottom")]
    [InlineData(NotyfPosition.TopFullWidth, "center", "top")]
    [InlineData(NotyfPosition.BottomFullWidth, "center", "bottom")]
    public void Every_position_maps_to_notyf_coordinates(NotyfPosition position, string x, string y)
    {
        var entity = new NotyfEntity(new NotyfConfig { Position = position });

        entity.position.x.ShouldBe(x);
        entity.position.y.ShouldBe(y);
    }

    [Theory]
    [InlineData(0, 5000)]
    [InlineData(-1, 5000)]
    [InlineData(3, 3000)]
    public void Global_duration_defaults_to_five_seconds(int seconds, int expected)
    {
        new NotyfEntity(new NotyfConfig { DurationInSeconds = seconds }).duration.ShouldBe(expected);
    }

    [Fact]
    public void Dismissible_and_ripple_come_from_config()
    {
        // HasRippleEffect was ignored in v1.
        var entity = new NotyfEntity(new NotyfConfig { IsDismissable = true, HasRippleEffect = false });

        entity.dismissible.ShouldBeTrue();
        entity.ripple.ShouldBeFalse();
    }

    [Fact]
    public void Default_type_styles_match_v1()
    {
        var types = new NotyfEntity(new NotyfConfig()).types.ToDictionary(t => t.type!);

        types.Keys.ShouldBe(["success", "error", "warning", "info", "custom"]);
        types["success"].background.ShouldBe("#28a745");
        types["error"].background.ShouldBe("#dc3545");
        types["warning"].background.ShouldBe("orange");
        types["warning"].icon!.className.ShouldBe("fa fa-warning text-dark");
        types["info"].icon!.className.ShouldBe("fa fa-info text-white");
        types["success"].icon.ShouldBeNull(); // Notyf's built-in icon
    }

    [Fact]
    public void Type_styles_can_be_customised()
    {
        // Issues #7 and #16.
        var config = new NotyfConfig();
        config.Success.BackgroundColor = "#00ff00";
        config.Success.ClassName = "my-success";
        config.Success.IconClassName = "bi bi-check";

        var success = new NotyfEntity(config).types.Single(t => t.type == "success");

        success.background.ShouldBe("#00ff00");
        success.className.ShouldBe("my-success");
        success.icon!.className.ShouldBe("bi bi-check");
        success.icon.tagName.ShouldBe("i");
    }

    [Fact]
    public void Legacy_constructor_still_works()
    {
        var entity = new NotyfEntity(10, NotyfPosition.TopLeft, false);

        entity.duration.ShouldBe(10000);
        entity.position.x.ShouldBe("left");
        entity.dismissible.ShouldBeFalse();
    }
}

public class NotyfClientOptionsTests
{
    private static JsonElement Options(NotyfConfig config)
        => JsonDocument.Parse(NotyfViewComponent.BuildClientOptions(config).ToJson()).RootElement;

    [Fact]
    public void Defaults()
    {
        var options = Options(new NotyfConfig());

        options.GetProperty("className").GetString().ShouldBe(string.Empty);
        options.GetProperty("autoHandleAjax").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public void Rtl_full_width_and_custom_class_are_combined()
    {
        var options = Options(new NotyfConfig
        {
            IsRtl = true,
            Position = NotyfPosition.BottomFullWidth,
            ClassName = " brand "
        });

        options.GetProperty("className").GetString().ShouldBe("brand notyf__toast--rtl notyf__toast--full-width");
    }

    [Fact]
    public void Ajax_handling_can_be_turned_off()
    {
        Options(new NotyfConfig { AutoHandleAjax = false }).GetProperty("autoHandleAjax").GetBoolean().ShouldBeFalse();
    }
}

public class ToastifyConfigurationTests
{
    [Theory]
    [InlineData(0, 5000)]
    [InlineData(4, 4000)]
    public void Global_duration_defaults_to_five_seconds(int seconds, int expected)
    {
        new ToastifyEntity(seconds).duration.ShouldBe(expected);
    }

    [Fact]
    public void Gravity_and_position_use_toastify_names()
    {
        var entity = new ToastifyEntity(1, Gravity.Top, Position.Left);

        entity.gravity.ShouldBe("top");
        entity.position.ShouldBe("left");
    }

    [Fact]
    public void Client_options_carry_per_type_styles()
    {
        var config = new ToastifyConfig();
        config.Error.BackgroundColor = "crimson";
        config.Error.ClassName = "my-error";

        var options = JsonDocument.Parse(ToastifyViewComponent.BuildClientOptions(config).ToJson()).RootElement;
        var error = options.GetProperty("types").GetProperty("error");

        error.GetProperty("backgroundColor").GetString().ShouldBe("crimson");
        error.GetProperty("className").GetString().ShouldBe("my-error");
        options.GetProperty("types").GetProperty("success").GetProperty("backgroundColor").GetString().ShouldBe("#388e3c");
    }
}
