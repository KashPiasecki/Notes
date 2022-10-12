using Notes.Application.CQRS.Filtering;
using Notes.Domain.Entities;
using Notes.Domain.Filters;

namespace Notes.Application.Common.Interfaces.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetNotesAsync(PaginationFilter paginationFilter, NoteFilterQuery noteFilterQuery,
        CancellationToken cancellationToken);
    Task<int> CountAsync(NoteFilterQuery noteFilterQuery, CancellationToken cancellationToken);
}