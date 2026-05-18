using FundTrading.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FundTrading.Application.Orders.Commands
{
    public class ProcessScheduledFundOrdersCommandHandler
        : IRequestHandler<ProcessScheduledFundOrdersCommand>
    {
        private readonly IFundOrderRepository _fundOrderRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessScheduledFundOrdersCommandHandler> _logger;

        public ProcessScheduledFundOrdersCommandHandler(
            IFundOrderRepository fundOrderRepository,
            IMediator mediator,
            ILogger<ProcessScheduledFundOrdersCommandHandler> logger)
        {
            _fundOrderRepository = fundOrderRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(
            ProcessScheduledFundOrdersCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting scheduled fund orders processing");

            var today = DateOnly.FromDateTime(DateTime.Today);

            var scheduledOrders =
                await _fundOrderRepository.GetScheduledOrdersToProcessAsync(
                    today,
                    cancellationToken);

            _logger.LogInformation(
                "Found {Count} scheduled fund orders to process",
                scheduledOrders.Count);

            foreach (var order in scheduledOrders)
            {
                try
                {
                    _logger.LogInformation(
                        "Processing scheduled fund order {OrderId}",
                        order.Id);

                    await _mediator.Send(
                        new ExecuteFundOrderCommand(order.Id),
                        cancellationToken);

                    _logger.LogInformation(
                        "Scheduled fund order {OrderId} processed successfully",
                        order.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error processing scheduled fund order {OrderId}",
                        order.Id);
                }
            }

            _logger.LogInformation("Finished scheduled fund orders processing");
        }
    }
}