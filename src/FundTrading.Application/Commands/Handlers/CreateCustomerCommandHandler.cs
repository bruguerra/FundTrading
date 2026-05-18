using FundTrading.Domain.Entities;
using FundTrading.Domain.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FundTrading.Application.Customers.Commands
{
    public class CreateCustomerCommandHandler : CommandHandler,
                                                IRequestHandler<CreateCustomerCommand, int>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CreateCustomerCommandHandler> _logger;

        public CreateCustomerCommandHandler(ICustomerRepository customerRepository,
                                            ILogger<CreateCustomerCommandHandler> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        public async Task<int> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating customer with document {Document}", request.Document);

            var existingCustomer = await _customerRepository.GetByDocumentAsync(request.Document, cancellationToken);

            if (existingCustomer is not null)
                throw new Exception("Customer email already exists");

            if (request.AvailableBalance < 0)
                throw new Exception("Initial balance cannot be negative");

            var customer = new Customer(
                request.Name,
                request.Document,
                request.AvailableBalance);

            await _customerRepository.AddAsync(
                customer,
                cancellationToken);

            await SaveDatabase(
                _customerRepository.UnitOfWork);

            return customer.Id;
        }
    }
}