using FundTrading.Domain.Entities;
using FundTrading.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace FundTrading.Data.Repository
{
    public class InvestmentFundRepository : IDisposable, IInvestmentFundRepository
    {
        private readonly FundTradingContext _context;

        public InvestmentFundRepository(FundTradingContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task AddAsync(InvestmentFund fund, CancellationToken cancellationToken)
        {
            await _context.Set<InvestmentFund>()
                .AddAsync(fund, cancellationToken);
        }

        public async Task<InvestmentFund?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Set<InvestmentFund>()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task UpdateAsync(InvestmentFund fund, CancellationToken cancellationToken)
        {
            _context.Set<InvestmentFund>().Update(fund);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}