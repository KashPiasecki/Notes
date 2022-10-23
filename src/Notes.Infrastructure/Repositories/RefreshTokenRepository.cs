using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Domain.Entities;
using Notes.Infrastructure.Persistence;

namespace Notes.Infrastructure.Repositories;

public class RefreshTokenRepository : BaseRepository, IRefreshTokenRepository
{
    public RefreshTokenRepository(DataContext dataContext) : base(dataContext)
    {
    }

    public async Task<RefreshToken?> GetAsync(string refreshToken, CancellationToken cancellationToken) =>
        await DataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token.Equals(refreshToken), cancellationToken);

    public void Update(RefreshToken storedRefreshToken) =>
        DataContext.RefreshTokens.Update(storedRefreshToken);

    public async Task AddAsync(RefreshToken refreshToken) =>
        await DataContext.RefreshTokens.AddAsync(refreshToken);
}