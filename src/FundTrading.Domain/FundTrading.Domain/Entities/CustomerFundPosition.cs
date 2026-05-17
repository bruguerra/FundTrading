using FundTrading.Domain.DomainObjects;

namespace FundTrading.Domain.Entities
{
    public class CustomerFundPosition : Entity
    {
        public int CustomerId { get; private set; }
        public int InvestmentFundId { get; private set; }
        public int ShareQuantity { get; private set; }

        public Customer Customer { get; private set; }
        public InvestmentFund InvestmentFund { get; private set; }

        public CustomerFundPosition(int customerId,
                                    int investmentFundId,
                                    int shareQuantity)
        {
            CustomerId = customerId;
            InvestmentFundId = investmentFundId;
            ShareQuantity = shareQuantity;
        }

        public void AddShares(int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Share quantity must be greater than zero.");

            ShareQuantity += quantity;
        }

        public void RemoveShares(int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Share quantity must be greater than zero.");

            if (ShareQuantity < quantity)
                throw new Exception("Insufficient shares.");

            ShareQuantity -= quantity;
        }
    }
}
