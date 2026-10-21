using Microsoft.AspNetCore.Diagnostics;

namespace Polsan.Leave.Web.Errors;

/// <summary>
/// Logs every unhandled exception with the error id the user sees on the error page.
/// Returns false so the exception handler middleware re-executes the error page.
/// </summary>
public sealed partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        LogUnhandled(logger, exception, httpContext.TraceIdentifier);
        return ValueTask.FromResult(false);
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = "Unhandled exception. ErrorId={ErrorId}")]
    private static partial void LogUnhandled(ILogger logger, Exception exception, string errorId);
}
