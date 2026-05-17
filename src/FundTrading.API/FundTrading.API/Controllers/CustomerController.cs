using FundTrading.API.DTOs;
using FundTrading.Application.Customers.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FundTrading.API.Controllers
{
    [ApiController]
    [Route("clientes")]
    public class CustomerController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(
            [FromBody] CreateCustomerRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateCustomerCommand(
                    request.Name,
                    request.Document,
                    request.AvailableBalance);

                var customerId =
                    await _mediator.Send(command, cancellationToken);

                return CustomResponse(new
                {
                    id = customerId,
                    message = "Customer created successfully"
                });
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return CustomResponse();
            }
        }
    }
}