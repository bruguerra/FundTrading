namespace FundTrading.Domain.Repository
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
