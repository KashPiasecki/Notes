using Swashbuckle.AspNetCore.Annotations;

namespace Notes.Application.Filtering;

public class NoteFilterQuery
{
    [SwaggerParameter(Description="Title containing phrase", Required = false)]
    public string Title { get; init; } = string.Empty;
    [SwaggerParameter(Description="Content containing phrase", Required = false)]
    public string Content { get; init; } = string.Empty;
}