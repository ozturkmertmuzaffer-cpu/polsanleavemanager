using System.Reflection;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;
using static Polsan.Leave.Architecture.Tests.ArchitectureRules;
using ArchModel = ArchUnitNET.Domain.Architecture;

namespace Polsan.Leave.Architecture.Tests;

public sealed class ProductionAssembliesTests
{
    private static readonly Assembly[] ModuleAssemblies =
        [.. Modules.SelectMany(m => Layers.Select(l => Assembly.Load($"Polsan.Leave.{m}.{l}")))];

    private static readonly Assembly WebAssembly = typeof(Program).Assembly;

    private static readonly ArchModel Production =
        new ArchLoader().LoadAssemblies([.. ModuleAssemblies, WebAssembly]).Build();

    public static TheoryData<string> ModuleNames => [.. Modules];

    // AC-8
    [Fact]
    public void AllModuleAssemblies_Exist()
    {
        ModuleAssemblies.Select(a => a.GetName().Name).ShouldBe(
            Modules.SelectMany(m => Layers.Select(l => $"Polsan.Leave.{m}.{l}")),
            ignoreOrder: true);
        WebAssembly.GetName().Name.ShouldBe("Polsan.Leave.Web");
    }

    // AC-7, rule 1
    [Theory]
    [MemberData(nameof(ModuleNames))]
    public void Module_DependsOnOtherModules_OnlyThroughContracts(string module) =>
        OnlyOtherModulesContracts(TypesInNamespace($@"^Polsan\.Leave\.{module}\."), module).Check(Production);

    // AC-7, rule 2
    [Fact]
    public void Domain_HasNoEfCoreOrAspNetCoreDependency() =>
        DomainHasNoInfrastructure(TypesInNamespace(@"^Polsan\.Leave\.\w+\.Domain(\.|$)")).Check(Production);

    // AC-7, rule 3
    [Fact]
    public void Components_HaveNoStorageAccess() =>
        ComponentsHaveNoStorageAccess(TypesInNamespace(@"^Polsan\.Leave\.Web\.Components(\.|$)")).Check(Production);

    // AC-7, rule 5
    [Fact]
    public void Modules_HaveNoOutboundHttpDependency() =>
        NoOutboundHttp(TypesInNamespace(@"^Polsan\.Leave\.(Staff|Policy|Requests|Balance|Audit)\.")).Check(Production);
}
