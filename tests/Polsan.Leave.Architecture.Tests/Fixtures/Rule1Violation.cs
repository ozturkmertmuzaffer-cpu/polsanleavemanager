// Deliberate violation for RuleSelfTests: breaks exactly one rule. Never referenced by production code.
#pragma warning disable CS0169, CA1812, CA1823, IDE0051, IDE0052 // inspected by ArchUnitNET, never executed

namespace Polsan.Leave.Architecture.Tests.Fixtures.Rule1;

/// <summary>Pretends to be in module Staff and reaches into Policy's Domain.</summary>
internal static class ReachesIntoOtherModule
{
    public static Type Probe() => typeof(Polsan.Leave.Policy.Domain.AssemblyMarker);
}
