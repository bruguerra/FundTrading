using FundTrading.Application.Orders.Commands;
using FundTrading.Domain.Entities;
using FundTrading.Domain.Enums;
using FundTrading.Domain.Repository;
using MediatR;

namespace FundTrading.Application.Commands.Handlers
{
    public class CreateFundOrderCommandHandler : CommandHandler,
                                                 IRequestHandler<CreateFundOrderCommand>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IInvestmentFundRepository _fundRepository;
        private readonly IFundOrderRepository _fundOrderRepository;
        private readonly IMediator _mediator;

        public CreateFundOrderCommandHandler(
            ICustomerRepository customerRepository,
            IInvestmentFundRepository fundRepository,
            IFundOrderRepository fundOrderRepository,
            IMediator mediator)
        {
            _customerRepository = customerRepository;
            _fundRepository = fundRepository;
            _fundOrderRepository = fundOrderRepository;
            _mediator = mediator;
        }

        public async Task Handle(
            CreateFundOrderCommand request,
            CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetByIdAsync(
                request.CustomerId,
                cancellationToken);

            if (customer is null)
                throw new Exception("Customer not found");

            var fund = await _fundRepository.GetByIdAsync(
                request.InvestmentFundId,
                cancellationToken);

            if (fund is null)
                throw new Exception("Investment fund not found");

            if (!fund.IsOpen())
                throw new Exception("Fund is closed for operations");

            var isScheduled = request.ScheduledDate.HasValue;

            if (!isScheduled)
            {
                if (request.OperationType == OperationType.Contribution)
                {
                    var total = request.ShareQuantity * fund.SharePrice;

                    if (customer.AvailableBalance < total)
                        throw new Exception("Insufficient balance");

                    if (total < fund.MinimumContributionAmount)
                        throw new Exception("Below minimum contribution");
                }

                if (request.OperationType == OperationType.Redemption)
                {
                    var position =
                        await _customerRepository.GetFundPositionAsync(
                            request.CustomerId,
                            request.InvestmentFundId,
                            cancellationToken);

                    if (position is null ||
                        position.ShareQuantity < request.ShareQuantity)
                    {
                        throw new Exception("Insufficient position");
                    }
                }
            }

            // Regras de agendamento
            if (isScheduled)
            {
                var scheduledDate = request.ScheduledDate!.Value;

                if (scheduledDate <= DateOnly.FromDateTime(DateTime.Today))
                    throw new Exception("Scheduled date must be a future business day");

                if (scheduledDate.DayOfWeek == DayOfWeek.Saturday ||
                    scheduledDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    throw new Exception("Scheduled date cannot be weekend");
                }
            }

            var orderStatus =
                isScheduled
                    ? OrderStatus.Scheduled
                    : OrderStatus.Pending;

            var order = new FundOrder(
                request.CustomerId,
                request.InvestmentFundId,
                request.OperationType,
                request.ShareQuantity,
                fund.SharePrice,
                request.ScheduledDate,
                orderStatus);

            await _fundOrderRepository.AddAsync(
                order,
                cancellationToken);

            await SaveDatabase(_fundOrderRepository.UnitOfWork);

            // Execução imediata
            if (!isScheduled)
            {
                await _mediator.Send(new ExecuteFundOrderCommand(order.Id), cancellationToken);
            }
        }
    }
}