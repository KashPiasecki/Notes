using Notes.Application.CQRS.Filtering;
using Notes.Domain.Contracts.Filters;
using Notes.Domain.Entities;

namespace Notes.Application.Common.Interfaces.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync(PaginationFilter paginationFilter, NoteFilterQuery noteFilterQuery,
        CancellationToken cancellationToken);

    Task<Note?> GetByIdAsync(Guid noteId, CancellationToken cancellationToken);
    Task<Note?> GetByIdForUserAsync(string userId, Guid noteId, CancellationToken cancellationToken);
    Task<int> CountAsync(NoteFilterQuery noteFilterQuery, CancellationToken cancellationToken);
    void Remove(Note note);
    Task<IEnumerable<Note>> GetAllForUserAsync(string userId, PaginationFilter paginationFilter, NoteFilterQuery noteFilterQuery, CancellationToken cancellationToken);
    public Task<int> CountForUserAsync(string userId, NoteFilterQuery noteFilterQuery, CancellationToken cancellationToken);
    Task<Note> AddAsync(Note note, CancellationToken cancellationToken);
}