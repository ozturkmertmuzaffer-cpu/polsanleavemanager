using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Polsan.Leave.Architecture.Tests;

/// <summary>
/// The forbidden dependencies of docs/architecture.md as reusable rules. Each rule takes the types
/// it governs, so the same rule runs against production code and against the deliberate
/// violations in Fixtures/ (which proves the rule can fail at all).
/// Rule 4 (no DateTime.Now) is enforced by BannedApiAnalyzers at compile time instead.
/// </summary>
internal static class ArchitectureRules
{
    public static readonly string[] Modules = ["Staff", "Policy", "Requests", "Balance", "Audit"];
    public static readonly string[] Layers = ["Contracts", "Domain", "Application", "Infrastructure"];

    /// <summary>Rule 1: a module reaches another module only through its Contracts.</summary>
    public static IArchRule OnlyOtherModulesContracts(GivenTypesConjunction moduleTypes, string module) =>
        moduleTypes.Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching($@"^Polsan\.Leave\.(?!{module}\.)\w+\.(Domain|Application|Infrastructure)(\.|$)")
            .Because("modules talk to each other only through Contracts (docs/architecture.md, rule 1)");

    /// <summary>Rule 2: Domain stays free of EF Core, ASP.NET Core and Blazor.</summary>
    public static IArchRule DomainHasNoInfrastructure(GivenTypesConjunction domainTypes) =>
        domainTypes.Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching(@"^Microsoft\.(EntityFrameworkCore|AspNetCore)(\.|$)")
            .Because("Domain is plain C# (docs/architecture.md, rule 2)");

    /// <summary>Rule 3: Blazor components never touch storage directly.</summary>
    public static IArchRule ComponentsHaveNoStorageAccess(GivenTypesConjunction componentTypes) =>
        componentTypes.Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching(@"^(Microsoft\.EntityFrameworkCore|Microsoft\.Data\.Sqlite|Polsan\.Leave\.\w+\.Infrastructure)(\.|$)")
            .Because("components call Application services only (docs/architecture.md, rule 3)");

    /// <summary>Rule 5 (partial proof, spec 0001 R-7): business modules have no HTTP client.</summary>
    public static IArchRule NoOutboundHttp(GivenTypesConjunction moduleTypes) =>
        moduleTypes.Should().NotDependOnAnyTypesThat()
            .ResideInNamespaceMatching(@"^System\.Net\.Http(\.|$)")
            .Because("the application makes no outbound network calls (docs/architecture.md, rule 5)");

    public static GivenTypesConjunction TypesInNamespace(string namespacePattern) =>
        Types().That().ResideInNamespaceMatching(namespacePattern);
}
