using FundTrading.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace FundTrading.Application
{
    public abstract class CommandHandler
    {
        protected CommandHandler() { }

        protected async Task SaveDatabase(IUnitOfWork unitOfWork)
        {
            try
            {
                await unitOfWork.Commit();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception or handle it as needed
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
        }
    }
}
