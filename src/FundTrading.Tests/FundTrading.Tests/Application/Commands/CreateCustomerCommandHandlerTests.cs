using FundTrading.Application.Customers.Commands;
using FundTrading.Domain.Entities;
using FundTrading.Domain.Repository;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;

namespace FundTrading.Tests.Application.Customers
{
    public class CreateCustomerCommandHandlerTests
    {
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CreateCustomerCommandHandler>> _loggerMock;

        private readonly CreateCustomerCommandHandler _handler;

        public CreateCustomerCommandHandlerTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CreateCustomerCommandHandler>>();

            _customerRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new CreateCustomerCommandHandler(
                _customerRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Should_Create_Customer_When_Data_Is_Valid()
        {
            // Arrange
            var command = new CreateCustomerCommand(
                "Bruno Ferreira",
                "bruno@email.com",
                10000);

            _customerRepositoryMock
                .Setup(x => x.GetByDocumentAsync(
                    command.Document,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var customerId = await _handler.Handle(
                command,
                CancellationToken.None);

            // Assert
            customerId.Should().BeGreaterThan(0);

            _customerRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Customer>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.Commit(),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Document_Already_Exists()
        {
            // Arrange
            var command = new CreateCustomerCommand(
                "Bruno Ferreira",
                "458855855",
                10000);

            var existingCustomer = new Customer(
                "Existing Customer",
                "458855855",
                5000);

            _customerRepositoryMock
                .Setup(x => x.GetByDocumentAsync(
                    command.Document,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingCustomer);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(
                    command,
                    CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Customer email already exists");

            _customerRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Customer>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.Commit(),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_Exception_When_Initial_Balance_Is_Negative()
        {
            // Arrange
            var command = new CreateCustomerCommand(
                "Bruno Ferreira",
                "45852662558",
                -100);

            _customerRepositoryMock
                .Setup(x => x.GetByDocumentAsync(
                    command.Document,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Customer?)null);

            // Act
            Func<Task> act = async () =>
                await _handler.Handle(
                    command,
                    CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<Exception>()
                .WithMessage("Initial balance cannot be negative");

            _customerRepositoryMock.Verify(
                x => x.AddAsync(
                    It.IsAny<Customer>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.Commit(),
                Times.Never);
        }
    }
}