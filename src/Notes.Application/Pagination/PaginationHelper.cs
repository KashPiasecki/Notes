using Notes.Domain.Contracts;

namespace Notes.Application.Pagination;

public interface IPaginationHelper
{
    PagedResponse<T> CreatePagedReponse<T>(IEnumerable<T> pagedData, PaginationFilter validFilter, int totalRecords, string route);
}

public class PaginationHelper : IPaginationHelper
{
    private readonly IUriService _uriService;

    public PaginationHelper(IUriService uriService)
    {
        _uriService = uriService;
    }
    
    public PagedResponse<T> CreatePagedReponse<T>(IEnumerable<T> pagedData, PaginationFilter validFilter, int totalRecords, string route)
    {
        var response = new PagedResponse<T>(pagedData, validFilter.PageNumber, validFilter.PageSize);
        var totalPages = totalRecords / (double)validFilter.PageSize;
        var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
        response.NextPage =
            validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? _uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber + 1, validFilter.PageSize), route)
                : null!;
        response.PreviousPage =
            validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? _uriService.GetPageUri(new PaginationFilter(validFilter.PageNumber - 1, validFilter.PageSize), route)
                : null!;
        response.FirstPage = _uriService.GetPageUri(new PaginationFilter(1, validFilter.PageSize), route);
        response.LastPage = _uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.PageSize), route);
        response.TotalPages = roundedTotalPages;
        response.TotalRecords = totalRecords;
        return response;
    }
}