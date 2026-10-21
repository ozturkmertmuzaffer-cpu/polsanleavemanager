using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;

namespace Polsan.Leave.Web.Health;

/// <summary>Reports whether PostgreSQL answers a trivial query. Details stay in the logs.</summary>
public sealed class DatabaseHealthCheck(NpgsqlDataSource? dataSource = null) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (dataSource is null)
        {
            return HealthCheckResult.Unhealthy("No database connection string is configured.");
        }

        try
        {
            await using var command = dataSource.CreateCommand("SELECT 1");
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (NpgsqlException ex)
        {
            return HealthCheckResult.Unhealthy("Database is unreachable.", ex);
        }
        catch (TimeoutException ex)
        {
            return HealthCheckResult.Unhealthy("Database is unreachable.", ex);
        }
    }
}
