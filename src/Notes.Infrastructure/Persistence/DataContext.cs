using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Contracts.Identity;
using Notes.Domain.Entities;

namespace Notes.Infrastructure.Persistence;

public class DataContext : IdentityDbContext<IdentityUser, IdentityRole, string>, IDataContext
{
    public DbSet<Note> Notes { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}