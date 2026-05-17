using FundTrading.Application.Funds.Commands;
using FundTrading.Domain.Entities;
using FundTrading.Domain.Enums;
using FundTrading.Domain.Repository;
using FluentAssertions;
using Moq;

namespace FundTrading.Tests.Application.Funds.Commands
{
    public class CreateInvestmentFundCommandHandlerTests
    {
        private readonly Mock<IInvestmentFundRepository> _fundRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateInvestmentFundCommandHandler _handler;

        public CreateInvestmentFundCommandHandlerTests()
        {
            _fundRepositoryMock = new Mock<IInvestmentFundRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _fundRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new CreateInvestmentFundCommandHandler(
                _fundRepositoryMock.Object);
        }

        [Fact]
        public async Task Should_Create_Investment_Fund_When_Data_Is_Valid()
        {
            // Arrange
            var command = new CreateInvestmentFundCommand(
                name: "Itaú Asset Renda Fixa",
                cutoffTime: new TimeOnly(15, 00),
                sharePrice: 10.50m,
                minimumContributionAmount: 1000m,
                minimumRemainingBalance: 500m,
                capacityLimit: 1_000_000m,
                status: FundStatus.Open);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var fundId = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            fundId.Should().BeGreaterThan(0);

            _fundRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<InvestmentFund>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.Commit(),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_SharePrice_Is_Zero()
        {
            // Arrange
            var command = new CreateInvestmentFundCommand(
                name: "Itaú Asset Renda Fixa",
                cutoffTime: new TimeOnly(15, 00),
                sharePrice: 0,
                minimumContributionAmount: 1000m,
                minimumRemainingBalance: 500m,
                capacityLimit: 1_000_000m,
                status: FundStatus.Open);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Share price must be greater than zero");

            _fundRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<InvestmentFund>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.Commit(),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_MinimumContributionAmount_Is_Zero()
        {
            // Arrange
            var command = new CreateInvestmentFundCommand(
                name: "Itaú Asset Renda Fixa",
                cutoffTime: new TimeOnly(15, 00),
                sharePrice: 10.50m,
                minimumContributionAmount: 0,
                minimumRemainingBalance: 500m,
                capacityLimit: 1_000_000m,
                status: FundStatus.Open);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Minimum contribution amount must be greater than zero");

            _fundRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<InvestmentFund>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.Commit(),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_CapacityLimit_Is_Zero()
        {
            // Arrange
            var command = new CreateInvestmentFundCommand(
                name: "Itaú Asset Renda Fixa",
                cutoffTime: new TimeOnly(15, 00),
                sharePrice: 10.50m,
                minimumContributionAmount: 1000m,
                minimumRemainingBalance: 500m,
                capacityLimit: 0,
                status: FundStatus.Open);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Capacity limit must be greater than zero");

            _fundRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<InvestmentFund>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.Commit(),
                Times.Never);
        }
    }
}