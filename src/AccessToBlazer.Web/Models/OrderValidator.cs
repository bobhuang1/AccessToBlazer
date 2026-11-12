namespace AccessToBlazer.Web.Models;

/// <summary>
/// Server-side business rules for an order that the data annotations cannot
/// express (they depend on the product's stock and on today's date).
/// </summary>
public static class OrderValidator
{
    public static readonly DateTime MinOrderDate = new(2000, 1, 1);

    public static IReadOnlyList<string> Validate(int quantity, DateTime orderDate, int unitsInStock, DateTime today)
    {
        var errors = new List<string>();

        if (quantity <= 0)
        {
            errors.Add("Quantity must be at least 1.");
        }
        else if (quantity > unitsInStock)
        {
            errors.Add($"Quantity {quantity} exceeds the {unitsInStock} unit(s) in stock for this product.");
        }

        if (orderDate.Date < MinOrderDate)
        {
            errors.Add($"Order date cannot be before {MinOrderDate:yyyy-MM-dd}.");
        }
        else if (orderDate.Date > today.Date)
        {
            errors.Add($"Order date cannot be after today ({today:yyyy-MM-dd}).");
        }

        return errors;
    }
}
