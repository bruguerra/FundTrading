using MediatR;

namespace FundTrading.Application.Orders.Commands
{
    public class ExecuteFundOrderCommand : IRequest
    {
        public int OrderId { get; private set; }

        public ExecuteFundOrderCommand(int orderId)
        {
            OrderId = orderId;
        }
    }
}