-- AccessToBlazer - SQL Server schema and seed data
--
-- Generated from the EF Core migration, not written by hand:
--
--   dotnet ef migrations script --project src/AccessToBlazer.Web \
--     --context SampleDbContext --output docs/schema.sql
--
-- This mirrors the three Access tables one-for-one:
--   Products   4 columns  + IDENTITY(1,1) on ProductID   (Access COUNTER)
--   Customers  4 columns  + IDENTITY(1,1) on CustomerID  (Access COUNTER)
--   Orders     5 columns  + 2 enforced foreign keys
--
-- The FKs use ON DELETE NO ACTION, which is EF's DeleteBehavior.Restrict.
-- That is why the app refuses to delete a product or customer that an order
-- still references, and why the seed data gives every product and customer
-- at least one order.
--
-- This is a PLAIN script, not an idempotent one: it creates the objects
-- directly and will fail if run twice. Regenerate it rather than editing it.
--
-- To apply it by hand (the app does all of this for you on first run):
--
--   sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "CREATE DATABASE AccessToBlazerSample;" -C
--   sqlcmd -S "(localdb)\MSSQLLocalDB" -d AccessToBlazerSample -i docs/schema.sql -C
--
-- The seed rows use SET IDENTITY_INSERT so the ids match the .accdb exactly,
-- which is what lets you compare the Access file and the running app.

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
CREATE TABLE [Customers] (
    [CustomerId] int NOT NULL IDENTITY,
    [CustomerName] nvarchar(80) NOT NULL,
    [City] nvarchar(60) NULL,
    [Phone] nvarchar(30) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
);

CREATE TABLE [Products] (
    [ProductId] int NOT NULL IDENTITY,
    [ProductName] nvarchar(80) NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    [UnitsInStock] int NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([ProductId])
);

CREATE TABLE [Orders] (
    [OrderId] int NOT NULL IDENTITY,
    [CustomerId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [OrderDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([OrderId]),
    CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([ProductId]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CustomerId', N'City', N'CustomerName', N'Phone') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] ON;
INSERT INTO [Customers] ([CustomerId], [City], [CustomerName], [Phone])
VALUES (1, N'Springfield', N'Acme Corp', N'555-0101'),
(2, N'Shelbyville', N'Globex Inc', N'555-0142'),
(3, N'Austin', N'Initech', N'555-0177'),
(4, N'Palo Alto', N'Hooli', N'555-0113'),
(5, N'New York', N'Stark Industries', N'555-0190');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CustomerId', N'City', N'CustomerName', N'Phone') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ProductId', N'ProductName', N'UnitPrice', N'UnitsInStock') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([ProductId], [ProductName], [UnitPrice], [UnitsInStock])
VALUES (1, N'Widget A', 9.99, 120),
(2, N'Widget B', 14.5, 85),
(3, N'Gadget X', 39.95, 40),
(4, N'Gizmo Basic', 5.25, 300),
(5, N'Gizmo Pro', 19.99, 150),
(6, N'Turbo Chip', 79.0, 12);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'ProductId', N'ProductName', N'UnitPrice', N'UnitsInStock') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrderId', N'CustomerId', N'OrderDate', N'ProductId', N'Quantity') AND [object_id] = OBJECT_ID(N'[Orders]'))
    SET IDENTITY_INSERT [Orders] ON;
INSERT INTO [Orders] ([OrderId], [CustomerId], [OrderDate], [ProductId], [Quantity])
VALUES (1, 1, '2026-01-05T00:00:00.0000000', 1, 10),
(2, 1, '2026-01-18T00:00:00.0000000', 4, 25),
(3, 2, '2026-02-02T00:00:00.0000000', 3, 2),
(4, 3, '2026-02-14T00:00:00.0000000', 5, 8),
(5, 4, '2026-03-01T00:00:00.0000000', 6, 1),
(6, 5, '2026-03-22T00:00:00.0000000', 2, 12);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'OrderId', N'CustomerId', N'OrderDate', N'ProductId', N'Quantity') AND [object_id] = OBJECT_ID(N'[Orders]'))
    SET IDENTITY_INSERT [Orders] OFF;

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);

CREATE INDEX [IX_Orders_ProductId] ON [Orders] ([ProductId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260929003155_InitialCreate', N'10.0.12');

COMMIT;
GO

