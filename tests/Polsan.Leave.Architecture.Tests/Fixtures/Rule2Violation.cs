// Deliberate violation for RuleSelfTests: breaks exactly one rule. Never referenced by production code.
#pragma warning disable CS0169, CA1812, CA1823, IDE0051, IDE0052 // inspected by ArchUnitNET, never executed

namespace Polsan.Leave.Architecture.Tests.Fixtures.Rule2;

/// <summary>A "domain" type that depends on ASP.NET Core.</summary>
internal sealed class DomainWithAspNetCore
{
    private readonly Microsoft.AspNetCore.Http.HttpContext? _context;
}
