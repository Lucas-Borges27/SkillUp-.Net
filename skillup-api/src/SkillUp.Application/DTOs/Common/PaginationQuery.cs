namespace SkillUp.Application.DTOs.Common;

public record PaginationQuery
{
    private const int MaxPageSize = 100;

    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string? SortBy { get; init; } = null;
    public string SortDirection { get; init; } = "asc";
    public string? Filter { get; init; }

    public int NormalizePage() => Page < 1 ? 1 : Page;
    public int NormalizeSize()
    {
        if (Size < 1) return 10;
        return Size > MaxPageSize ? MaxPageSize : Size;
    }
}
