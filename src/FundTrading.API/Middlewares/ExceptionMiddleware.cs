using FundTrading.Application.Services.Notification;
using Serilog;

namespace FundTrading.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly INotificationService _notificationService;

        //public ExceptionMiddleware(RequestDelegate next, INotificationService notificationService)
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
            //_notificationService = notificationService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var correlationId =
                    context.Items["X-Correlation-Id"]?.ToString()
                    ?? Guid.NewGuid().ToString();

                // 1. Log estruturado (Serilog)
                Log.Error(ex,
                    "Unhandled exception. CorrelationId: {CorrelationId}",
                    correlationId);

                // 2. Notificação (Teams ou qualquer outro canal)
                //await _notificationService.NotifyAsync(new NotificationMessage
                //{
                //    Title = "Unhandled Exception",
                //    Message = ex.Message,
                //    CorrelationId = correlationId,
                //    Type = NotificationType.Error
                //});

                // 3. Response padronizada
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Internal server error",
                    correlationId
                });
            }
        }
    }
}