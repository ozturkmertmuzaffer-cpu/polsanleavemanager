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
        var database = TestDatabase.Create();
        using (database)
        {
            await using var factory = new TestWebApplicationFactory { ConnectionString = database.ConnectionString };
            var client = factory.CreateClient();

            var response = await client.GetAsync(Health, TestContext.Current.CancellationToken);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldBe("Healthy");
        }

        File.Exists(database.FilePath).ShouldBeFalse();
    }

    // AC-6 — the database file sits in a directory that does not exist, so it cannot be opened.
    [Fact]
    public async Task Health_DatabaseUnreachable_Returns503UnhealthyOnly()
    {
        var missing = Path.Combine(Path.GetTempPath(), "polsan-leave-tests", Guid.NewGuid().ToString("N"), "leave.db");
        await using var factory = new TestWebApplicationFactory { ConnectionString = $"Data Source={missing};Mode=ReadWrite" };
        var client = factory.CreateClient();

        var response = await client.GetAsync(Health, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldBe("Unhealthy");
    }
}
