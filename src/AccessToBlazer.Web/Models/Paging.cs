namespace AccessToBlazer.Web.Models;

/// <summary>Server-side paging helpers shared by the list pages.</summary>
public static class Paging
{
    public const int DefaultPageSize = 10;

    public static int TotalPages(int totalCount, int pageSize = DefaultPageSize)
        => totalCount <= 0 ? 1 : (int)Math.Ceiling(totalCount / (double)pageSize);

    /// <summary>Keeps a 1-based page number inside 1..TotalPages (e.g. after the last row of a page is deleted).</summary>
    public static int ClampPage(int page, int totalCount, int pageSize = DefaultPageSize)
        => Math.Clamp(page, 1, TotalPages(totalCount, pageSize));

    /// <summary>Applies Skip/Take for a 1-based page; the query must already be ordered.</summary>
    public static IQueryable<T> ForPage<T>(this IQueryable<T> ordered, int page, int pageSize = DefaultPageSize)
        => ordered.Skip((Math.Max(page, 1) - 1) * pageSize).Take(pageSize);
}
