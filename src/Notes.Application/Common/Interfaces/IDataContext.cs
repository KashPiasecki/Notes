using Microsoft.EntityFrameworkCore;
using Notes.Domain.Entities;
using Notes.Domain.Identity;

namespace Notes.Application.Common.Interfaces;

public interface IDataContext
{
    public DbSet<Note> Notes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}