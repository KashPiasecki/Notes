using Notes.Application.Common.Interfaces.Repositories;
using Notes.Infrastructure.Persistence;

namespace Notes.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;
    public INoteRepository Notes { get; init; }
    public IRefreshTokenRepository RefreshTokens { get; init; }
    
    public UnitOfWork(
        DataContext dataContext,
        INoteRepository noteRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _dataContext = dataContext;
        Notes = noteRepository;
        RefreshTokens = refreshTokenRepository;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken) =>
        await _dataContext.SaveChangesAsync(cancellationToken);
}