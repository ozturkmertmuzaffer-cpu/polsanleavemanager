// Deliberate violation for RuleSelfTests: breaks exactly one rule. Never referenced by production code.

namespace Polsan.Leave.Architecture.Tests.Fixtures.Rule3;

/// <summary>A "component" that reaches into a module's Infrastructure.</summary>
internal static class ComponentWithInfrastructure
{
    public static Type Probe() => typeof(Polsan.Leave.Staff.Infrastructure.AssemblyMarker);
}
