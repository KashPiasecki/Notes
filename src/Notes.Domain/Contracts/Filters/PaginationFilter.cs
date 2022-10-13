namespace Notes.Domain.Contracts.Filters;

public class PaginationFilter
{
    public int PageNumber { get; init; } 
    public int PageSize { get; init; }
}