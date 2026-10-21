using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Polsan.Leave.Web.Components.Layout;

namespace Polsan.Leave.Web.Tests;

public sealed class LayoutComponentTests : BunitContext
{
    public LayoutComponentTests() => Services.AddLocalization();

    // AC-2
    [Fact]
    public void MainLayout_Render_HasHeaderEmptyMenuAndBody()
    {
        RenderFragment body = builder => builder.AddMarkupContent(0, "<p id=\"page-body\">içerik</p>");

        var layout = Render<MainLayout>(parameters => parameters.Add(p => p.Body, body));

        layout.Find("header .brand").TextContent.Trim().ShouldBe("Polsan İzin Yönetimi");
        layout.Find("nav.app-menu").Children.ShouldBeEmpty();
        layout.Find("main #page-body").TextContent.ShouldBe("içerik");
    }
}
