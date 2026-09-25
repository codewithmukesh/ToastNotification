using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AspNetCoreHero.ToastNotification.Tests.Integration;

public class ToastNotificationIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private HttpClient CreateClient(bool followRedirects = true) => factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = followRedirects,
        HandleCookies = true
    });

    private static JsonElement ReadClientData(string html, string id)
    {
        var match = Regex.Match(html, $"<script type=\"application/json\" id=\"{id}\">(.*?)</script>", RegexOptions.Singleline);
        match.Success.ShouldBeTrue($"data block #{id} not found");
        return JsonDocument.Parse(match.Groups[1].Value).RootElement;
    }

    private static string[] Messages(string html, string id = "aspnetcorehero-notyf-data")
        => ReadClientData(html, id).GetProperty("notifications").EnumerateArray()
            .Select(n => n.GetProperty("message").GetString()!).ToArray();

    [Fact]
    public async Task Page_renders_all_notification_types()
    {
        var html = await CreateClient().GetStringAsync("/Home/NotyfAll", Ct);

        Messages(html).ShouldBe(["Notyf success", "Notyf error", "Notyf warning", "Notyf information", "Notyf custom"]);
        var custom = ReadClientData(html, "aspnetcorehero-notyf-data").GetProperty("notifications")[4];
        custom.GetProperty("backgroundColor").GetString().ShouldBe("#4f46e5");
        custom.GetProperty("icon").GetString().ShouldBe("fa fa-gear");
        custom.GetProperty("className").GetString().ShouldBe("my-custom-class");
    }

    [Fact]
    public async Task Page_renders_notyf_configuration()
    {
        var html = await CreateClient().GetStringAsync("/Home/Index", Ct);
        var config = ReadClientData(html, "aspnetcorehero-notyf-data").GetProperty("config");

        config.GetProperty("duration").GetInt32().ShouldBe(2000);
        config.GetProperty("dismissible").GetBoolean().ShouldBeTrue();
        config.GetProperty("types").GetArrayLength().ShouldBe(5);
    }

    [Fact]
    public async Task No_inline_javascript_is_rendered()
    {
        // CSP: every <script> is either external (src=) or a JSON data block.
        var html = await CreateClient().GetStringAsync("/Home/NotyfAll", Ct);

        var inlineScripts = Regex.Matches(html, "<script(?![^>]*\\bsrc=)(?![^>]*type=\"application/json\")[^>]*>");
        inlineScripts.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Assets_are_cache_busted()
    {
        var html = await CreateClient().GetStringAsync("/Home/Index", Ct);

        html.ShouldMatch("_content/AspNetCoreHero.ToastNotification/notyf.aspnetcore.js\\?v=");
        html.ShouldMatch("_content/AspNetCoreHero.ToastNotification/notyf.min.css\\?v=");
    }

    [Theory]
    [InlineData("notyf.min.js")]
    [InlineData("notyf.min.css")]
    [InlineData("notyf.aspnetcore.js")]
    [InlineData("toastify.js")]
    [InlineData("toastify.css")]
    [InlineData("toastify.aspnetcore.js")]
    [InlineData("toastnotification.ajax.js")]
    public async Task Static_assets_are_served(string file)
    {
        var response = await CreateClient().GetAsync($"/_content/AspNetCoreHero.ToastNotification/{file}", Ct);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Font_awesome_can_be_turned_off()
    {
        var html = await CreateClient().GetStringAsync("/Home/Index", Ct);

        html.ShouldNotContain("font-awesome");
    }

    [Fact]
    public async Task Notification_survives_a_redirect_and_shows_once()
    {
        var client = CreateClient(followRedirects: false);

        var redirect = await client.GetAsync("/Home/NotyfRedirect", Ct);
        redirect.StatusCode.ShouldBe(HttpStatusCode.Redirect);

        var first = await client.GetStringAsync(redirect.Headers.Location!.ToString(), Ct);
        Messages(first).ShouldBe(["Survived the redirect"]);

        var second = await client.GetStringAsync("/Home/Index", Ct);
        Messages(second).ShouldBeEmpty();
    }

    [Fact]
    public async Task Form_post_without_redirect_shows_the_notification()
    {
        // Issue #8.
        var response = await CreateClient().PostAsync("/Home/NotyfPost", new FormUrlEncodedContent([]), Ct);

        Messages(await response.Content.ReadAsStringAsync(Ct)).ShouldBe(["Posted without redirect"]);
    }

    [Theory]
    [InlineData("X-Requested-With", "XMLHttpRequest")]
    [InlineData("HX-Request", "true")]
    public async Task Ajax_and_htmx_requests_get_notifications_in_a_header(string header, string value)
    {
        var client = CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/Home/NotyfAjax?client=test");
        request.Headers.Add(header, value);

        var response = await client.SendAsync(request, Ct);

        response.Headers.TryGetValues("X-Notyf-Notifications", out var values).ShouldBeTrue();
        var json = JsonDocument.Parse(Uri.UnescapeDataString(values!.Single())).RootElement;
        json[0].GetProperty("message").GetString().ShouldBe("Ajax via test");
        response.Headers.GetValues("Access-Control-Expose-Headers").ShouldContain("X-Notyf-Notifications");

        // Not stored in TempData, so it doesn't show again on the next page.
        Messages(await client.GetStringAsync("/Home/Index", Ct)).ShouldBeEmpty();
    }

    [Fact]
    public async Task Error_responses_carry_notifications_too()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Home/NotyfAjaxError");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");

        var response = await CreateClient().SendAsync(request, Ct);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Headers.Contains("X-Notyf-Notifications").ShouldBeTrue();
    }

    [Fact]
    public async Task Non_ajax_call_to_an_ajax_endpoint_keeps_the_notification_for_the_next_page()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/Home/NotyfAjax?client=plain", Ct);
        response.Headers.Contains("X-Notyf-Notifications").ShouldBeFalse();

        Messages(await client.GetStringAsync("/Home/Index", Ct)).ShouldBe(["Ajax via plain"]);
    }

    [Fact]
    public async Task Script_breaking_messages_are_escaped()
    {
        // Issue #4 and XSS.
        var html = await CreateClient().GetStringAsync("/Home/NotyfXss", Ct);

        var dataBlock = Regex.Match(html, "id=\"aspnetcorehero-notyf-data\">(.*?)</script>", RegexOptions.Singleline).Groups[1].Value;
        dataBlock.ShouldNotContain("<");
        Messages(html).Single().ShouldBe(AspNetCoreHero.ToastNotification.TestApp.Controllers.HomeController.XssMessage);
    }

    [Fact]
    public async Task Quotes_in_a_loop_do_not_break_rendering()
    {
        // Issue #4.
        var html = await CreateClient().GetStringAsync("/Home/NotyfLoop", Ct);

        Messages(html).ShouldBe(["O'Brien is invalid", "D'Angelo is invalid", "N'Golo is invalid"]);
    }

    [Fact]
    public async Task Sticky_notification_has_zero_duration()
    {
        var html = await CreateClient().GetStringAsync("/Home/NotyfSticky", Ct);

        ReadClientData(html, "aspnetcorehero-notyf-data").GetProperty("notifications")[0]
            .GetProperty("duration").GetInt32().ShouldBe(0);
    }

    [Fact]
    public async Task Nonce_is_added_to_every_script_tag()
    {
        var html = await CreateClient().GetStringAsync("/Home/Nonce", Ct);

        var executable = Regex.Matches(html, "<script[^>]*_content/AspNetCoreHero[^>]*>");
        executable.Count.ShouldBe(6);
        executable.ShouldAllBe(m => m.Value.Contains("nonce=\"test-nonce-123\""));
    }

    [Fact]
    public async Task No_nonce_attribute_when_none_is_given()
    {
        var html = await CreateClient().GetStringAsync("/Home/Index", Ct);

        html.ShouldNotContain("nonce=");
    }

    [Fact]
    public async Task Ajax_request_that_redirects_still_delivers_the_toast()
    {
        // Audit finding: fetch/XHR follow redirects silently, so the 302 header was never seen.
        var request = new HttpRequestMessage(HttpMethod.Get, "/Home/NotyfAjaxRedirect");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");

        var response = await CreateClient().SendAsync(request, Ct);

        response.RequestMessage!.RequestUri!.AbsolutePath.ShouldBe("/Home/AjaxTarget");
        response.Headers.TryGetValues("X-Notyf-Notifications", out var values).ShouldBeTrue();
        Uri.UnescapeDataString(values!.Single()).ShouldContain("Survived an ajax redirect");
    }

    [Fact]
    public async Task Header_is_capped_and_the_overflow_waits_for_the_next_page()
    {
        var client = CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "/Home/NotyfMany");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");

        var response = await client.SendAsync(request, Ct);

        var header = response.Headers.GetValues("X-Notyf-Notifications").Single();
        header.Length.ShouldBeLessThanOrEqualTo(4096);
        var inHeader = JsonDocument.Parse(Uri.UnescapeDataString(header)).RootElement.GetArrayLength();
        inHeader.ShouldBeGreaterThan(0);

        var nextPage = Messages(await client.GetStringAsync("/Home/Index", Ct));
        (inHeader + nextPage.Length).ShouldBe(40);
    }

    [Fact]
    public async Task Html_in_ajax_messages_does_not_bloat_the_header()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Home/NotyfHtmlAjax");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");

        var response = await CreateClient().SendAsync(request, Ct);

        var header = response.Headers.GetValues("X-Notyf-Notifications").Single();
        header.ShouldNotContain("%5Cu003C"); // no double escaping of <
        Uri.UnescapeDataString(header).ShouldContain("</script>");
    }

    [Fact]
    public async Task Toastify_page_renders_all_types()
    {
        var html = await CreateClient().GetStringAsync("/Home/ToastifyAll", Ct);

        Messages(html, "aspnetcorehero-toastify-data").ShouldBe(
            ["Toastify success", "Toastify error", "Toastify warning", "Toastify information", "Toastify custom"]);
    }

    [Fact]
    public async Task Toastify_survives_a_redirect()
    {
        var html = await CreateClient().GetStringAsync("/Home/ToastifyRedirect", Ct);

        Messages(html, "aspnetcorehero-toastify-data").ShouldBe(["Toastify survived the redirect"]);
    }

    [Fact]
    public async Task Toastify_ajax_uses_its_own_header()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/Home/ToastifyAjax");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");

        var response = await CreateClient().SendAsync(request, Ct);

        response.Headers.Contains("X-Toastify-Notifications").ShouldBeTrue();
        response.Headers.Contains("X-Notyf-Notifications").ShouldBeFalse();
    }

    [Fact]
    public async Task Notyf_and_Toastify_notifications_never_mix()
    {
        var html = await CreateClient().GetStringAsync("/Home/Both", Ct);

        Messages(html).ShouldBe(["From Notyf"]);
        Messages(html, "aspnetcorehero-toastify-data").ShouldBe(["From Toastify"]);
    }
}
