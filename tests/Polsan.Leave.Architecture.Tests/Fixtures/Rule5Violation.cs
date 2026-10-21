// Deliberate violation for RuleSelfTests: breaks exactly one rule. Never referenced by production code.
#pragma warning disable CS0169, CA1812, CA1823, IDE0051, IDE0052 // inspected by ArchUnitNET, never executed

namespace Polsan.Leave.Architecture.Tests.Fixtures.Rule5;

/// <summary>A "module" type that makes HTTP calls.</summary>
internal sealed class ModuleWithHttpClient
{
    private readonly System.Net.Http.HttpClient? _client;
}
