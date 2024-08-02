namespace ReportBot.Common.Responses;

public class PageList<T>
{
    public List<T> Items { get; set; } = null!;
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}