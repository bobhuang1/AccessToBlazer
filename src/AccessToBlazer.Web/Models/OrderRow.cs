namespace AccessToBlazer.Web.Models;

/// <summary>
/// Read-only projection of an order for the Orders list: just the columns the
/// grid shows plus the ids and row version the editor needs.
/// </summary>
public record OrderRow(
    int OrderId,
    int CustomerId,
    string CustomerName,
    int ProductId,
    string ProductName,
    int Quantity,
    DateTime OrderDate,
    byte[]? RowVersion);
