using Notes.Domain.Entities;

namespace Notes.Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetAsync(string refreshToken, CancellationToken cancellationToken);
    void UpdateAsync(RefreshToken storedRefreshToken);
    Task AddAsync(RefreshToken refreshToken);
}