using System.Net;
using SkillUp.Application.DTOs.Common;

namespace SkillUp.Application.Services;

public static class PaginationLinkBuilder
{
    public static IReadOnlyCollection<LinkResponse> Build(string resourcePath, PaginationQuery query, int totalPages, IDictionary<string, string?>? extraParameters = null)
    {
        var links = new List<LinkResponse>();
        var page = query.NormalizePage();
        var size = query.NormalizeSize();

        links.Add(new LinkResponse("self", BuildUrl(resourcePath, query, page, size, extraParameters)));
        links.Add(new LinkResponse("first", BuildUrl(resourcePath, query, 1, size, extraParameters)));
        links.Add(new LinkResponse("last", BuildUrl(resourcePath, query, totalPages == 0 ? 1 : totalPages, size, extraParameters)));

        if (page > 1)
        {
            links.Add(new LinkResponse("prev", BuildUrl(resourcePath, query, page - 1, size, extraParameters)));
        }

        if (page < totalPages)
        {
            links.Add(new LinkResponse("next", BuildUrl(resourcePath, query, page + 1, size, extraParameters)));
        }

        return links;
    }

    private static string BuildUrl(string resourcePath, PaginationQuery query, int page, int size, IDictionary<string, string?>? extraParameters)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["page"] = page.ToString(),
            ["size"] = size.ToString(),
            ["sortBy"] = query.SortBy,
            ["sortDirection"] = query.SortDirection,
            ["filter"] = query.Filter
        };

        if (extraParameters != null)
        {
            foreach (var kvp in extraParameters)
            {
                parameters[kvp.Key] = kvp.Value;
            }
        }

        var queryString = string.Join("&", parameters
            .Where(p => !string.IsNullOrWhiteSpace(p.Value))
            .Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));

        return string.IsNullOrWhiteSpace(queryString) ? resourcePath : $"{resourcePath}?{queryString}";
    }
}
