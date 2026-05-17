using FundTrading.Application.Commands;
using FundTrading.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FundTrading.API.DTOs;
using FundTrading.Application.Queries;

namespace FundTrading.API.Controllers
{
    [ApiController]
    [Route("ordens")]
    public class OrdersController : ApiControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFundOrderQuery _fundOrderQuery;

        public OrdersController(IMediator mediator,
                                IFundOrderQuery fundOrderQuery)
        {
            _mediator = mediator;
            _fundOrderQuery = fundOrderQuery;
        }

        [HttpPost]
        public async Task<IActionResult> CreateImmediateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateFundOrderCommand(
                    request.IdCliente,
                    request.IdFundo,
                    MapOperationType(request.TipoOperacao),
                    request.QuantidadeCotas,
                    sharePrice: 0,
                    scheduledDate: null);

                await _mediator.Send(command, cancellationToken);

                return CustomResponse(new
                {
                    message = "Order created successfully"
                });
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return CustomResponse();
            }
        }

        [HttpPost("agendamento")]
        public async Task<IActionResult> CreateScheduledOrder([FromBody] CreateScheduledOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateFundOrderCommand(
                    request.IdCliente,
                    request.IdFundo,
                    MapOperationType(request.TipoOperacao),
                    request.QuantidadeCotas,
                    sharePrice: 0,
                    scheduledDate: request.DataAgendamento);

                await _mediator.Send(command, cancellationToken);

                return CustomResponse(new
                {
                    message = "Scheduled order created successfully"
                });
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return CustomResponse();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery(Name = "id_cliente")] int? customerId, CancellationToken cancellationToken)
        {
            if (!customerId.HasValue)
            {
                AddError("Customer ID is required");
                return CustomResponse();
            }
            try
            {
                var orders = await _fundOrderQuery.GetOrdersByCustomerIdAsync(customerId.Value, cancellationToken);
                return CustomResponse(orders);
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return CustomResponse();
            }
        }

        private static OperationType MapOperationType(string operationType)
        {
            return operationType.ToUpperInvariant() switch
            {
                "APORTE" => OperationType.Contribution,
                "RESGATE" => OperationType.Redemption,
                _ => throw new Exception("Invalid operation type")
            };
        }
    }
}