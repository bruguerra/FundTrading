using FundTrading.Application.Orders.Commands;
using MediatR;
using Quartz;
using Serilog.Context;

namespace FundTrading.API.Jobs
{
    public class ProcessScheduledOrdersJob : IJob
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessScheduledOrdersJob> _logger;

        public ProcessScheduledOrdersJob(
            IMediator mediator,
            ILogger<ProcessScheduledOrdersJob> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var correlationId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                _logger.LogInformation("Fund Orders Job triggered scheduled orders processing");

                await _mediator.Send(
                    new ProcessScheduledFundOrdersCommand(),
                    context.CancellationToken);
            }
        }
    }
}