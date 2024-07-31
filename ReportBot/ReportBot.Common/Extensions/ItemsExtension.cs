using ReportBot.Common.Responses;

namespace ReportBot.Common.Extensions;

public static class ItemsExtension
{
    public static PageList<T> Pagination<T>(this List<T> items, int page, int pageSize)
    {
        var pageUsers = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var pageList = new PageList<T>()
        {
            TotalCount = items.Count,
            TotalPages = (int)Math.Ceiling((decimal)items.Count / pageSize),
            Items = pageUsers
        };

        return pageList;
    }
}
