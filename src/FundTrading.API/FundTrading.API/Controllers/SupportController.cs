using FundTrading.Application.Orders.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FundTrading.API.Controllers
{
    [ApiController]
    [Route("support")]
    public class SupportController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SupportController> _logger;

        public SupportController(IMediator mediator, ILogger<SupportController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("process-scheduled-orders")]
        public async Task<IActionResult> ProcessScheduledOrders(
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Manual trigger for scheduled orders processing");

                await _mediator.Send(
                    new ProcessScheduledFundOrdersCommand(),
                    cancellationToken);

                return CustomResponse(new
                {
                    message = "Scheduled orders processing started successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while manually processing scheduled orders");

                AddError(ex.Message);

                return CustomResponse();
            }
        }
    }
}