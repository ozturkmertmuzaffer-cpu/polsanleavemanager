using System.Net;

namespace Polsan.Leave.Web.Tests;

public sealed class PagesTests : IClassFixture<TestWebApplicationFactory>
{
    private const string AppTitle = "Polsan İzin Yönetimi";
    private readonly HttpClient _client;

    public PagesTests(TestWebApplicationFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _client = factory.CreateClient();
    }

    // AC-1
    [Fact]
    public async Task Home_Get_ReturnsTurkishTitleAndLangTr()
    {
        var response = await _client.GetAsync(new Uri("/", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var document = await Html.ParseAsync(response);
        document.DocumentElement.GetAttribute("lang").ShouldBe("tr");
        document.Title.ShouldBe(AppTitle);
        document.Text("h1").ShouldBe(AppTitle);
    }

    // AC-2 — at page level: the home page is rendered inside the main layout, menu empty.
    [Fact]
    public async Task Home_Get_UsesMainLayoutWithEmptyMenu()
    {
        var response = await _client.GetAsync(new Uri("/", UriKind.Relative), TestContext.Current.CancellationToken);

        var document = await Html.ParseAsync(response);
        document.Text("header .brand").ShouldBe(AppTitle);
        var menu = document.QuerySelector("nav.app-menu");
        menu.ShouldNotBeNull();
        menu.Children.ShouldBeEmpty();
        document.QuerySelector("main.app-content h1").ShouldNotBeNull();
    }

    // AC-15 — the parsed document is the raw server response: no JavaScript has run.
    [Fact]
    public async Task Home_Get_InitialHtmlContainsTitleAndText()
    {
        var response = await _client.GetAsync(new Uri("/", UriKind.Relative), TestContext.Current.CancellationToken);

        var document = await Html.ParseAsync(response);
        document.Text("main h1").ShouldBe(AppTitle);
        document.Text("main p").ShouldStartWith("Polsan çalışanlarının izin talepleri");
    }

    // AC-3
    [Fact]
    public async Task UnknownPath_Get_Returns404WithTurkishMessageAndHomeLink()
    {
        var response = await _client.GetAsync(new Uri("/olmayan-sayfa", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var document = await Html.ParseAsync(response);
        document.Text("h1").ShouldBe("Sayfa bulunamadı");
        var homeLink = document.QuerySelector("main a[href='/']");
        homeLink.ShouldNotBeNull();
        homeLink.TextContent.Trim().ShouldBe("Açılış sayfasına dön");
    }
}
