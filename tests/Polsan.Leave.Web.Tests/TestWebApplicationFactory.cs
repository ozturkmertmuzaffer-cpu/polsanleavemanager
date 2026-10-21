using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace Polsan.Leave.Web.Tests;

/// <summary>
/// In-memory host for HTTP-level tests. Environment "Testing" unless told otherwise; the database
/// connection string is whatever the test passes (none by default).
/// </summary>
public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    public string Environment { get; init; } = "Testing";

    public string? ConnectionString { get; init; }

    public FakeLogCollector Logs => Services.GetRequiredService<FakeLogCollector>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.UseEnvironment(Environment);
        if (ConnectionString is not null)
        {
            builder.UseSetting("ConnectionStrings:LeaveDb", ConnectionString);
        }
        builder.ConfigureLogging(logging => logging.AddFakeLogging());
    }
}
