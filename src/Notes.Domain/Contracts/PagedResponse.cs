namespace Notes.Domain.Contracts;

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; set; }
    public int TotalRecords { get; set; }
    public Uri NextPage { get; set; }
    public Uri PreviousPage { get; set; }
    public Uri FirstPage { get; set; }
    public Uri LastPage { get; set; }

    public PagedResponse(IEnumerable<T> data, int pageSize, int pageNumber)
    {
        Data = data;
        PageSize = pageSize;
        PageNumber = pageNumber;
    }
}