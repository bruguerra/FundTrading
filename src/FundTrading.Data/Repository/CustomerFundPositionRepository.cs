using FundTrading.Domain.Entities;
using FundTrading.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace FundTrading.Data.Repository
{
    public class CustomerFundPositionRepository : ICustomerFundPositionRepository
    {
        private readonly FundTradingContext _context;

        public CustomerFundPositionRepository(FundTradingContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task<CustomerFundPosition?> GetByIdAsync(int id)
        {
            return await _context.Set<CustomerFundPosition>()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CustomerFundPosition?> GetByCustomerAndFundAsync(
            int customerId,
            int investmentFundId)
        {
            return await _context.Set<CustomerFundPosition>()
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customerId &&
                    x.InvestmentFundId == investmentFundId &&
                    !x.IsDeleted);
        }

        public async Task AddAsync(CustomerFundPosition position)
        {
            await _context.Set<CustomerFundPosition>().AddAsync(position);
        }

        public void Update(CustomerFundPosition position)
        {
            _context.Set<CustomerFundPosition>().Update(position);
        }
    }
}