IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE TABLE [Customers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Document] nvarchar(11) NOT NULL,
        [AvailableBalance] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedBy] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE TABLE [InvestmentFunds] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [CutoffTime] time NOT NULL,
        [SharePrice] decimal(18,2) NOT NULL,
        [MinimumContributionAmount] decimal(18,2) NOT NULL,
        [MinimumRemainingBalance] decimal(18,2) NOT NULL,
        [CurrentCapacity] decimal(18,2) NOT NULL,
        [CapacityLimit] decimal(18,2) NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedBy] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_InvestmentFunds] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE TABLE [CustomerFundPositions] (
        [Id] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [InvestmentFundId] int NOT NULL,
        [ShareQuantity] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedBy] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_CustomerFundPositions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CustomerFundPositions_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CustomerFundPositions_InvestmentFunds_InvestmentFundId] FOREIGN KEY ([InvestmentFundId]) REFERENCES [InvestmentFunds] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE TABLE [FundOrders] (
        [Id] int NOT NULL IDENTITY,
        [CustomerId] int NOT NULL,
        [InvestmentFundId] int NOT NULL,
        [OperationType] nvarchar(30) NOT NULL,
        [ShareQuantity] int NOT NULL,
        [SharePrice] decimal(18,2) NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [ScheduledDate] date NULL,
        [Status] nvarchar(30) NOT NULL,
        [RejectionReason] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [CreatedBy] nvarchar(max) NOT NULL,
        [UpdatedBy] nvarchar(max) NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_FundOrders] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FundOrders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FundOrders_InvestmentFunds_InvestmentFundId] FOREIGN KEY ([InvestmentFundId]) REFERENCES [InvestmentFunds] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_CustomerFundPositions_CustomerId_InvestmentFundId] ON [CustomerFundPositions] ([CustomerId], [InvestmentFundId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE INDEX [IX_CustomerFundPositions_InvestmentFundId] ON [CustomerFundPositions] ([InvestmentFundId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Customers_Document] ON [Customers] ([Document]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE INDEX [IX_FundOrders_CustomerId] ON [FundOrders] ([CustomerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    CREATE INDEX [IX_FundOrders_InvestmentFundId] ON [FundOrders] ([InvestmentFundId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260517195152_SetupInicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260517195152_SetupInicial', N'8.0.27');
END;
GO

COMMIT;
GO

