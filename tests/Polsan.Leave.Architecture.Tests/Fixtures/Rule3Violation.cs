// Deliberate violation for RuleSelfTests: breaks exactly one rule. Never referenced by production code.
#pragma warning disable CS0169, CA1812, CA1823, IDE0051, IDE0052 // inspected by ArchUnitNET, never executed

namespace Polsan.Leave.Architecture.Tests.Fixtures.Rule3;

/// <summary>A "component" that reaches into a module's Infrastructure.</summary>
internal static class ComponentWithInfrastructure
{
    public static Type Probe() => typeof(Polsan.Leave.Staff.Infrastructure.AssemblyMarker);
}
