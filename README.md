# AccessToBlazer - a sample Access (MDB/ACCDB) to Blazor Server conversion

A small, complete, self-contained demonstration of porting a Microsoft Access
database to an **ASP.NET Core Blazor Server** application on **.NET 10**, using
**EF Core** against **SQL Server LocalDB**.

The repo ships both halves of the story:

- `docs/AccessToBlazerSample.accdb` - the original Access application (3 tables,
  3 bound forms, no authentication).
- `src/AccessToBlazer.Web` - the ported Blazor Server app that replaces it.

`docs/conversion-notes.md` maps every Access object to its Blazor equivalent,
table by table and control by control.

## What is in the sample

A tiny product catalogue with orders - deliberately generic, and with no
authentication anywhere, so the focus stays on the conversion itself.

| Access form | Access table | Blazor page | Rows |
| --- | --- | --- | --- |
| Products | `Products` | `/products` | 6 |
| Customers | `Customers` | `/customers` | 5 |
| Orders (combo lookups) | `Orders` | `/orders` | 6 |

The seeded data is identical in both halves, so you can open the `.accdb` in
Access and the running Blazor app side by side and see the same six products,
five customers and six orders.

## Features

**Data**

- Three EF Core entities mirroring the Access tables one-for-one
  (`src/AccessToBlazer.Web/Models/`).
- `IDbContextFactory<SampleDbContext>` - a short-lived context per operation, so
  a long-lived Blazor Server circuit never leaks a change tracker between users.
- One EF migration (`InitialCreate`) that creates the schema and inserts the seed
  rows with the same ids the Access file uses.
- The database is created and seeded **automatically on first run** against
  LocalDB; there is no manual setup step.

**UI**

- Blazor Server with interactive render mode (`.razor` components).
- One page per Access form, each with a list view plus an inline
  `EditForm` editor - add, edit and delete without leaving the page.
- The Access combo boxes on the `Orders` form become `<InputSelect>` dropdowns
  populated from the `Customers` and `Products` tables, and the orders list
  shows the joined display names via `.Include(...)`.
- DataAnnotations validation (`[Required]`, `[Range]`) with
  `<ValidationMessage>` in place of Access field-level validation.
- Sidebar navigation, responsive Bootstrap layout, and the standard .NET 10
  Blazor template styling.

## How to run

Requires the **.NET 10 SDK** and **SQL Server Express LocalDB** (both ship with
Visual Studio; LocalDB is also a standalone download from Microsoft).

```bash
git clone https://github.com/bobhuang1/AccessToBlazer.git
cd AccessToBlazer
dotnet run --project src/AccessToBlazer.Web
```

On first start the app creates the `AccessToBlazerSample` database, applies the
migration and seeds it. Open the URL the console prints (by default
<http://localhost:5000>). There is no login.

To reset the data, drop the database and start again:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "DROP DATABASE AccessToBlazerSample;" -C
```

The connection string is `ConnectionStrings:SampleDb` in
`src/AccessToBlazer.Web/appsettings.json`; point it at any SQL Server you like.

## Opening the original Access file

`docs/AccessToBlazerSample.accdb` opens in Microsoft Access with no macros, no
VBA and no login. If desktop Access is not installed, the app itself is enough -
the Access file is only there to show the "before" side of the conversion.

To regenerate it from scratch (requires desktop Access), run
`tools/New-SampleAccessDb.ps1`, which builds the tables, seed data and the three
bound forms by automation.

## Project layout

```
AccessToBlazer/
|-- docs/
|   |-- AccessToBlazerSample.accdb   the original Access app (3 tables, 3 forms)
|   `-- conversion-notes.md          Access -> Blazor mapping, type by type
|-- src/AccessToBlazer.Web/
|   |-- Data/SampleDbContext.cs      EF Core context + seed data
|   |-- Models/                      Product, Customer, Order
|   |-- Migrations/                  InitialCreate
|   `-- Components/Pages/            Home, Products, Customers, Orders
|-- tools/New-SampleAccessDb.ps1     regenerates the .accdb via Access automation
`-- LICENSE                          MIT
```

## Scope and limitations

This is a **sample**, not a template for a production migration. It deliberately
omits the things that make a real conversion hard:

- No authentication or authorization.
- No VBA, no macros, no event-driven form logic to translate.
- No lookup/report queries, no subforms beyond a simple join.
- No concurrent-user testing, no deployment/IIS configuration.

Real conversions add those one at a time, on top of exactly this structure.

## License

MIT - see [LICENSE](LICENSE).
