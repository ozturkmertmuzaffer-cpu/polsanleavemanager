using ArchUnitNET.Loader;
using static Polsan.Leave.Architecture.Tests.ArchitectureRules;
using ArchModel = ArchUnitNET.Domain.Architecture;

namespace Polsan.Leave.Architecture.Tests;

/// <summary>
/// Proves each rule can fail: it must reject the deliberate violation in Fixtures/ (AC-7).
/// A rule that passes here is broken, however green the production run looks.
/// </summary>
public sealed class RuleSelfTests
{
    private const string Fixtures = @"^Polsan\.Leave\.Architecture\.Tests\.Fixtures\.";

    private static readonly ArchModel FixtureArchitecture =
        new ArchLoader().LoadAssemblies(typeof(RuleSelfTests).Assembly).Build();

    [Fact]
    public void Rule1_ModuleReachingIntoOtherModulesDomain_IsRejected() =>
        OnlyOtherModulesContracts(TypesInNamespace(Fixtures + "Rule1"), "Staff")
            .HasNoViolations(FixtureArchitecture).ShouldBeFalse();

    [Fact]
    public void Rule2_DomainDependingOnAspNetCore_IsRejected() =>
        DomainHasNoInfrastructure(TypesInNamespace(Fixtures + "Rule2"))
            .HasNoViolations(FixtureArchitecture).ShouldBeFalse();

    [Fact]
    public void Rule3_ComponentReachingIntoInfrastructure_IsRejected() =>
        ComponentsHaveNoStorageAccess(TypesInNamespace(Fixtures + "Rule3"))
            .HasNoViolations(FixtureArchitecture).ShouldBeFalse();

    [Fact]
    public void Rule5_ModuleUsingHttpClient_IsRejected() =>
        NoOutboundHttp(TypesInNamespace(Fixtures + "Rule5"))
            .HasNoViolations(FixtureArchitecture).ShouldBeFalse();

    // Guards the self-tests themselves: a rule over types that break nothing must pass.
    [Fact]
    public void Rule1_ModuleUsingOnlyItsOwnLayers_IsAccepted() =>
        OnlyOtherModulesContracts(TypesInNamespace(Fixtures + "Rule1"), "Policy")
            .HasNoViolations(FixtureArchitecture).ShouldBeTrue();
}
