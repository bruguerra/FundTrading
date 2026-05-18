using MediatR;

namespace FundTrading.Application.Customers.Commands
{
    public class CreateCustomerCommand : IRequest<int>
    {
        public string Name { get; private set; }
        public string Document { get; private set; }
        public decimal AvailableBalance { get; private set; }

        public CreateCustomerCommand(string name,
                                     string document,
                                     decimal availableBalance)
        {
            Name = name;
            Document = document;
            AvailableBalance = availableBalance;
        }
    }
}