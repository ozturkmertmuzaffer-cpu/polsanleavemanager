// Deliberate violation for RuleSelfTests: breaks exactly one rule. Never referenced by production code.

namespace Polsan.Leave.Architecture.Tests.Fixtures.Rule1;

/// <summary>Pretends to be in module Staff and reaches into Policy's Domain.</summary>
internal static class ReachesIntoOtherModule
{
    public static Type Probe() => typeof(Polsan.Leave.Policy.Domain.AssemblyMarker);
}
