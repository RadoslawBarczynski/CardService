using CardService.Api.Controllers;
using CardService.Api.Services;
using CardService.Domain.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace CardService.Api.Exceptions
{
    public sealed class OperationCanceledExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<OperationCanceledExceptionHandler> _logger;

        public OperationCanceledExceptionHandler( ILogger<OperationCanceledExceptionHandler> logger)
        {
            _logger = logger;
        }

        public ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken ct)
        {
            if (ex is not OperationCanceledException) return ValueTask.FromResult(false);

            if (context.RequestAborted.IsCancellationRequested)
            {
                return ValueTask.FromResult(true);
            }

            _logger.LogWarning(ex, "Request was cancelled.");
            context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
            return ValueTask.FromResult(true);
        }
    }
}
