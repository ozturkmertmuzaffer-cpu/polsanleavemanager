using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace Polsan.Leave.E2E.Tests;

/// <summary>
/// Owns the app-under-test lifecycle (docs/testing.md, Resource hygiene): starts the app on a real
/// Kestrel port and one headless Chromium, and stops both on dispose — also when start-up fails.
/// </summary>
public sealed class AppFixture : IAsyncLifetime
{
    private readonly TestingFactory _factory = new();
    private IPlaywright? _playwright;
    private IBrowser? _browser;

    public Uri BaseAddress { get; private set; } = null!;

    public IBrowser Browser => _browser ?? throw new InvalidOperationException("Fixture not initialized.");

    public async ValueTask InitializeAsync()
    {
        try
        {
            _factory.UseKestrel(0);
            _factory.StartServer();
            var addresses = _factory.Services.GetRequiredService<IServer>().Features
                .Get<IServerAddressesFeature>()?.Addresses
                ?? throw new InvalidOperationException("Kestrel did not report a listening address.");
            BaseAddress = new Uri(addresses.First());

            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        }
        catch
        {
            await DisposeAsync();
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }
        _playwright?.Dispose();
        _playwright = null;
        await _factory.DisposeAsync();
    }

    private sealed class TestingFactory : WebApplicationFactory<Program>
    {
        // Static web assets (app.css, scoped CSS) are wired automatically only in Development.
        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            builder.UseEnvironment("Testing").UseStaticWebAssets();
    }
}
