using Microsoft.Data.Sqlite;

namespace Polsan.Leave.Web.Tests;

/// <summary>
/// A real SQLite database in its own temporary file (ADR 0003) for Category=Integration tests.
/// Needs no installation or environment variable; the file is deleted on dispose.
/// </summary>
internal sealed class TestDatabase : IDisposable
{
    private TestDatabase(string filePath)
    {
        FilePath = filePath;
        ConnectionString = new SqliteConnectionStringBuilder
        {
            DataSource = filePath,
            Mode = SqliteOpenMode.ReadWrite,
        }.ToString();
    }

    public string FilePath { get; }

    public string ConnectionString { get; }

    public static TestDatabase Create()
    {
        var directory = Path.Combine(Path.GetTempPath(), "polsan-leave-tests");
        Directory.CreateDirectory(directory);
        var filePath = Path.Combine(directory, $"{Guid.NewGuid():N}.db");

        var create = new SqliteConnectionStringBuilder { DataSource = filePath, Mode = SqliteOpenMode.ReadWriteCreate };
        using (var connection = new SqliteConnection(create.ToString()))
        {
            connection.Open();
        }
        return new TestDatabase(filePath);
    }

    public void Dispose()
    {
        // The connection pool keeps the file handle open; release it before deleting (plan 0001, D-R1).
        SqliteConnection.ClearAllPools();
        File.Delete(FilePath);
    }
}
