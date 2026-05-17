using FundTrading.Domain.Entities;

namespace FundTrading.Domain.Repository
{
    public interface IFundOrderRepository : IRepository
    {
        Task AddAsync(FundOrder order, CancellationToken cancellationToken);
        Task<FundOrder?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<FundOrder>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken);
        Task<List<FundOrder>> GetScheduledOrdersToProcessAsync(DateOnly date,CancellationToken cancellationToken);
    }
}
