using FundTrading.Domain.DomainObjects;
using FundTrading.Domain.Enums;

namespace FundTrading.Domain.Entities
{
    public class InvestmentFund : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public TimeOnly CutoffTime { get; private set; }
        public decimal SharePrice { get; private set; }
        public decimal MinimumContributionAmount { get; private set; }
        public decimal MinimumRemainingBalance { get; private set; }
        public decimal CurrentCapacity { get; private set; }
        public decimal CapacityLimit { get; private set; }
        public FundStatus Status { get; private set; }

        public virtual ICollection<CustomerFundPosition> Positions { get; private set; }
        public virtual ICollection<FundOrder> Orders { get; private set; }

        public InvestmentFund(string name,
                              TimeOnly cutoffTime,
                              decimal sharePrice,
                              decimal minimumContributionAmount,
                              decimal minimumRemainingBalance,
                              decimal capacityLimit,
                              FundStatus status)
        {
            Name = name;
            CutoffTime = cutoffTime;
            SharePrice = sharePrice;
            MinimumContributionAmount = minimumContributionAmount;
            MinimumRemainingBalance = minimumRemainingBalance;
            CapacityLimit = capacityLimit;
            Status = status;
        }

        public bool IsOpen()
        {
            return Status == FundStatus.Open;
        }

        public void AddCapacity(decimal amount)
        {
            CurrentCapacity += amount;
        }

        public void RemoveCapacity(decimal amount)
        {
            CurrentCapacity -= amount;
        }
    }
}
