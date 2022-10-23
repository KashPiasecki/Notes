using EntityFrameworkCore.Testing.NSubstitute;
using Notes.Domain.Entities;
using Notes.Infrastructure.Persistence;
using Notes.Infrastructure.Repositories;
using TddXt.AnyRoot;
using TddXt.AnyRoot.Collections;
using TddXt.AnyRoot.Invokable;
using TddXt.AnyRoot.Strings;

namespace Notes.Infrastructure.UnitTests.Repositories;

public class RefreshTokenRepositoryTests
{
    [Test]
    public async Task GetAsync_NotExistingToken_ReturnsNull()
    {
        // Arrange
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();

        var refreshTokenRepository = new RefreshTokenRepository(mockedDbContext);

        // Act
        var result = await refreshTokenRepository.GetAsync(Any.String(), Any.CancellationToken());
        
        // Assert
        result.Should().BeNull();
    }
    
    [Test]
    public async Task GetAsync_ExistingToken_ReturnsRefreshToken()
    {
        // Arrange
        var refreshTokenList = Any.List<RefreshToken>(3);
        var refreshToken = refreshTokenList.First();
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.RefreshTokens.AddRange(refreshTokenList);
        await mockedDbContext.SaveChangesAsync();

        var refreshTokenRepository = new RefreshTokenRepository(mockedDbContext);

        // Act
        var result = await refreshTokenRepository.GetAsync(refreshToken.Token, Any.CancellationToken());
        
        // Assert
        result.Should().Be(refreshToken);
    }
    
    [Test]
    public async Task UpdateAsync_Called_UpdatesToken()
    {
        // Arrange
        var refreshToken = Any.Instance<RefreshToken>();
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        mockedDbContext.RefreshTokens.Add(refreshToken);
        await mockedDbContext.SaveChangesAsync();
        
        var refreshTokenRepository = new RefreshTokenRepository(mockedDbContext);
        refreshToken.Invalidated = Any.Boolean();
        refreshToken.Used = Any.Boolean();

        // Act
        refreshTokenRepository.Update(refreshToken);
        var result = await refreshTokenRepository.GetAsync(refreshToken.Token, Any.CancellationToken());
        
        // Assert
        result!.Invalidated.Should().Be(refreshToken.Invalidated);
        result.Used.Should().Be(refreshToken.Used);
    }
    
    [Test]
    public async Task AddAsync_Called_AddsRefreshToken()
    {
        // Arrange
        var mockedDbContext = Create.MockedDbContextFor<DataContext>();
        
        var refreshTokenRepository = new RefreshTokenRepository(mockedDbContext);
        var refreshToken = Any.Instance<RefreshToken>();

        // Act
        await refreshTokenRepository.AddAsync(refreshToken);
        await mockedDbContext.SaveChangesAsync();
        var result = await refreshTokenRepository.GetAsync(refreshToken.Token, Any.CancellationToken());
        
        // Assert
        result.Should().Be(refreshToken);
    }
}