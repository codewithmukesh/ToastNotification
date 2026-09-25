namespace AspNetCoreHero.ToastNotification.E2ETests;

public class NotyfBrowserTests(BrowserFixture fixture) : BrowserTest(fixture)
{
    [Fact]
    public async Task Shows_every_notification_type_on_page_load()
    {
        await Page.GotoAsync("/Home/NotyfAll");

        await Expect(NotyfToast("Notyf success"));
        await Expect(NotyfToast("Notyf error"));
        await Expect(NotyfToast("Notyf warning"));
        await Expect(NotyfToast("Notyf information"));
        await Expect(NotyfToast("Notyf custom"));
        (await NotyfToast("Notyf custom").GetAttributeAsync("class"))!.ShouldContain("my-custom-class");
    }

    [Fact]
    public async Task Shows_after_a_redirect()
    {
        await Page.GotoAsync("/Home/NotyfRedirect");

        await Expect(NotyfToast("Survived the redirect"));
        Page.Url.ShouldEndWith("/");
    }

    [Fact]
    public async Task Shows_after_a_form_post_without_redirect()
    {
        await Page.GotoAsync("/");
        await Page.ClickAsync("#btn-post");

        await Expect(NotyfToast("Posted without redirect"));
    }

    [Theory]
    [InlineData("#btn-fetch", "Ajax via fetch")]
    [InlineData("#btn-xhr", "Ajax via xhr")]
    [InlineData("#btn-fetch-error", "Ajax failed")]
    public async Task Shows_notifications_from_ajax_calls(string button, string message)
    {
        await Page.GotoAsync("/");
        await Page.ClickAsync(button);

        await Expect(NotyfToast(message));
    }

    [Fact]
    public async Task jQuery_ajax_shows_the_toast_exactly_once()
    {
        await Page.GotoAsync("/");
        await Page.ClickAsync("#btn-jquery");

        await Expect(NotyfToast("Ajax via jquery"));
        await Page.WaitForTimeoutAsync(300);
        (await NotyfToast("Ajax via jquery").CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task Works_with_htmx()
    {
        await Page.GotoAsync("/");
        await Page.ClickAsync("#btn-htmx");

        await Expect(Page.Locator("#htmx-result"));
        await Expect(NotyfToast("Hello htmx"));
    }

    [Fact]
    public async Task Works_without_jQuery()
    {
        // Issue #13: v1 crashed with "$ is not defined" when jQuery loaded later or not at all.
        await Page.GotoAsync("/Home/NoJquery");

        await Expect(NotyfToast("Works without jQuery"));
        (await Page.EvaluateAsync<bool>("typeof window.jQuery === 'undefined'")).ShouldBeTrue();
    }

    [Fact]
    public async Task Regular_toasts_close_on_their_own()
    {
        await Page.GotoAsync("/Home/NotyfRedirect");
        await Expect(NotyfToast("Survived the redirect"));

        await Assertions.Expect(NotyfToast("Survived the redirect")).ToBeHiddenAsync(new() { Timeout = 6000 });
    }

    [Fact]
    public async Task Sticky_toasts_stay_until_closed()
    {
        // Issue #15.
        await Page.GotoAsync("/Home/NotyfSticky");
        var toast = NotyfToast("I am sticky");
        await Expect(toast);

        await Page.WaitForTimeoutAsync(3000); // longer than the 2s global duration
        await Expect(toast);

        // Issue #11: the close button is labelled for screen readers.
        var close = toast.GetByRole(AriaRole.Button, new() { Name = "Close notification" });
        await close.ClickAsync();
        await Assertions.Expect(toast).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Messages_cannot_break_out_of_the_script_context()
    {
        await Page.GotoAsync("/Home/NotyfXss");

        await Expect(Page.Locator(".notyf__toast"));
        (await Page.EvaluateAsync<bool>("window.__xss === true")).ShouldBeFalse();
    }

    [Fact]
    public async Task Quotes_inside_a_loop_render_every_toast()
    {
        // Issue #4.
        await Page.GotoAsync("/Home/NotyfLoop");

        await Expect(NotyfToast("O'Brien is invalid"));
        await Expect(NotyfToast("D'Angelo is invalid"));
        await Expect(NotyfToast("N'Golo is invalid"));
    }

    [Fact]
    public async Task Works_under_a_strict_content_security_policy()
    {
        await Page.GotoAsync("/Home/NotyfAll?csp=1");

        await Expect(NotyfToast("Notyf success"));
        ConsoleErrors.ShouldNotContain(e => e.Contains("Content Security Policy"));
    }

    [Fact]
    public async Task Ajax_works_under_a_strict_content_security_policy()
    {
        await Page.GotoAsync("/?csp=1");
        await Page.ClickAsync("#btn-fetch");

        await Expect(NotyfToast("Ajax via fetch"));
        ConsoleErrors.ShouldNotContain(e => e.Contains("Content Security Policy"));
    }

    [Fact]
    public async Task Ignores_toast_headers_from_other_origins()
    {
        // Audit finding (security): a CORS API must not be able to inject HTML toasts into the page.
        await Page.GotoAsync("/");
        await Page.ClickAsync("#btn-cross-origin");
        await Assertions.Expect(Page.Locator("body[data-cross-done]")).ToHaveCountAsync(1);
        await Page.WaitForTimeoutAsync(300);

        (await Page.Locator(".notyf__toast").CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task Ajax_request_that_redirects_shows_the_toast()
    {
        await Page.GotoAsync("/");
        await Page.ClickAsync("#btn-ajax-redirect");

        await Expect(NotyfToast("Survived an ajax redirect"));
    }

    [Fact]
    public async Task Works_with_htmx_boosted_navigation()
    {
        // hx-boost swaps the <body>, which used to detach Notyf's container.
        await Page.GotoAsync("/");
        await Page.ClickAsync("#lnk-boost");

        await Expect(NotyfToast("Survived the redirect"));
    }

    [Fact]
    public async Task Announces_toasts_to_screen_readers()
    {
        await Page.GotoAsync("/Home/NotyfRedirect");

        await Assertions.Expect(Page.Locator(".notyf-announcer[aria-live='polite']")).ToHaveTextAsync("Survived the redirect");
    }
}
