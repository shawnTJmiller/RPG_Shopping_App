namespace GurpsShoppingApp.Models;

public sealed class DatabaseMenuItem
{
    public int Id { get; init; }

    public string MenuName { get; init; } = string.Empty;

    public string Label { get; init; } = string.Empty;

    public string? TargetScreen { get; init; }

    public bool IsExit { get; init; }

    public int SortOrder { get; init; }
}
