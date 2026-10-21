using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Polsan.Leave.Web.Health;

/// <summary>
/// Reports whether the SQLite database (ADR 0003) answers a trivial query. Details stay in the logs.
/// The check always opens with Mode=ReadWrite, whatever the configuration says, so a missing database
/// file is reported as Unhealthy and never created here (creation belongs to migrations).
/// </summary>
public sealed class DatabaseHealthCheck(IConfiguration configuration) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var connectionString = configuration.GetConnectionString("LeaveDb");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return HealthCheckResult.Unhealthy("No database connection string is configured.");
        }

        try
        {
            var readWriteOnly = new SqliteConnectionStringBuilder(connectionString) { Mode = SqliteOpenMode.ReadWrite };
            await using var connection = new SqliteConnection(readWriteOnly.ToString());
            await connection.OpenAsync(cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (SqliteException ex)
        {
            return HealthCheckResult.Unhealthy("Database is unreachable.", ex);
        }
    }
}
