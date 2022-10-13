namespace Notes.Application.Common.Interfaces.Repositories;

public interface IUnitOfWork
{
    public INoteRepository Notes { get; init; }
    public IRefreshTokenRepository RefreshTokens { get; init; }

    public Task SaveChangesAsync(CancellationToken cancellationToken);
}