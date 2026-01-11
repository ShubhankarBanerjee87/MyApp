namespace MyNewApp.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var correlationId =
            httpContext.Request.Headers.TryGetValue(HeaderName, out var id)
                ? id.ToString()
                : Guid.NewGuid().ToString();

            httpContext.Items["CorrelationId"] = correlationId;
            httpContext.Response.Headers[HeaderName] = correlationId;

            using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(httpContext);
            }
        }
    }
}
