namespace Notes.Domain.Contracts.Responses;

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

    public PagedResponse(IEnumerable<T> data, int pageNumber, int pageSize)
    {
        Data = data;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}