using System.ComponentModel.DataAnnotations;

namespace AccessToBlazer.Web.Models;

/// <summary>
/// A sellable product. Ported from the Access <c>Products</c> table.
/// </summary>
public class Product
{
    public int ProductId { get; set; }

    [Required, StringLength(80)]
    public string ProductName { get; set; } = string.Empty;

    [Range(0, 1_000_000)]
    public decimal UnitPrice { get; set; }

    [Range(0, 1_000_000)]
    public int UnitsInStock { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}