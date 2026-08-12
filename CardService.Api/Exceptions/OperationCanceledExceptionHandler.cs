using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CardService.Api.Exceptions;

public sealed class OperationCanceledExceptionHandler : IExceptionHandler
{
    private readonly ILogger<OperationCanceledExceptionHandler> _logger;

    public OperationCanceledExceptionHandler(ILogger<OperationCanceledExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not OperationCanceledException)
        {
            return false;
        }

        if (context.RequestAborted.IsCancellationRequested)
        {
            return true;
        }

        _logger.LogWarning(
            exception,
            "Request cancelled. CorrelationId={CorrelationId}",
            context.TraceIdentifier);

        context.Response.StatusCode = StatusCodes.Status408RequestTimeout;

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status408RequestTimeout,
            Title = "Request timeout",
            Detail = "The request was cancelled before it completed",
            Instance = context.Request.Path
        };

        problem.Extensions["correlationId"] = context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}