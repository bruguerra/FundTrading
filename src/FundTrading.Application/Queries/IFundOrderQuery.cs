using FundTrading.Domain.Entities;

namespace FundTrading.Application.Queries
{
    public interface IFundOrderQuery
    {
        Task<IEnumerable<FundOrder>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
    }
}
