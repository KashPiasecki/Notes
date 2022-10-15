using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts.Filters;
using Notes.Domain.Contracts.Responses;

namespace Notes.Application.Common.Interfaces.Handlers;

public interface IPaginationHandler
{
    PagedResponse<T> CreatePagedResponse<T>(IEnumerable<T> pagedData, PaginationFilter validFilter, int totalRecords, string route);
    PaginationFilter ValidateQuery(PaginationFilterQuery paginationFilterQuery);
}