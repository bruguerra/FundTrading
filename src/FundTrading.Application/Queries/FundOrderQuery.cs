using FundTrading.Domain.Entities;
using FundTrading.Domain.Repository;

namespace FundTrading.Application.Queries
{
    public class FundOrderQuery : IFundOrderQuery
    {
        private readonly IFundOrderRepository _fundOrderRepository;
        public FundOrderQuery(IFundOrderRepository fundOrderRepository) 
        {
            _fundOrderRepository = fundOrderRepository;
        }

        public async Task<IEnumerable<FundOrder>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken cancellationToken)
        {
            return await _fundOrderRepository.GetByCustomerIdAsync(customerId, cancellationToken);
        }

    }
}
