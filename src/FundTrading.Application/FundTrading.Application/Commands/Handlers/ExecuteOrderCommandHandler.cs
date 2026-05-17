using FundTrading.Domain.Entities;
using FundTrading.Domain.Enums;
using FundTrading.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FundTrading.Application.Orders.Commands
{
    public class ExecuteFundOrderCommandHandler : CommandHandler,
                                                  IRequestHandler<ExecuteFundOrderCommand>
    {
        private readonly IFundOrderRepository _fundOrderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IInvestmentFundRepository _investmentFundRepository;
        private readonly ICustomerFundPositionRepository _customerFundPositionRepository;
        private readonly ILogger<ExecuteFundOrderCommandHandler> _logger;

        public ExecuteFundOrderCommandHandler(
            IFundOrderRepository fundOrderRepository,
            ICustomerRepository customerRepository,
            IInvestmentFundRepository investmentFundRepository,
            ICustomerFundPositionRepository customerFundPositionRepository,
            ILogger<ExecuteFundOrderCommandHandler> logger)
        {
            _fundOrderRepository = fundOrderRepository;
            _customerRepository = customerRepository;
            _investmentFundRepository = investmentFundRepository;
            _customerFundPositionRepository = customerFundPositionRepository;
            _logger = logger;
        }

        public async Task Handle(
            ExecuteFundOrderCommand request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Starting execution of order {OrderId}",
                request.OrderId);

            var order =
                await _fundOrderRepository.GetByIdAsync(request.OrderId, cancellationToken);

            if (order is null)
                throw new Exception("Order not found.");

            if (order.Status != OrderStatus.Pending &&
                order.Status != OrderStatus.Scheduled)
            {
                throw new Exception(
                    $"Order {order.Id} cannot be executed because status is {order.Status}");
            }

            var customer =
                await _customerRepository.GetByIdAsync(order.CustomerId, cancellationToken);

            if (customer is null)
                throw new Exception("Customer not found.");

            var fund =
                await _investmentFundRepository.GetByIdAsync(order.InvestmentFundId, cancellationToken);

            if (fund is null)
                throw new Exception("Investment fund not found.");

            switch (order.OperationType)
            {
                case OperationType.Contribution:

                    await ExecuteApplication(
                        customer,
                        fund,
                        order);

                    break;

                case OperationType.Redemption:

                    await ExecuteRedemption(
                        customer,
                        fund,
                        order);

                    break;

                default:
                    throw new Exception("Invalid operation type.");
            }

            order.MarkAsExecuted();

            await SaveDatabase(_fundOrderRepository.UnitOfWork);

            _logger.LogInformation(
                "Order {OrderId} executed successfully",
                order.Id);
        }

        private async Task ExecuteApplication(
            Customer customer,
            InvestmentFund fund,
            FundOrder order)
        {
            if (customer.AvailableBalance < order.TotalAmount)
            {
                order.Reject("Insufficient balance.");

                await SaveDatabase(_fundOrderRepository.UnitOfWork);

                throw new Exception("Insufficient balance.");
            }

            if (fund.CurrentCapacity + order.TotalAmount > fund.CapacityLimit)
            {
                order.Reject("Fund capacity exceeded.");

                await SaveDatabase(_fundOrderRepository.UnitOfWork);

                throw new Exception("Fund capacity exceeded.");
            }

            customer.Debit(order.TotalAmount);

            fund.AddCapacity(order.TotalAmount);

            var position =
                await _customerFundPositionRepository.GetByCustomerAndFundAsync(
                    customer.Id,
                    fund.Id);

            if (position is null)
            {
                position = new CustomerFundPosition(
                    customer.Id,
                    fund.Id,
                    order.ShareQuantity);

                await _customerFundPositionRepository.AddAsync(position);
            }
            else
            {
                position.AddShares(order.ShareQuantity);
            }
        }

        private async Task ExecuteRedemption(
            Customer customer,
            InvestmentFund fund,
            FundOrder order)
        {
            var position =
                await _customerFundPositionRepository.GetByCustomerAndFundAsync(
                    customer.Id,
                    fund.Id);

            if (position is null ||
                position.ShareQuantity < order.ShareQuantity)
            {
                order.Reject("Insufficient shares.");

                await SaveDatabase(_fundOrderRepository.UnitOfWork);

                throw new Exception("Insufficient shares.");
            }

            position.RemoveShares(order.ShareQuantity);

            customer.Credit(order.TotalAmount);

            fund.RemoveCapacity(order.TotalAmount);
        }
    }
}