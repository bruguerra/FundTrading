using Microsoft.AspNetCore.Mvc;

namespace FundTrading.API.Controllers
{
    public class ApiControllerBase : Controller
    {
        protected ICollection<string> Errors = new List<string>();

        protected string CorrelationId =>
            HttpContext.Items["X-Correlation-Id"]?.ToString()
            ?? string.Empty;

        protected IActionResult CustomResponse(object? result = null)
        {
            if (Errors.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    correlationId = CorrelationId,
                    errors = Errors
                });
            }

            return Ok(new
            {
                success = true,
                correlationId = CorrelationId,
                data = result
            });
        }

        protected void AddError(string error)
        {
            Errors.Add(error);
        }
    }
}