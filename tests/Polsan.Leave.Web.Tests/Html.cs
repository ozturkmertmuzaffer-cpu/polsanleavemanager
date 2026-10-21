using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Polsan.Leave.Web.Tests;

/// <summary>Parses a server response the way a browser without JavaScript would see it.</summary>
internal static class Html
{
    public static async Task<IDocument> ParseAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        return await new HtmlParser().ParseDocumentAsync(body, TestContext.Current.CancellationToken);
    }

    public static string Text(this IDocument document, string selector) =>
        document.QuerySelector(selector)?.TextContent.Trim() ?? string.Empty;
}
