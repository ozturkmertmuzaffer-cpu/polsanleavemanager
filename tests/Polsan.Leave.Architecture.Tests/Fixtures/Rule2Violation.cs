// Deliberate violation for RuleSelfTests: breaks exactly one rule. Never referenced by production code.

namespace Polsan.Leave.Architecture.Tests.Fixtures.Rule2;

/// <summary>A "domain" type that depends on ASP.NET Core.</summary>
internal sealed class DomainWithAspNetCore
{
    private readonly Microsoft.AspNetCore.Http.HttpContext? _context;
}
