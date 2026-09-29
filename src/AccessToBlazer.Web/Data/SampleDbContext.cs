using AccessToBlazer.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AccessToBlazer.Web.Data;

/// <summary>
/// EF Core context for the sample. The three tables mirror the Access
/// <c>Products</c>, <c>Customers</c> and <c>Orders</c> tables one-for-one.
/// </summary>
public class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(p => p.ProductId);
            e.Property(p => p.ProductName).IsRequired().HasMaxLength(80);
            e.Property(p => p.UnitPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.HasKey(c => c.CustomerId);
            e.Property(c => c.CustomerName).IsRequired().HasMaxLength(80);
            e.Property(c => c.City).HasMaxLength(60);
            e.Property(c => c.Phone).HasMaxLength(30);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.HasKey(o => o.OrderId);
            e.HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// The same rows that ship in <c>docs/AccessToBlazerSample.accdb</c>,
    /// so the ported app starts with identical data.
    /// </summary>
    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, ProductName = "Widget A", UnitPrice = 9.99m, UnitsInStock = 120 },
            new Product { ProductId = 2, ProductName = "Widget B", UnitPrice = 14.50m, UnitsInStock = 85 },
            new Product { ProductId = 3, ProductName = "Gadget X", UnitPrice = 39.95m, UnitsInStock = 40 },
            new Product { ProductId = 4, ProductName = "Gizmo Basic", UnitPrice = 5.25m, UnitsInStock = 300 },
            new Product { ProductId = 5, ProductName = "Gizmo Pro", UnitPrice = 19.99m, UnitsInStock = 150 },
            new Product { ProductId = 6, ProductName = "Turbo Chip", UnitPrice = 79.00m, UnitsInStock = 12 });

        modelBuilder.Entity<Customer>().HasData(
            new Customer { CustomerId = 1, CustomerName = "Acme Corp", City = "Springfield", Phone = "555-0101" },
            new Customer { CustomerId = 2, CustomerName = "Globex Inc", City = "Shelbyville", Phone = "555-0142" },
            new Customer { CustomerId = 3, CustomerName = "Initech", City = "Austin", Phone = "555-0177" },
            new Customer { CustomerId = 4, CustomerName = "Hooli", City = "Palo Alto", Phone = "555-0113" },
            new Customer { CustomerId = 5, CustomerName = "Stark Industries", City = "New York", Phone = "555-0190" });

        modelBuilder.Entity<Order>().HasData(
            new Order { OrderId = 1, CustomerId = 1, ProductId = 1, Quantity = 10, OrderDate = new DateTime(2026, 1, 5) },
            new Order { OrderId = 2, CustomerId = 1, ProductId = 4, Quantity = 25, OrderDate = new DateTime(2026, 1, 18) },
            new Order { OrderId = 3, CustomerId = 2, ProductId = 3, Quantity = 2, OrderDate = new DateTime(2026, 2, 2) },
            new Order { OrderId = 4, CustomerId = 3, ProductId = 5, Quantity = 8, OrderDate = new DateTime(2026, 2, 14) },
            new Order { OrderId = 5, CustomerId = 4, ProductId = 6, Quantity = 1, OrderDate = new DateTime(2026, 3, 1) },
            new Order { OrderId = 6, CustomerId = 5, ProductId = 2, Quantity = 12, OrderDate = new DateTime(2026, 3, 22) });
    }
}