using Serilog.Context;

namespace FundTrading.API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        public const string Header = "X-Correlation-Id";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId =
                context.Request.Headers[Header].FirstOrDefault()
                ?? Guid.NewGuid().ToString();

            context.Items[Header] = correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[Header] = correlationId;
                return Task.CompletedTask;
            });

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next(context);
            }
        }
    }
}