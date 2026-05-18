using FundTrading.Application.Orders.Commands;
using FundTrading.Domain.Entities;
using FundTrading.Domain.Enums;
using FundTrading.Domain.Repository;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace FundTrading.Tests.Application.Orders.Commands
{
    public class ExecuteFundOrderCommandHandlerTests
    {
        private readonly Mock<IFundOrderRepository> _fundOrderRepositoryMock;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IInvestmentFundRepository> _investmentFundRepositoryMock;
        private readonly Mock<ICustomerFundPositionRepository> _positionRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ExecuteFundOrderCommandHandler>> _loggerMock;

        private readonly ExecuteFundOrderCommandHandler _handler;

        public ExecuteFundOrderCommandHandlerTests()
        {
            _fundOrderRepositoryMock = new Mock<IFundOrderRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _investmentFundRepositoryMock = new Mock<IInvestmentFundRepository>();
            _positionRepositoryMock = new Mock<ICustomerFundPositionRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ExecuteFundOrderCommandHandler>>();

            _fundOrderRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            _handler = new ExecuteFundOrderCommandHandler(
                _fundOrderRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _investmentFundRepositoryMock.Object,
                _positionRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Should_Execute_Contribution_Order_When_Data_Is_Valid_And_Position_Does_Not_Exist()
        {
            var order = CreateContributionOrder();
            var customer = CreateCustomer(availableBalance: 10000m);
            var fund = CreateFund();

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(order.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _investmentFundRepositoryMock
                .Setup(x => x.GetByIdAsync(order.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            _positionRepositoryMock
                .Setup(x => x.GetByCustomerAndFundAsync(customer.Id, fund.Id))
                .ReturnsAsync((CustomerFundPosition?)null);

            await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            order.Status.Should().Be(OrderStatus.Processed);
            customer.AvailableBalance.Should().Be(9800m);
            fund.CurrentCapacity.Should().Be(200m);

            _positionRepositoryMock.Verify(
                x => x.AddAsync(It.Is<CustomerFundPosition>(p =>
                    p.CustomerId == customer.Id &&
                    p.InvestmentFundId == fund.Id &&
                    p.ShareQuantity == order.ShareQuantity)),
                Times.Once);

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Should_Execute_Contribution_Order_When_Position_Already_Exists()
        {
            var order = CreateContributionOrder();
            var customer = CreateCustomer(availableBalance: 10000m);
            var fund = CreateFund();
            var position = new CustomerFundPosition(customer.Id, fund.Id, 10);

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(order.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _investmentFundRepositoryMock
                .Setup(x => x.GetByIdAsync(order.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            _positionRepositoryMock
                .Setup(x => x.GetByCustomerAndFundAsync(customer.Id, fund.Id))
                .ReturnsAsync(position);

            await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            order.Status.Should().Be(OrderStatus.Processed);
            position.ShareQuantity.Should().Be(30);
            customer.AvailableBalance.Should().Be(9800m);

            _positionRepositoryMock.Verify(
                x => x.AddAsync(It.IsAny<CustomerFundPosition>()),
                Times.Never);

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Should_Reject_Contribution_Order_When_Customer_Has_Insufficient_Balance()
        {
            var order = CreateContributionOrder();
            var customer = CreateCustomer(availableBalance: 100m);
            var fund = CreateFund();

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(order.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _investmentFundRepositoryMock
                .Setup(x => x.GetByIdAsync(order.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            Func<Task> act = async () =>
                await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Insufficient balance.");

            order.Status.Should().Be(OrderStatus.Rejected);
            order.RejectionReason.Should().Be("Insufficient balance.");

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Should_Reject_Contribution_Order_When_Fund_Capacity_Is_Exceeded()
        {
            var order = CreateContributionOrder();
            var customer = CreateCustomer(availableBalance: 10000m);
            var fund = CreateFund(currentCapacity: 999950m, capacityLimit: 1_000_000m);

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(order.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _investmentFundRepositoryMock
                .Setup(x => x.GetByIdAsync(order.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            Func<Task> act = async () =>
                await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Fund capacity exceeded.");

            order.Status.Should().Be(OrderStatus.Rejected);
            order.RejectionReason.Should().Be("Fund capacity exceeded.");

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Should_Execute_Redemption_Order_When_Position_Is_Sufficient()
        {
            var order = CreateRedemptionOrder();
            var customer = CreateCustomer(availableBalance: 1000m);
            var fund = CreateFund(currentCapacity: 1000m);
            var position = new CustomerFundPosition(customer.Id, fund.Id, 50);

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(order.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _investmentFundRepositoryMock
                .Setup(x => x.GetByIdAsync(order.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            _positionRepositoryMock
                .Setup(x => x.GetByCustomerAndFundAsync(customer.Id, fund.Id))
                .ReturnsAsync(position);

            await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            order.Status.Should().Be(OrderStatus.Processed);
            position.ShareQuantity.Should().Be(30);
            customer.AvailableBalance.Should().Be(1200m);
            fund.CurrentCapacity.Should().Be(800m);

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Should_Reject_Redemption_Order_When_Position_Does_Not_Exist()
        {
            var order = CreateRedemptionOrder();
            var customer = CreateCustomer(availableBalance: 1000m);
            var fund = CreateFund();

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(order.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _investmentFundRepositoryMock
                .Setup(x => x.GetByIdAsync(order.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fund);

            _positionRepositoryMock
                .Setup(x => x.GetByCustomerAndFundAsync(customer.Id, fund.Id))
                .ReturnsAsync((CustomerFundPosition?)null);

            Func<Task> act = async () =>
                await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Insufficient shares.");

            order.Status.Should().Be(OrderStatus.Rejected);
            order.RejectionReason.Should().Be("Insufficient shares.");

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Order_Not_Found()
        {
            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((FundOrder?)null);

            Func<Task> act = async () =>
                await _handler.Handle(new ExecuteFundOrderCommand(1), CancellationToken.None);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Order not found.");

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Order_Status_Is_Invalid()
        {
            var order = CreateContributionOrder();
            order.MarkAsExecuted();

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            Func<Task> act = async () =>
                await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage($"Order {order.Id} cannot be executed because status is {order.Status}");

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Customer_Not_Found()
        {
            var order = CreateContributionOrder();

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(order.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            Func<Task> act = async () =>
                await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Customer not found.");

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Fund_Not_Found()
        {
            var order = CreateContributionOrder();
            var customer = CreateCustomer(10000m);

            _fundOrderRepositoryMock
                .Setup(x => x.GetByIdAsync(order.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(order);

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(order.CustomerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(customer);

            _investmentFundRepositoryMock
                .Setup(x => x.GetByIdAsync(order.InvestmentFundId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((InvestmentFund?)null);

            Func<Task> act = async () =>
                await _handler.Handle(new ExecuteFundOrderCommand(order.Id), CancellationToken.None);

            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Investment fund not found.");

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        private static Customer CreateCustomer(decimal availableBalance)
        {
            return new Customer(
                "Bruno Ferreira",
                "12345678901",
                availableBalance);
        }

        private static InvestmentFund CreateFund(
            decimal currentCapacity = 0,
            decimal capacityLimit = 1_000_000m)
        {
            var fund = new InvestmentFund(
                "Itaú Asset RF",
                new TimeOnly(15, 00),
                10m,
                100m,
                50m,
                capacityLimit,
                FundStatus.Open);

            if (currentCapacity > 0)
                fund.AddCapacity(currentCapacity);

            return fund;
        }

        private static FundOrder CreateContributionOrder()
        {
            return new FundOrder(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Contribution,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: null,
                status: OrderStatus.Pending);
        }

        private static FundOrder CreateRedemptionOrder()
        {
            return new FundOrder(
                customerId: 1,
                investmentFundId: 1,
                operationType: OperationType.Redemption,
                shareQuantity: 20,
                sharePrice: 10m,
                scheduledDate: null,
                status: OrderStatus.Pending);
        }
    }
}