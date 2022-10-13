using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts.Filters;
using Notes.Domain.Contracts.Responses;

namespace Notes.Infrastructure.Pagination;

public class PaginationHandler : IPaginationHandler
{
    private readonly IUriService _uriService;

    public PaginationHandler(IUriService uriService)
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
        response.NextPage = GenerateResponseNextPage(validFilter, route, roundedTotalPages);
        response.PreviousPage = GenerateResponsePreviousPage(validFilter, route, roundedTotalPages);
        response.FirstPage = GenerateResponseFirstPage(validFilter, route);
        response.LastPage = GenerateResponseLastPage(validFilter, route, roundedTotalPages);
        response.TotalPages = roundedTotalPages;
        response.TotalRecords = totalRecords;
        return response;
    }

    private Uri GenerateResponseLastPage(PaginationFilter validFilter, string route, int roundedTotalPages)
    {
        return _uriService.GetPageUri(new PaginationFilter
        {
            PageNumber = roundedTotalPages,
            PageSize = validFilter.PageSize
        }, route);
    }

    private Uri GenerateResponseFirstPage(PaginationFilter validFilter, string route)
    {
        return _uriService.GetPageUri(new PaginationFilter
        {
            PageNumber = 1,
            PageSize = validFilter.PageSize
        }, route);
    }

    private Uri GenerateResponsePreviousPage(PaginationFilter validFilter, string route, int roundedTotalPages)
    {
        return validFilter.PageNumber - 1 >= 1 && validFilter.PageNumber <= roundedTotalPages
            ? _uriService.GetPageUri(new PaginationFilter
            {
                PageNumber = validFilter.PageNumber - 1,
                PageSize = validFilter.PageSize
            }, route)
            : null!;
    }

    private Uri GenerateResponseNextPage(PaginationFilter validFilter, string route, int roundedTotalPages)
    {
        return validFilter.PageNumber >= 1 && validFilter.PageNumber < roundedTotalPages
            ? _uriService.GetPageUri(new PaginationFilter
            {
                PageNumber = validFilter.PageNumber + 1,
                PageSize = validFilter.PageSize
            }, route)
            : null!;
    }
}