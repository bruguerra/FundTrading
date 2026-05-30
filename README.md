# Fund Trading API

## Overview

Fund Trading API is a backend application designed to manage investment fund operations, including subscriptions, redemptions, customer positions, and scheduled order processing.

The project was built with a strong focus on:

- Clean Architecture
- Separation of Concerns
- Scalability
- Maintainability
- Observability
- Testability
- Backend development best practices using .NET

The application supports:

- Customer management
- Investment fund management
- Subscription and redemption orders
- Scheduled order execution
- Customer share position tracking

---

## Key Features

- Clean Architecture
- CQRS with MediatR
- Entity Framework Core
- Quartz.NET Scheduled Jobs
- Repository Pattern
- Unit of Work Pattern
- Automatic Auditing
- Structured Logging with Serilog
- Correlation ID Tracking
- Swagger / OpenAPI Documentation
- Unit Testing with xUnit and Moq

---

# Solution Architecture

The solution follows a layered architecture to promote maintainability, testability, and separation of concerns.

```text
FundTrading.API
FundTrading.Application
FundTrading.Domain
FundTrading.Data
```

## FundTrading.API

Responsible for:

- Controllers
- Application configuration
- Middleware registration
- Quartz job configuration
- Swagger setup
- Serilog configuration
- Dependency Injection

## FundTrading.Application

Responsible for:

- Commands
- Command Handlers
- Use Case orchestration
- Application services
- Scheduled job orchestration

CQRS is implemented using MediatR.

## FundTrading.Domain

Responsible for:

- Entities
- Enums
- Interfaces
- Domain rules
- Base abstractions

## FundTrading.Data

Responsible for:

- Entity Framework Core
- DbContext implementation
- Entity mappings
- Repositories
- Data persistence
- Unit of Work implementation

---

# Technology Stack

- .NET 10
- ASP.NET Core
- Entity Framework Core
- SQL Server
- MediatR
- Quartz.NET
- Serilog
- Swagger / OpenAPI
- xUnit
- Moq

---

# Architectural Decisions

## CQRS with MediatR

CQRS was adopted to separate command execution from business rule processing.

Handlers are responsible for orchestrating application use cases and enforcing business workflows.

## Repository Pattern

Repositories abstract data access concerns and centralize persistence operations.

## Unit of Work

The DbContext implements the `IUnitOfWork` interface, providing:

- Transaction management
- Centralized persistence
- Consistent data operations

## Automatic Auditing

The system automatically updates the following fields during the commit process:

- CreatedAt
- UpdatedAt
- CreatedBy
- UpdatedBy

## Asynchronous Processing

Quartz.NET is used to automatically process scheduled orders.

## Observability

The application includes:

- Structured logging with Serilog
- Correlation ID tracking
- Global exception handling middleware
- Log persistence to file

---

# Application Workflow

## Order Creation

```text
Controller
    ↓
CreateFundOrderCommand
    ↓
CreateFundOrderCommandHandler
    ↓
Order Persistence
```

## Immediate Execution

Orders without scheduling are executed immediately.

```text
CreateFundOrderCommandHandler
    ↓
ExecuteFundOrderCommand
    ↓
ExecuteFundOrderCommandHandler
```

## Scheduled Execution

Future orders are processed automatically.

```text
Quartz Job
    ↓
ProcessScheduledOrdersJob
    ↓
ExecuteFundOrderCommand
    ↓
ExecuteFundOrderCommandHandler
```

---

# Business Rules

## Subscription Orders

- Available balance validation
- Fund capacity validation
- Minimum investment amount validation
- Customer position update

## Redemption Orders

- Share position validation
- Position update
- Balance credit processing

## Scheduled Orders

- Past dates are not allowed
- Same-day scheduling is not allowed
- Weekend scheduling is not allowed

## Investment Funds

- Funds must be open for trading

---

# Logging

Serilog is used as the centralized logging solution.

Log entries include:

- Correlation ID
- Structured logging
- File persistence
- Foundation for Microsoft Teams integration

---

# Scheduled Processing

Quartz.NET is responsible for processing scheduled orders automatically.

Current schedule:

- Monday to Friday
- 09:00 AM
- São Paulo Time Zone

---

# Running the Application

## Prerequisites

- .NET 10 SDK
- SQL Server
- Visual Studio 2026 or JetBrains Rider

---

## Database Configuration

Configure the connection string in:

```text
appsettings.Development.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FundTrading;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

---

## Applying Migrations

```bash
dotnet ef database update
```

---

## Running the Application

```bash
dotnet run
```

---

## Swagger

After starting the application:

```text
https://localhost:{port}/swagger
```

---

# Unit Tests

Unit tests were implemented using:

- xUnit
- Moq

Main scenarios covered:

- Successful subscription
- Insufficient balance
- Successful redemption
- Invalid redemption
- Order rejection
- Order execution

---

# Cloud Architecture

![Cloud Architecture](docs/images/cloud-architecture.png)

---

# Database Diagram

![Database Diagram](docs/images/database-diagram.png)

---

# AI-Assisted Development

Artificial Intelligence was used as a productivity and technical assistance tool throughout the development process.

Main use cases included:

- Initial solution architecture structuring
- Architectural discussions and reviews
- Boilerplate code generation
- Unit test support
- Documentation assistance
- Observability and resilience discussions
- Cloud architecture design support

The primary tool used was ChatGPT, acting as a technical copilot to accelerate repetitive tasks and allow greater focus on architectural decisions and business rules.

Despite AI assistance, all business rules, architectural decisions, code reviews, solution modeling, and implementation adjustments were manually validated and implemented.

---

# Roadmap

Potential future improvements:

- Retry policies with Polly
- Distributed caching
- Event Bus integration
- Docker support
- Kubernetes deployment
- Health Checks
- Integration Tests
- Authentication and Authorization
- Outbox Pattern
- Domain Events
- CI/CD Pipeline

---

# Final Considerations

This solution was designed to balance:

- Simplicity
- Maintainability
- Scalability
- Readability
- Software engineering best practices

The goal was to deliver a pragmatic solution while keeping a solid foundation for future evolution in enterprise environments.