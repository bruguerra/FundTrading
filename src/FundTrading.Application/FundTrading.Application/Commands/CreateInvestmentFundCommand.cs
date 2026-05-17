using FundTrading.Domain.Enums;
using MediatR;

namespace FundTrading.Application.Funds.Commands
{
    public class CreateInvestmentFundCommand : IRequest<int>
    {
        public string Name { get; private set; }
        public TimeOnly CutoffTime { get; private set; }
        public decimal SharePrice { get; private set; }
        public decimal MinimumContributionAmount { get; private set; }
        public decimal MinimumRemainingBalance { get; private set; }
        public decimal CapacityLimit { get; private set; }
        public FundStatus Status { get; private set; }

        public CreateInvestmentFundCommand(
            string name,
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
    }
}