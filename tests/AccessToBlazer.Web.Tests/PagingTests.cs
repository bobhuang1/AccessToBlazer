using AccessToBlazer.Web.Models;

namespace AccessToBlazer.Web.Tests;

public class PagingTests
{
    [Theory]
    [InlineData(0, 10, 1)]
    [InlineData(1, 10, 1)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(95, 10, 10)]
    public void TotalPages_rounds_up_and_is_at_least_one(int total, int size, int expected)
        => Assert.Equal(expected, Paging.TotalPages(total, size));

    [Theory]
    [InlineData(0, 25, 1)]   // unset page
    [InlineData(-4, 25, 1)]
    [InlineData(2, 25, 2)]
    [InlineData(9, 25, 3)]   // last row of the last page deleted
    [InlineData(5, 0, 1)]
    public void ClampPage_keeps_the_page_in_range(int page, int total, int expected)
        => Assert.Equal(expected, Paging.ClampPage(page, total, 10));

    [Fact]
    public void ForPage_returns_the_requested_slice_in_order()
    {
        var rows = Enumerable.Range(1, 25).AsQueryable();
        Assert.Equal(Enumerable.Range(1, 10), rows.ForPage(1, 10));
        Assert.Equal(Enumerable.Range(11, 10), rows.ForPage(2, 10));
        Assert.Equal(Enumerable.Range(21, 5), rows.ForPage(3, 10));
        Assert.Empty(rows.ForPage(4, 10));
    }

    [Fact]
    public void ForPage_treats_page_zero_as_the_first_page()
        => Assert.Equal(Enumerable.Range(1, 10), Enumerable.Range(1, 25).AsQueryable().ForPage(0, 10));
}
