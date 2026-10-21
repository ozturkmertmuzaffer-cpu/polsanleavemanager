namespace Polsan.Leave.Web.Tests;

/// <summary>
/// The real PostgreSQL used by Category=Integration tests. A missing LEAVE_TEST_DB is a failure,
/// never a skip (docs/testing.md, Protected-tests rule).
/// </summary>
internal static class TestDatabase
{
    public static string ConnectionString =>
        Environment.GetEnvironmentVariable("LEAVE_TEST_DB") is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException(
                "LEAVE_TEST_DB is not set. Integration tests need a real PostgreSQL connection string (docs/testing.md).");
}
