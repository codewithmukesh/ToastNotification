using Microsoft.AspNetCore.Mvc.Testing;

[assembly: AssemblyFixture(typeof(AspNetCoreHero.ToastNotification.E2ETests.BrowserFixture))]

namespace AspNetCoreHero.ToastNotification.E2ETests;

/// <summary>
/// Starts the test app on a real Kestrel port and one headless Chromium for the whole test run.
/// </summary>
public sealed class BrowserFixture : IAsyncLifetime
{
    private WebApplicationFactory<Program>? _factory;
    private IPlaywright? _playwright;

    public IBrowser Browser { get; private set; } = null!;
    public string BaseUrl { get; private set; } = string.Empty;

    public async ValueTask InitializeAsync()
    {
        _factory = new WebApplicationFactory<Program>();
        _factory.UseKestrel(0);
        _factory.StartServer();
        BaseUrl = _factory.ClientOptions.BaseAddress.ToString().TrimEnd('/');

        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
    }

    public async ValueTask DisposeAsync()
    {
        await Browser.DisposeAsync();
        _playwright?.Dispose();
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }
}
