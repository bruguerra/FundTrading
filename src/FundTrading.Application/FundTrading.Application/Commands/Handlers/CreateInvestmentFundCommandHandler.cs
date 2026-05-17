using FundTrading.Domain.Entities;
using FundTrading.Domain.Repository;
using MediatR;

namespace FundTrading.Application.Funds.Commands
{
    public class CreateInvestmentFundCommandHandler : CommandHandler,
                                                      IRequestHandler<CreateInvestmentFundCommand, int>
    {
        private readonly IInvestmentFundRepository _fundRepository;

        public CreateInvestmentFundCommandHandler(
            IInvestmentFundRepository fundRepository)
        {
            _fundRepository = fundRepository;
        }

        public async Task<int> Handle(
            CreateInvestmentFundCommand request,
            CancellationToken cancellationToken)
        {
            if (request.SharePrice <= 0)
                throw new Exception("Share price must be greater than zero");

            if (request.MinimumContributionAmount <= 0)
                throw new Exception("Minimum contribution amount must be greater than zero");

            if (request.CapacityLimit <= 0)
                throw new Exception("Capacity limit must be greater than zero");

            var fund = new InvestmentFund(
                request.Name,
                request.CutoffTime,
                request.SharePrice,
                request.MinimumContributionAmount,
                request.MinimumRemainingBalance,
                request.CapacityLimit,
                request.Status);

            await _fundRepository.AddAsync(
                fund,
                cancellationToken);

            await SaveDatabase(
                _fundRepository.UnitOfWork);

            return fund.Id;
        }
    }
}