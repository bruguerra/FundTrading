using FundTrading.Domain.Entities;

namespace FundTrading.Domain.Repository
{
    public interface IInvestmentFundRepository : IRepository
    {
        Task AddAsync(InvestmentFund fund, CancellationToken cancellationToken);
        Task<InvestmentFund?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateAsync(InvestmentFund fund, CancellationToken cancellationToken);
    }
}
