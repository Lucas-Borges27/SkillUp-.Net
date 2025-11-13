namespace SkillUp.Application.DTOs.Common;

public class ResourceResponse<T>
{
    public T Data { get; }
    public IReadOnlyCollection<LinkResponse> Links { get; }

    public ResourceResponse(T data, IEnumerable<LinkResponse> links)
    {
        Data = data;
        Links = links.ToList();
    }
}
