using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Filtering;
using Notes.Domain.Entities;
using Notes.Domain.Filters;

namespace Notes.Infrastructure.Repositories;

public class NoteRepository : BaseRepository, INoteRepository
{
    public NoteRepository(IDataContext dataContext) : base(dataContext)
    {
    }

    public async Task<IEnumerable<Note>> GetNotesAsync(PaginationFilter paginationFilter, NoteFilterQuery noteFilterQuery, CancellationToken cancellationToken)
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
}