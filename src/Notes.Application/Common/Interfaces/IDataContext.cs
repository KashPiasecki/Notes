using Microsoft.EntityFrameworkCore;
using Notes.Domain.Entities;

namespace Notes.Application.Common.Interfaces;

public interface IDataContext
{
    public DbSet<Note> Notes { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}