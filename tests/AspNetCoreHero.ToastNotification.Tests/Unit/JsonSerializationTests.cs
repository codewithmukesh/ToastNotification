using AspNetCoreHero.ToastNotification.Enums;
using AspNetCoreHero.ToastNotification.Helpers;
using AspNetCoreHero.ToastNotification.Notyf.Models;

namespace AspNetCoreHero.ToastNotification.Tests.Unit;

public class JsonSerializationTests
{
    [Fact]
    public void Output_is_safe_inside_a_script_block()
    {
        // Issue #4 and XSS: quotes and </script> must never break out of the JSON block.
        var json = new { message = "</script><script>alert('x')</script> & \"quoted\"" }.ToJson();

        json.ShouldNotContain("</script>");
        json.ShouldNotContain("<");
        json.ShouldNotContain("'");
        json.ShouldContain("\\u003C");
    }

    [Fact]
    public void Uses_camelCase_and_skips_nulls()
    {
        var json = new NotyfNotification(ToastNotificationType.Error, "m", null).ToJson();

        json.ShouldContain("\"message\":\"m\"");
        json.ShouldContain("\"type\":1");
        json.ShouldNotContain("duration");
        json.ShouldNotContain("backgroundColor");
    }

    [Fact]
    public void Keeps_zero_duration_for_sticky_toasts()
    {
        new NotyfNotification(ToastNotificationType.Success, "m", 0).ToJson().ShouldContain("\"duration\":0");
    }

    [Fact]
    public void Reads_what_it_writes_case_insensitively()
    {
        var back = "{\"Message\":\"hi\",\"Type\":2,\"Duration\":3000,\"Icon\":\"x\"}".FromJson<NotyfNotification>();

        back.ShouldNotBeNull();
        back.Message.ShouldBe("hi");
        back.Type.ShouldBe(ToastNotificationType.Warning);
        back.Duration.ShouldBe(3000);
        back.Icon.ShouldBe("x");
    }
}
