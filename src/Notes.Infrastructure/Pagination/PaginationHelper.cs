using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts;
using Notes.Domain.Filters;

namespace Notes.Infrastructure.Pagination;

public class PaginationHelper : IPaginationHelper
{
    private readonly IUriService _uriService;

    public PaginationHelper(IUriService uriService)
    {
        _uriService = uriService;
    }

    public PaginationFilter ValidateQuery(PaginationFilterQuery paginationFilterQuery)
    {
        return new PaginationFilter
        {
            PageNumber = paginationFilterQuery.PageNumber < 1 ? 1 : paginationFilterQuery.PageNumber,
            PageSize = paginationFilterQuery.PageSize is < 1 or > 10
                ? 10
                : paginationFilterQuery.PageSize
        };
    }
    
    public PagedResponse<T> CreatePagedResponse<T>(IEnumerable<T> pagedData, PaginationFilter validFilter, int totalRecords, string route)
    {
        var response = new PagedResponse<T>(pagedData, validFilter.PageNumber, validFilter.PageSize);
        var totalPages = totalRecords / (double)validFilter.PageSize;
        var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
        response.NextPage =
            validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
                ? _uriService.GetPageUri(new PaginationFilter
                {
                    PageNumber = validFilter.PageNumber + 1,
                    PageSize = validFilter.PageSize
                }, route)
                : null!;
        response.PreviousPage =
            validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
                ? _uriService.GetPageUri(new PaginationFilter
                {
                    PageNumber = validFilter.PageNumber - 1,
                    PageSize = validFilter.PageSize
                }, route)
                : null!;
        response.FirstPage = _uriService.GetPageUri(new PaginationFilter
        {
            PageNumber = 1,
            PageSize = validFilter.PageSize
        }, route);
        response.LastPage = _uriService.GetPageUri(new PaginationFilter
        {
            PageNumber = roundedTotalPages,
            PageSize = validFilter.PageSize
        }, route);
        response.TotalPages = roundedTotalPages;
        response.TotalRecords = totalRecords;
        return response;
    }
}