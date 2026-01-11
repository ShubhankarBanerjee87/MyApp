namespace MyNewApp.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var correlationId = httpContext.Items["CorrelationId"]?.ToString() ?? "N/A";

                _logger.LogError(
                    ex,
                    "Unhandled exception. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}",
                    correlationId,
                    httpContext.Request.Path,
                    httpContext.Request.Method
                );

                httpContext.Response.Clear();
                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                httpContext.Response.ContentType = "application/json";

                var response = new
                {
                    IsSuccess = false,
                    Message = "An unexpected error occurred. Please try again later.",
                    CorrelationId = correlationId
                };

                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
