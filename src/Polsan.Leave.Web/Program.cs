using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Polsan.Leave.Web.Components;
using Polsan.Leave.Web.Errors;
using Polsan.Leave.Web.Health;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddLocalization();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Without a LeaveDb connection string the app still starts; the health check then reports Unhealthy.
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");

var app = builder.Build();

var turkish = new CultureInfo("tr-TR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(turkish),
    SupportedCultures = [turkish],
    SupportedUICultures = [turkish],
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

// Status text only (Healthy / Unhealthy) — never names, versions or error details (docs/security.md).
app.MapHealthChecks("/health");

if (app.Environment.IsEnvironment("Testing"))
{
    // Test-only trigger for the error page (spec 0001, AC-4). Not mapped in any other environment.
    app.MapGet("/_test/throw", string () => throw new InvalidOperationException("Test-only error trigger."));
}

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();

/// <summary>Entry point; public so the test hosts can reference it.</summary>
public partial class Program;
