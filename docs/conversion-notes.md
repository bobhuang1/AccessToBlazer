# Conversion notes: Access → Blazor

How each concept in `AccessToBlazerSample.accdb` maps to the ported Blazor app.

## Objects

| Access object | Blazor equivalent | Where |
| --- | --- | --- |
| Table `Products` | EF Core entity `Product` + SQL table `Products` | `src/AccessToBlazer.Web/Models/Product.cs` |
| Table `Customers` | EF Core entity `Customer` + SQL table `Customers` | `src/AccessToBlazer.Web/Models/Customer.cs` |
| Table `Orders` | EF Core entity `Order` + SQL table `Orders` | `src/AccessToBlazer.Web/Models/Order.cs` |
| Form `Products` | Razor page `/products` | `src/AccessToBlazer.Web/Components/Pages/Products.razor` |
| Form `Customers` | Razor page `/customers` | `src/AccessToBlazer.Web/Components/Pages/Customers.razor` |
| Form `Orders` | Razor page `/orders` | `src/AccessToBlazer.Web/Components/Pages/Orders.razor` |

There is deliberately **no authentication** in the sample, so there is no
Access login form to port and no auth middleware to configure.

## Data types

| Access | SQL Server (EF Core) | C# |
| --- | --- | --- |
| `COUNTER` (AutoNumber) | `int` + `IDENTITY(1,1)` | `int` with `[Key]` |
| `TEXT(n)` | `nvarchar(n)` | `string` |
| `CURRENCY` | `decimal(18,2)` | `decimal` |
| `INTEGER` (Long) | `int` | `int` |
| `DATETIME` | `datetime2` | `DateTime` |
| Memo / Long Text | `nvarchar(max)` | `string` |

The `COUNTER` → `IDENTITY` conversion is the one that matters: Access assigns the
autonumber inside the form, so the original `ProductID` is only kept because the
sample was seeded with explicit ids in `HasData`. New rows get the database to
assign the id.

## UI idioms

| Access | Blazor |
| --- | --- |
| Form `RecordSource` | EF query in `OnInitializedAsync`, results bound to a `<table>` |
| Text box bound to a column | `<InputText>` / `<InputNumber>` / `<InputDate>` |
| Combo box (row source = another table) | `<InputSelect>` with an `<option>` per lookup row |
| Command button on a form | `<button @onclick="...">` calling an async method |
| Form-level validation | `[Required]` / `[Range]` DataAnnotations + `<ValidationMessage>` |
| Subform | A second `<table>` on the same page, joined with `.Include(...)` |

The Access `Orders` form is the interesting one: `CustomerID` and `ProductID`
were combo boxes whose row source was a `SELECT` over another table. The port
loads both lookup lists and renders them as `<select>` elements, then displays
the joined names in the orders table.

## Data access

There is no global state and no `DbContext` kept alive between events. Blazor
Server components are long-lived, so a single shared `DbContext` would leak the
change tracker across users. Instead the app registers an
`IDbContextFactory<SampleDbContext>` and each operation creates a short-lived
context:

```csharp
await using var db = await DbFactory.CreateDbContextAsync();
```

## Running against LocalDB

The app creates and seeds its own database on startup, so there is nothing to
set up beyond having SQL Server Express LocalDB installed:

```powershell
dotnet run
```

To reset the data, drop the database and start again:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "DROP DATABASE AccessToBlazerSample;" -C
```

## Rebuilding the Access file

`tools/New-SampleAccessDb.ps1` regenerates `docs/AccessToBlazerSample.accdb`
from scratch (tables, seed rows and the three bound forms) using Access
automation. It requires desktop Microsoft Access and is only needed if you want
to change the sample; the committed `.accdb` is ready to use as-is.
