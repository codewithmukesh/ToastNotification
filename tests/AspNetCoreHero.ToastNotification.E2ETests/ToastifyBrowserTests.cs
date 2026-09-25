namespace AspNetCoreHero.ToastNotification.E2ETests;

public class ToastifyBrowserTests(BrowserFixture fixture) : BrowserTest(fixture)
{
    [Fact]
    public async Task Shows_every_notification_type_on_page_load()
    {
        await Page.GotoAsync("/Home/ToastifyAll");

        await Expect(ToastifyToast("Toastify success"));
        await Expect(ToastifyToast("Toastify error"));
        await Expect(ToastifyToast("Toastify warning"));
        await Expect(ToastifyToast("Toastify information"));
        await Expect(ToastifyToast("Toastify custom"));
        (await ToastifyToast("Toastify custom").GetAttributeAsync("class"))!.ShouldContain("my-custom-class");
    }

    [Fact]
    public async Task Shows_after_a_redirect()
    {
        await Page.GotoAsync("/Home/ToastifyRedirect");

        await Expect(ToastifyToast("Toastify survived the redirect"));
    }

    [Fact]
    public async Task Shows_notifications_from_fetch()
    {
        // Issue #2: Toastify had no AJAX support in v1.
        await Page.GotoAsync("/");
        await Page.ClickAsync("#btn-toastify-fetch");

        await Expect(ToastifyToast("Toastify ajax"));
    }

    [Fact]
    public async Task Sticky_toasts_get_an_accessible_close_button()
    {
        await Page.GotoAsync("/Home/ToastifySticky");
        var toast = ToastifyToast("Toastify sticky");
        await Expect(toast);

        await Page.WaitForTimeoutAsync(3000);
        await Expect(toast);

        await toast.GetByRole(AriaRole.Button, new() { Name = "Close notification" }).ClickAsync();
        await Assertions.Expect(toast).ToBeHiddenAsync();
    }

    [Fact]
    public async Task Notyf_and_Toastify_work_side_by_side()
    {
        await Page.GotoAsync("/Home/Both");

        await Expect(NotyfToast("From Notyf"));
        await Expect(ToastifyToast("From Toastify"));
        (await Page.Locator(".notyf__toast").CountAsync()).ShouldBe(1);
        (await Page.Locator(".toastify").CountAsync()).ShouldBe(1);
    }
}
