using System.Diagnostics;

namespace BoardOutlook.Api.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                // Log incoming request
                _logger.LogInformation("Incoming Request: {Method} {Path}{QueryString}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString);

                // Optional: log headers
                // foreach (var header in context.Request.Headers)
                // {
                //     _logger.LogDebug("Header: {Key} = {Value}", header.Key, header.Value);
                // }

                // Call next middleware
                await _next(context);

                sw.Stop();

                // Log outgoing response
                _logger.LogInformation("Response: {StatusCode} executed in {ElapsedMilliseconds}ms",
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                sw.Stop();
                _logger.LogError(ex, "Exception in RequestLoggingMiddleware after {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
                throw; // rethrow so exception handler middleware can handle it
            }
        }
    }

}
