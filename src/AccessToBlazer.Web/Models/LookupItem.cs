namespace AccessToBlazer.Web.Models;

/// <summary>Id and display name for a dropdown; avoids loading whole entities.</summary>
public record LookupItem(int Id, string Name);
