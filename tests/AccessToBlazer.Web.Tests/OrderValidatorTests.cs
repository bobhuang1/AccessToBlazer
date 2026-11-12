using AccessToBlazer.Web.Models;

namespace AccessToBlazer.Web.Tests;

public class OrderValidatorTests
{
    private static readonly DateTime Today = new(2026, 11, 11, 15, 30, 0);

    [Fact]
    public void Valid_order_has_no_errors()
        => Assert.Empty(OrderValidator.Validate(5, new DateTime(2026, 11, 1), unitsInStock: 5, Today));

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Non_positive_quantity_is_rejected(int quantity)
        => Assert.Contains("Quantity must be at least 1.",
            OrderValidator.Validate(quantity, Today, unitsInStock: 10, Today));

    [Fact]
    public void Quantity_above_stock_is_rejected_with_the_stock_in_the_message()
    {
        var errors = OrderValidator.Validate(11, Today, unitsInStock: 10, Today);
        var error = Assert.Single(errors);
        Assert.Contains("11", error);
        Assert.Contains("10", error);
    }

    [Fact]
    public void Any_quantity_is_rejected_when_out_of_stock()
        => Assert.Single(OrderValidator.Validate(1, Today, unitsInStock: 0, Today));

    [Fact]
    public void Today_is_allowed_even_with_a_time_component()
        => Assert.Empty(OrderValidator.Validate(1, Today.Date, 1, Today));

    [Fact]
    public void Future_date_is_rejected()
        => Assert.Single(OrderValidator.Validate(1, Today.Date.AddDays(1), 1, Today));

    [Fact]
    public void Earliest_allowed_date_is_accepted_and_the_day_before_is_not()
    {
        Assert.Empty(OrderValidator.Validate(1, new DateTime(2000, 1, 1), 1, Today));
        Assert.Single(OrderValidator.Validate(1, new DateTime(1999, 12, 31), 1, Today));
    }

    [Fact]
    public void Quantity_and_date_errors_are_both_reported()
        => Assert.Equal(2, OrderValidator.Validate(0, new DateTime(1990, 1, 1), 1, Today).Count);
}
