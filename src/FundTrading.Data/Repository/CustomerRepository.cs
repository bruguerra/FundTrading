using FundTrading.Domain.Entities;
using FundTrading.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace FundTrading.Data.Repository
{
    public class CustomerRepository : IDisposable, ICustomerRepository
    {
        private readonly FundTradingContext _context;

        public CustomerRepository(FundTradingContext context)
        {
            _context = context;
        }

        public IUnitOfWork UnitOfWork => _context;

        public async Task AddAsync(Customer customer, CancellationToken cancellationToken)
        {
            await _context.Set<Customer>()
                .AddAsync(customer, cancellationToken);
        }

        public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Set<Customer>()
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken)
        {
            return await _context.Set<Customer>()
                .FirstOrDefaultAsync(
                    x => x.Document == document && !x.IsDeleted,
                    cancellationToken);
        }

        public async Task<CustomerFundPosition?> GetFundPositionAsync(int customerId, int fundId, CancellationToken cancellationToken)
        {
            return await _context.Set<CustomerFundPosition>()
                .FirstOrDefaultAsync(x =>
                    x.CustomerId == customerId &&
                    x.InvestmentFundId == fundId &&
                    !x.IsDeleted,
                    cancellationToken);
        }

        public void UpdateAsync(Customer customer, CancellationToken cancellationToken)
        {
            _context.Set<Customer>().Update(customer);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
