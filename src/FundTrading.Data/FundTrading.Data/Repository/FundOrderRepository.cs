using FundTrading.Domain.Entities;
using FundTrading.Domain.Enums;
using FundTrading.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace FundTrading.Data.Repository
{
    public class FundOrderRepository : IDisposable, IFundOrderRepository
    {
        private readonly FundTradingContext _context;

        public FundOrderRepository(FundTradingContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task AddAsync(FundOrder order, CancellationToken cancellationToken)
        {
            await _context.Set<FundOrder>()
                .AddAsync(order, cancellationToken);
        }

        public async Task<FundOrder?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Set<FundOrder>()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<FundOrder>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            return await _context.Set<FundOrder>()
                .Where(x => x.CustomerId == customerId && !x.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<FundOrder>> GetScheduledOrdersToProcessAsync(DateOnly date, CancellationToken cancellationToken)
        {
            return await _context.Set<FundOrder>()
                .Where(x =>
                    x.Status == OrderStatus.Scheduled &&
                    x.ScheduledDate.HasValue &&
                    x.ScheduledDate.Value <= date)
                .ToListAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}