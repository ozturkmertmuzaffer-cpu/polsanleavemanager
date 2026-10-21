using System.Reflection;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;
using Microsoft.AspNetCore.Components;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
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

    // Every Blazor component in Web, wherever its folder or namespace (review 0001, L3).
    private static readonly GivenTypesConjunction WebComponents =
        Types().That().ResideInAssembly(WebAssembly).And().ImplementInterface(typeof(IComponent));

    // Guards rule 3 against an empty selection, which would pass without checking anything.
    [Fact]
    public void WebComponents_Selection_FindsKnownComponents()
    {
        var selected = WebComponents.GetObjects(Production).Select(t => t.FullName).ToList();

        string[] known =
        [
            "Polsan.Leave.Web.Components.App",
            "Polsan.Leave.Web.Components.Routes",
            "Polsan.Leave.Web.Components.Layout.MainLayout",
            "Polsan.Leave.Web.Components.Pages.Home",
            "Polsan.Leave.Web.Components.Pages.NotFound",
            "Polsan.Leave.Web.Components.Pages.Error",
        ];
        known.Except(selected).ShouldBeEmpty();
    }

    // AC-7, rule 3
    [Fact]
    public void Components_HaveNoStorageAccess() =>
        ComponentsHaveNoStorageAccess(WebComponents).Check(Production);

    // AC-7, rule 5
    [Fact]
    public void Modules_HaveNoOutboundHttpDependency() =>
        NoOutboundHttp(TypesInNamespace(@"^Polsan\.Leave\.(Staff|Policy|Requests|Balance|Audit)\.")).Check(Production);
}
