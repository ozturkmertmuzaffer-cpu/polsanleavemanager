using Microsoft.Playwright;

namespace Polsan.Leave.E2E.Tests;

[Trait("Category", "E2E")]
public sealed class SmokeTests(AppFixture app) : IClassFixture<AppFixture>
{
    private const string AppTitle = "Polsan İzin Yönetimi";

    // AC-10
    [Fact]
    public async Task Home_OpensInHeadlessBrowser_TitleVisible()
    {
        var page = await app.Browser.NewPageAsync();
        try
        {
            await page.GotoAsync(app.BaseAddress.ToString());

            (await page.TitleAsync()).ShouldBe(AppTitle);
            (await page.Locator("h1").InnerTextAsync()).ShouldBe(AppTitle);

            // Styles are served: otherwise the 360 px check below would pass on unstyled markup.
            var headerBackground = await page.Locator("header.app-header")
                .EvaluateAsync<string>("e => getComputedStyle(e).backgroundColor");
            headerBackground.ShouldBe("rgb(18, 53, 91)");
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    // AC-14
    [Theory]
    [InlineData("/")]
    [InlineData("/olmayan-sayfa")]
    public async Task Pages_At360px_HaveNoHorizontalScroll(string path)
    {
        await using var context = await app.Browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 360, Height = 740 },
        });
        var page = await context.NewPageAsync();

        await page.GotoAsync(new Uri(app.BaseAddress, path).ToString());

        var overflows = await page.EvaluateAsync<bool>(
            "() => document.documentElement.scrollWidth > document.documentElement.clientWidth");
        overflows.ShouldBeFalse();
    }
}
