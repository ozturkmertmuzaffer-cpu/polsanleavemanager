using System.Net;
using Microsoft.Extensions.Logging;

namespace Polsan.Leave.Web.Tests;

public sealed class ErrorHandlingTests
{
    private static readonly Uri ThrowTrigger = new("/_test/throw", UriKind.Relative);

    // AC-4
    [Fact]
    public async Task Throw_InTesting_Returns500GenericMessageAndErrorIdLogged()
    {
        await using var factory = new TestWebApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync(ThrowTrigger, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        body.ShouldNotContain("InvalidOperationException");
        body.ShouldNotContain("Test-only error trigger");
        body.ShouldNotContain("   at ");

        var document = await Html.ParseAsync(response);
        document.Text("h1").ShouldBe("Bir hata oluştu");
        var errorId = document.Text("code.error-id");
        errorId.ShouldNotBeNullOrWhiteSpace();

        factory.Logs.GetSnapshot().ShouldContain(log =>
            log.Level == LogLevel.Error && log.Message.Contains($"ErrorId={errorId}", StringComparison.Ordinal));
    }

    // AC-4 — the trigger exists only in the Testing environment.
    [Fact]
    public async Task ThrowTrigger_InProduction_Returns404()
    {
        await using var factory = new TestWebApplicationFactory { Environment = "Production" };
        var client = factory.CreateClient();

        var response = await client.GetAsync(ThrowTrigger, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
