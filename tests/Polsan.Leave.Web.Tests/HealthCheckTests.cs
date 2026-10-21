using System.Net;

namespace Polsan.Leave.Web.Tests;

[Trait("Category", "Integration")]
public sealed class HealthCheckTests
{
    private static readonly Uri Health = new("/health", UriKind.Relative);

    // AC-5, AC-6 (body), AC-9
    [Fact]
    public async Task Health_DatabaseReachable_Returns200Healthy()
    {
        await using var factory = new TestWebApplicationFactory { ConnectionString = TestDatabase.ConnectionString };
        var client = factory.CreateClient();

        var response = await client.GetAsync(Health, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldBe("Healthy");
    }

    // AC-6 — nothing listens on port 1, so the database is unreachable.
    [Fact]
    public async Task Health_DatabaseUnreachable_Returns503UnhealthyOnly()
    {
        const string unreachable = "Host=127.0.0.1;Port=1;Database=leave;Username=leave;Password=unused;Timeout=3";
        await using var factory = new TestWebApplicationFactory { ConnectionString = unreachable };
        var client = factory.CreateClient();

        var response = await client.GetAsync(Health, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldBe("Unhealthy");
    }
}
