using FundTrading.Domain.Enums;
using MediatR;

namespace FundTrading.Application.Commands
{
    public class CreateFundOrderCommand : IRequest
    {
        public int CustomerId { get; private set; }
        public int InvestmentFundId { get; private set; }
        public OperationType OperationType { get; private set; }
        public int ShareQuantity { get; private set; }
        public decimal SharePrice { get; private set; }
        public DateOnly? ScheduledDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public string? RejectionReason { get; private set; }

        public CreateFundOrderCommand(int customerId,
                                      int investmentFundId,
                                      OperationType operationType,
                                      int shareQuantity,
                                      decimal sharePrice,
                                      DateOnly? scheduledDate = null)
        {
            CustomerId = customerId;
            InvestmentFundId = investmentFundId;
            OperationType = operationType;
            ShareQuantity = shareQuantity;
            SharePrice = sharePrice;
            ScheduledDate = scheduledDate;
            Status = OrderStatus.Pending;
            RejectionReason = null;
        }

    }
}
