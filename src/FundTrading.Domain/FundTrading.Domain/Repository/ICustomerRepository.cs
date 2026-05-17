using FundTrading.Domain.Entities;

namespace FundTrading.Domain.Repository
{
    public interface ICustomerRepository : IRepository
    {
        Task AddAsync(Customer customer, CancellationToken cancellationToken);
        Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Customer?> GetByDocumentAsync(string document, CancellationToken cancellationToken);
        Task<CustomerFundPosition> GetFundPositionAsync(int customerId, int fundId, CancellationToken cancellationToken);
        void UpdateAsync(Customer customer, CancellationToken cancellationToken);
    }
}
