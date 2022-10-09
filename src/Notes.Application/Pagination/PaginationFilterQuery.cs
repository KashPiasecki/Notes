using Swashbuckle.AspNetCore.Annotations;

namespace Notes.Application.Pagination;

public class PaginationFilterQuery
{
    [SwaggerParameter(Description = "Default value 10", Required = false)]
    public int PageSize { get; init; } = 10;
    [SwaggerParameter(Description = "Default value 1", Required = false)]
    public int PageNumber { get; init; } = 1;
}