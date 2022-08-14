using Microsoft.EntityFrameworkCore;
using Notes.Domain.Entities;

namespace Notes.Infrastructure.Persistence;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }
    
    public DbSet<Note> Notes { get; set; }
}