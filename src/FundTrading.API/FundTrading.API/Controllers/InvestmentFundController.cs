using FundTrading.API.DTOs;
using FundTrading.Application.Funds.Commands;
using FundTrading.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FundTrading.API.Controllers
{
    [ApiController]
    [Route("fundos")]
    public class InvestmentFundController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public InvestmentFundController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvestmentFund(
            [FromBody] CreateInvestmentFundRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var command = new CreateInvestmentFundCommand(
                    request.Name,
                    request.CutoffTime,
                    request.SharePrice,
                    request.MinimumContributionAmount,
                    request.MinimumRemainingBalance,
                    request.CapacityLimit,
                    MapFundStatus(request.Status));

                var fundId = await _mediator.Send(command, cancellationToken);

                return CustomResponse(new
                {
                    id = fundId,
                    message = "Investment fund created successfully"
                });
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return CustomResponse();
            }
        }

        private static FundStatus MapFundStatus(string status)
        {
            return status.ToUpperInvariant() switch
            {
                "ABERTO" => FundStatus.Open,
                "FECHADO" => FundStatus.Closed,
                _ => throw new Exception("Invalid fund status")
            };
        }
    }
}