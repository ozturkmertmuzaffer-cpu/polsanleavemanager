namespace Polsan.Leave.Architecture.Tests;

/// <summary>
/// Rule 4 (no DateTime.Now) is a compile-time ban (BannedApiAnalyzers, RS0030). A violation cannot
/// live in a test project — it would not compile — so these tests pin the wiring instead, and
/// VERIFY demonstrates a temporary violation breaking the build (plan 0001, R-4).
/// </summary>
public sealed class BannedApiConfigurationTests
{
    private static readonly DirectoryInfo RepoRoot = FindRepoRoot();

    public static TheoryData<string> BannedSymbols =>
    [
        "P:System.DateTime.Now",
        "P:System.DateTime.UtcNow",
        "P:System.DateTime.Today",
        "P:System.DateTimeOffset.Now",
        "P:System.DateTimeOffset.UtcNow",
    ];

    [Theory]
    [MemberData(nameof(BannedSymbols))]
    public void BannedSymbolsFile_ListsCurrentTimeApi(string symbol)
    {
        var lines = File.ReadAllLines(Path.Combine(RepoRoot.FullName, "BannedSymbols.txt"));

        lines.ShouldContain(line => line.StartsWith(symbol + ";", StringComparison.Ordinal));
    }

    [Fact]
    public void DirectoryBuildProps_WiresAnalyzerIntoEveryProject()
    {
        var props = File.ReadAllText(Path.Combine(RepoRoot.FullName, "Directory.Build.props"));

        props.ShouldContain("Microsoft.CodeAnalysis.BannedApiAnalyzers");
        props.ShouldContain("BannedSymbols.txt");
        props.ShouldContain("<TreatWarningsAsErrors>true</TreatWarningsAsErrors>");
    }

    [Fact]
    public void NoProject_OptsOutOfDirectoryBuildProps()
    {
        var optingOut = RepoRoot.EnumerateFiles("*.csproj", SearchOption.AllDirectories)
            .Where(f => File.ReadAllText(f.FullName).Contains("ImportDirectoryBuildProps", StringComparison.Ordinal))
            .Select(f => f.Name);

        optingOut.ShouldBeEmpty();
    }

    private static DirectoryInfo FindRepoRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "Polsan.Leave.slnx")))
            {
                return dir;
            }
        }
        throw new InvalidOperationException("Repository root (Polsan.Leave.slnx) not found above " + AppContext.BaseDirectory);
    }
}
