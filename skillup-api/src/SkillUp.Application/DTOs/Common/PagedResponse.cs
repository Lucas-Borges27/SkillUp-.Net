namespace SkillUp.Application.DTOs.Common;

public record LinkResponse(string Rel, string Href, string Method = "GET");

public record PaginationMetadata(int Page, int Size, int TotalPages, long TotalItems);

public class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public PaginationMetadata Metadata { get; }
    public IReadOnlyCollection<LinkResponse> Links { get; }

    public PagedResponse(IEnumerable<T> items, PaginationMetadata metadata, IEnumerable<LinkResponse> links)
    {
        Items = items.ToList();
        Metadata = metadata;
        Links = links.ToList();
    }
}
