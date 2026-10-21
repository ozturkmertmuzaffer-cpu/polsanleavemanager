// Deliberate violation for RuleSelfTests: breaks exactly one rule. Never referenced by production code.

namespace Polsan.Leave.Architecture.Tests.Fixtures.Rule5;

/// <summary>A "module" type that makes HTTP calls.</summary>
internal sealed class ModuleWithHttpClient
{
    private readonly System.Net.Http.HttpClient? _client;
}
