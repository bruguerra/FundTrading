namespace FundTrading.Domain.Repository
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
