using FundTrading.Domain.DomainObjects;

namespace FundTrading.Domain.Entities
{
    public class Customer : Entity
    {
        public string Name { get; private set; }
        public string Document { get; private set; }
        public decimal AvailableBalance { get; private set; }

        public virtual ICollection<FundOrder> Orders { get; private set; }

        public virtual ICollection<CustomerFundPosition> Positions { get; private set; }
        public Customer(string name, 
                        string document, 
                        decimal availableBalance)
        {
            Name = name;
            Document = document;
            AvailableBalance = availableBalance;
        }

        public void Debit(decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Debit amount must be greater than zero.");

            if (AvailableBalance < amount)
                throw new Exception("Insufficient balance.");

            AvailableBalance -= amount;
        }

        public void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Credit amount must be greater than zero.");

            AvailableBalance += amount;
        }
    }
}
