using FundTrading.Domain.Entities;

namespace FundTrading.Domain.Repository
{
    public interface ICustomerFundPositionRepository : IRepository
    {
        Task<CustomerFundPosition?> GetByIdAsync(int id);

        Task<CustomerFundPosition?> GetByCustomerAndFundAsync(
            int customerId,
            int investmentFundId);

        Task AddAsync(CustomerFundPosition position);

        void Update(CustomerFundPosition position);
    }
}