using FundTrading.Domain.DomainObjects;
using FundTrading.Domain.Enums;

namespace FundTrading.Domain.Entities
{
    public class FundOrder : Entity
    {
        public int CustomerId { get; private set; }
        public int InvestmentFundId { get; private set; }
        public OperationType OperationType { get; private set; }
        public int ShareQuantity { get; private set; }
        public decimal SharePrice { get; private set; }
        public decimal TotalAmount { get; private set; }
        public DateOnly? ScheduledDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public string? RejectionReason { get; private set; }

        public Customer Customer { get; private set; }
        public InvestmentFund InvestmentFund { get; private set; }

        public FundOrder(int customerId,
                         int investmentFundId,
                         OperationType operationType,
                         int shareQuantity,
                         decimal sharePrice,
                         DateOnly? scheduledDate,
                         OrderStatus status)
        {
            CustomerId = customerId;
            InvestmentFundId = investmentFundId;
            OperationType = operationType;
            ShareQuantity = shareQuantity;
            SharePrice = sharePrice;
            TotalAmount = shareQuantity * sharePrice;
            ScheduledDate = scheduledDate;
            Status = status;
        }

        public void MarkAsExecuted()
        {
            Status = OrderStatus.Processed;
            RejectionReason = null;
        }

        public void Reject(string reason)
        {
            Status = OrderStatus.Rejected;
            RejectionReason = reason;
        }
    }
}
