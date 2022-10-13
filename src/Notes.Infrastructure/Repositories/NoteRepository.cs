using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Filtering;
using Notes.Domain.Contracts.Filters;
using Notes.Domain.Entities;
using Notes.Infrastructure.Persistence;

namespace Notes.Infrastructure.Repositories;

public class NoteRepository : BaseRepository, INoteRepository
{
    public NoteRepository(DataContext dataContext) : base(dataContext)
    {
    }

    public async Task<Note?> GetNoteByIdAsync(Guid noteId, CancellationToken cancellationToken)
    {
        return await DataContext.Notes.SingleOrDefaultAsync(x => x.Id == noteId, cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<Note>> GetAllAsync(PaginationFilter paginationFilter, NoteFilterQuery noteFilterQuery,
        CancellationToken cancellationToken)
    {
        return await DataContext.Notes.Include(x => x.User)
            .Where(x => x.Title.ToLower().Contains(noteFilterQuery.Title.ToLower()))
            .Where(x => x.Content.ToLower().Contains(noteFilterQuery.Content.ToLower()))
            .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
            .Take(paginationFilter.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(NoteFilterQuery noteFilterQuery, CancellationToken cancellationToken)
    {
        return await DataContext.Notes
            .AsNoTracking()
            .Where(x => x.Title.ToLower().Contains(noteFilterQuery.Title.ToLower()))
            .Where(x => x.Content.ToLower().Contains(noteFilterQuery.Content.ToLower()))
            .CountAsync(cancellationToken);
    }

    public void Remove(Note note)
    {
        DataContext.Notes.Remove(note);
    }

    public async Task<IEnumerable<Note>> GetAllForUserAsync(string userId, PaginationFilter paginationFilter, NoteFilterQuery noteFilterQuery,
        CancellationToken cancellationToken)
    {
        return await DataContext.Notes.Include(x => x.User)
            .Where(x => x.UserId.Equals(userId))
            .Where(x => x.Title.ToLower().Contains(noteFilterQuery.Title.ToLower()))
            .Where(x => x.Content.ToLower().Contains(noteFilterQuery.Content.ToLower()))
            .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
            .Take(paginationFilter.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountForUserAsync(string userId, NoteFilterQuery noteFilterQuery, CancellationToken cancellationToken)
    {
        return await DataContext.Notes
            .AsNoTracking()
            .Where(x => x.UserId.Equals(userId))
            .Where(x => x.Title.ToLower().Contains(noteFilterQuery.Title.ToLower()))
            .Where(x => x.Content.ToLower().Contains(noteFilterQuery.Content.ToLower()))
            .CountAsync(cancellationToken);
    }

    public async Task<Note> AddAsync(Note note, CancellationToken cancellationToken)
    {
        return (await DataContext.Notes.AddAsync(note, cancellationToken)).Entity;
    }

    public async Task<Note?> GetByIdForUserAsync(string userId, Guid noteId, CancellationToken cancellationToken)
    {
        return await DataContext.Notes
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.UserId.Equals(userId) && x.Id == noteId, cancellationToken);
    }

    public async Task<Note?> GetByIdAsync(Guid noteId, CancellationToken cancellationToken)
    {
        return await DataContext.Notes
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.Id.Equals(noteId), cancellationToken);
    }
}