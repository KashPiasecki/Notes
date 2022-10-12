using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts;
using Notes.Domain.Filters;

namespace Notes.Application.Common.Interfaces;

public interface IPaginationHelper
{
    PagedResponse<T> CreatePagedResponse<T>(IEnumerable<T> pagedData, PaginationFilter validFilter, int totalRecords, string route);
    PaginationFilter ValidateQuery(PaginationFilterQuery paginationFilterQuery);
}