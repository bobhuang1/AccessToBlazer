using System.ComponentModel.DataAnnotations;

namespace AccessToBlazer.Web.Models;

/// <summary>
/// A customer order line. Ported from the Access <c>Orders</c> table, where the
/// CustomerID and ProductID columns were combo-box lookups.
/// </summary>
public class Order
{
    public int OrderId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Select a customer.")]
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Select a product.")]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Range(1, 1_000_000)]
    public int Quantity { get; set; }

    public DateTime OrderDate { get; set; }
}