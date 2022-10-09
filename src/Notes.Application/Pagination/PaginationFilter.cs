namespace Notes.Application.Pagination;

public class PaginationFilter
{
    public int PageNumber { get; init; } 
    public int PageSize { get; init; }

    public PaginationFilter(int pageSize, int pageNumber)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize is < 1 or > 10 
            ? 10 
            : pageSize;
    }
}