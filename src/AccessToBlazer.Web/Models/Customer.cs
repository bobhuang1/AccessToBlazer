using System.ComponentModel.DataAnnotations;

namespace AccessToBlazer.Web.Models;

/// <summary>
/// A customer who places orders. Ported from the Access <c>Customers</c> table.
/// </summary>
public class Customer
{
    public int CustomerId { get; set; }

    [Required, StringLength(80)]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(60)]
    public string? City { get; set; }

    [StringLength(30)]
    public string? Phone { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}