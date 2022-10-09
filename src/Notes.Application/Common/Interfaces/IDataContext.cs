using Microsoft.EntityFrameworkCore;
using Notes.Domain.Contracts.Identity;
using Notes.Domain.Entities;

namespace Notes.Application.Common.Interfaces;

public interface IDataContext
{
    public DbSet<Note> Notes { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}