using FluentAssertions;
using FundTrading.Application.Commands;
using FundTrading.Application.Commands.Handlers;
using FundTrading.Application.Orders.Commands;
using FundTrading.Domain.Entities;
using FundTrading.Domain.Enums;
using FundTrading.Domain.Repository;
using MediatR;
using Moq;

namespace FundTrading.Tests.Application.Orders.Commands
{
    public class CreateFundOrderCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IInvestmentFundRepository> _fundRepositoryMock;
        private readonly Mock<IFundOrderRepository> _fundOrderRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly CreateFundOrderCommandHandler _handler;

        public CreateFundOrderCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _fundRepositoryMock = new Mock<IInvestmentFundRepository>();
            _fundOrderRepositoryMock = new Mock<IFundOrderRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mediatorMock = new Mock<IMediator>();

            _fundOrderRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            _handler = new CreateFundOrderCommandHandler(
                _customerRepositoryMock.Object,
                _fundRepositoryMock.Object,
                _fundOrderRepositoryMock.Object,
                _mediatorMock.Object);
        }

        [Fact]
        public async Task Should_Create_And_Execute_Immediate_Contribution_Order_When_Data_Is_Valid()
        {
            // Arrange
            var customer = new Customer(
                name: "Bruno Ferreira",
                document: "12345678901",
                availableBalance: 10000m);

            var fund = new InvestmentFund(
                name: "Itaú Asset RF",
                cutoffTime: new TimeOnly(15, 00),
                sharePrice: 10m,
                minimumContributionAmount: 100m,
                minimumRemainingBalance: 50m,
                capacityLimit: 1_000_000m,
                status: FundStatus.Open);

            var command = new CreateFundOrderCommand(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Contribution,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: null);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<FundOrder>(o =>
                        o.CustomerId == command.CustomerId &&
                        o.InvestmentFundId == command.InvestmentFundId &&
                        o.OperationType == OperationType.Contribution &&
                        o.ShareQuantity == command.ShareQuantity &&
                        o.Status == OrderStatus.Pending),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);

            _mediatorMock.Verify(
                x => x.Send(
                    It.IsAny<ExecuteFundOrderCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_Create_Scheduled_Order_And_Not_Execute_Immediately()
        {
            // Arrange
            var customer = new Customer(
                "Bruno Ferreira",
                "12345678901",
                10000m);

            var fund = new InvestmentFund(
                "Itaú Asset RF",
                new TimeOnly(15, 00),
                10m,
                100m,
                50m,
                1_000_000m,
                FundStatus.Open);

            var scheduledDate = GetNextBusinessDay();

            var command = new CreateFundOrderCommand(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Contribution,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: scheduledDate);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(
                    It.Is<FundOrder>(o =>
                        o.Status == OrderStatus.Scheduled &&
                        o.ScheduledDate == scheduledDate),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);

            _mediatorMock.Verify(
                x => x.Send(
                    It.IsAny<ExecuteFundOrderCommand>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Customer_Not_Found()
        {
            // Arrange
            var command = CreateValidImmediateContributionCommand();

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Customer not found");

            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<FundOrder>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Investment_Fund_Not_Found()
        {
            // Arrange
            var command = CreateValidImmediateContributionCommand();

            var customer = new Customer(
                "Bruno Ferreira",
                "12345678901",
                10000m);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((InvestmentFund?)null);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Investment fund not found");

            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<FundOrder>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Fund_Is_Closed()
        {
            // Arrange
            var command = CreateValidImmediateContributionCommand();

            var customer = new Customer(
                "Bruno Ferreira",
                "12345678901",
                10000m);

            var fund = new InvestmentFund(
                "Itaú Asset RF",
                new TimeOnly(15, 00),
                10m,
                100m,
                50m,
                1_000_000m,
                FundStatus.Closed);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Fund is closed for operations");

            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<FundOrder>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Immediate_Contribution_Has_Insufficient_Balance()
        {
            // Arrange
            var customer = new Customer(
                "Bruno Ferreira",
                "12345678901",
                50m);

            var fund = new InvestmentFund(
                "Itaú Asset RF",
                new TimeOnly(15, 00),
                10m,
                100m,
                50m,
                1_000_000m,
                FundStatus.Open);

            var command = new CreateFundOrderCommand(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Contribution,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: null);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Insufficient balance");

            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<FundOrder>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mediatorMock.Verify(
                x => x.Send(It.IsAny<ExecuteFundOrderCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Immediate_Contribution_Is_Below_Minimum()
        {
            // Arrange
            var customer = new Customer(
                "Bruno Ferreira",
                "12345678901",
                10000m);

            var fund = new InvestmentFund(
                "Itaú Asset RF",
                new TimeOnly(15, 00),
                10m,
                1000m,
                50m,
                1_000_000m,
                FundStatus.Open);

            var command = new CreateFundOrderCommand(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Contribution,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: null);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Below minimum contribution");

            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<FundOrder>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Immediate_Redemption_Has_Insufficient_Position()
        {
            // Arrange
            var customer = new Customer(
                "Bruno Ferreira",
                "12345678901",
                10000m);

            var fund = new InvestmentFund(
                "Itaú Asset RF",
                new TimeOnly(15, 00),
                10m,
                100m,
                50m,
                1_000_000m,
                FundStatus.Open);

            var command = new CreateFundOrderCommand(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Redemption,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: null);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            _customerRepositoryMock
                .Setup(x => x.GetFundPositionAsync(
                    command.CustomerId,
                    command.InvestmentFundId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((CustomerFundPosition?)null);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Insufficient position");

            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<FundOrder>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _mediatorMock.Verify(
                x => x.Send(It.IsAny<ExecuteFundOrderCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Scheduled_Date_Is_Today()
        {
            // Arrange
            var customer = new Customer(
                "Bruno Ferreira",
                "12345678901",
                10000m);

            var fund = new InvestmentFund(
                "Itaú Asset RF",
                new TimeOnly(15, 00),
                10m,
                100m,
                50m,
                1_000_000m,
                FundStatus.Open);

            var command = new CreateFundOrderCommand(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Contribution,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: DateOnly.FromDateTime(DateTime.Today));

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Scheduled date must be a future business day");

            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<FundOrder>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Scheduled_Date_Is_Weekend()
        {
            // Arrange
            var customer = new Customer(
                "Bruno Ferreira",
                "12345678901",
                10000m);

            var fund = new InvestmentFund(
                "Itaú Asset RF",
                new TimeOnly(15, 00),
                10m,
                100m,
                50m,
                1_000_000m,
                FundStatus.Open);

            var command = new CreateFundOrderCommand(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Contribution,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: GetNextSaturday());

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(command.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _fundRepositoryMock
                .Setup(x => x.GetByIdAsync(command.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Scheduled date cannot be weekend");

            _fundOrderRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<FundOrder>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private static CreateFundOrderCommand CreateValidImmediateContributionCommand()
        {
            return new CreateFundOrderCommand(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Contribution,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: null);
        }

        private static DateOnly GetNextBusinessDay()
        {
            var date = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

            while (date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        private static DateOnly GetNextSaturday()
        {
            var date = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

            while (date.DayOfWeek != DayOfWeek.Saturday)
            {
                date = date.AddDays(1);
            }

            return date;
        }
    }
}