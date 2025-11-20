using Microsoft.AspNetCore.Diagnostics;

namespace BoardOutlook.Api.Middleware
{
    public class ApplicationExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ApplicationExceptionHandler> _logger;

        public ApplicationExceptionHandler(ILogger<ApplicationExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Every error gets logged
            var correlationId = Guid.NewGuid().ToString();
            _logger.LogError(exception,
                "Unhandled exception occurred. CorrelationId: {CorrelationId}", correlationId);

            // Default response
            var statusCode = StatusCodes.Status500InternalServerError;
            var title = "An unexpected error occurred.";

            // ----------------------------------
            // 1. Handle cancellation
            // ----------------------------------
            if (exception is OperationCanceledException)
            {
                statusCode = StatusCodes.Status499ClientClosedRequest;
                title = "The request was cancelled.";
            }
            // ----------------------------------
            // 2. Handle validation errors
            // ----------------------------------
            else if (exception is ArgumentNullException || exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                title = exception.Message;
            }
            // ----------------------------------
            // 3. Not found
            // ----------------------------------
            else if (exception is KeyNotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                title = exception.Message;
            }
            // ----------------------------------
            // 4. Custom domain or application exceptions
            // ----------------------------------
            else if (exception is ApplicationException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                title = exception.Message;
            }
            // ----------------------------------
            // 5. Timeout exceptions
            // ----------------------------------
            else if (exception is TimeoutException)
            {
                statusCode = StatusCodes.Status504GatewayTimeout;
                title = "The request timed out.";
            }
            // ----------------------------------
            // 6. Unauthorized / forbidden
            // ----------------------------------
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status403Forbidden;
                title = "You do not have permission to perform this operation.";
            }
            // ----------------------------------
            // 7. Fallback: Unexpected errors
            // ----------------------------------
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
            }

            // Prepare JSON error response
            var problemDetails = new
            {
                status = statusCode,
                title,
                detail = exception.Message,
                correlationId,
                path = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            // Returning true prevents further propagation
            return true;
        }
    }
}
