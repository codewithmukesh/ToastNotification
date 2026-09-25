namespace AspNetCoreHero.ToastNotification.E2ETests;

/// <summary>
/// One fresh browser context (cookies, TempData) per test. Fails the test on any JavaScript error.
/// </summary>
public abstract class BrowserTest(BrowserFixture fixture) : IAsyncLifetime
{
    private IBrowserContext? _context;

    protected IPage Page { get; private set; } = null!;
    protected List<string> PageErrors { get; } = [];
    protected List<string> ConsoleErrors { get; } = [];

    public async ValueTask InitializeAsync()
    {
        _context = await fixture.Browser.NewContextAsync(new BrowserNewContextOptions { BaseURL = fixture.BaseUrl });
        Page = await _context.NewPageAsync();
        Page.SetDefaultTimeout(5000);
        Page.PageError += (_, error) => PageErrors.Add(error);
        Page.Console += (_, message) =>
        {
            if (message.Type == "error") ConsoleErrors.Add(message.Text);
        };
    }

    public async ValueTask DisposeAsync()
    {
        PageErrors.ShouldBeEmpty();
        if (_context is not null)
        {
            await _context.DisposeAsync();
        }
    }

    protected ILocator NotyfToast(string text) => Page.Locator(".notyf__toast", new PageLocatorOptions { HasText = text });

    protected ILocator ToastifyToast(string text) => Page.Locator(".toastify", new PageLocatorOptions { HasText = text });

    protected static Task Expect(ILocator locator) => Assertions.Expect(locator).ToBeVisibleAsync();
}
