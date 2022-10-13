using Notes.Domain.Contracts.Filters;

namespace Notes.Application.Common.Interfaces;

public interface IUriService
{
    public Uri GetPageUri(PaginationFilter filter, string route);
}