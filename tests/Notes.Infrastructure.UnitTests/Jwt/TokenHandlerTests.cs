using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.Common.Interfaces.Wrappers;
using Notes.Domain.Configurations;
using Notes.Domain.Contracts.Constants;
using Notes.Domain.Entities;
using TddXt.AnyRoot.Builder;
using TokenHandler = Notes.Infrastructure.Jwt.TokenHandler;

namespace Notes.Infrastructure.UnitTests.Jwt;

public class TokenHandlerTests
{
    [Test]
    public async Task GenerateToken_Called_AssignsProperClaims()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();

        var tokenHandler = new TokenHandler(
            Any.Instance<JwtConfiguration>()
                .WithProperty(x => x.TokenLifetime, Any.Instance<TimeSpan>().ToString()),
            Any.Instance<TokenValidationParameters>(),
            userManagerWrapper,
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ILogger<TokenHandler>>());
        var identityUser = Any.Instance<IdentityUser>();
        userManagerWrapper.IsInRoleAsync(identityUser, RoleNames.Admin).Returns(false);

        // Act
        var result = await tokenHandler.GenerateToken(identityUser);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(result.Token);
        jwtSecurityToken.Claims.SingleOrDefault(x => x.Type.Equals(JwtClaimNames.Sub)).Should().NotBeNull();
        jwtSecurityToken.Claims.SingleOrDefault(x => x.Type.Equals(JwtClaimNames.Jti)).Should().NotBeNull();
        jwtSecurityToken.Claims.SingleOrDefault(x => x.Type.Equals(JwtClaimNames.Email)).Should().NotBeNull();
        jwtSecurityToken.Claims.SingleOrDefault(x => x.Type.Equals(JwtClaimNames.UserId)).Should().NotBeNull();
        jwtSecurityToken.Claims.SingleOrDefault(x => x.Value.Equals(RoleNames.User)).Should().NotBeNull();
    }

    [Test]
    public async Task GenerateToken_WithoutAdminRole_DoesNotAddAdminClaim()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();

        var tokenHandler = new TokenHandler(
            Any.Instance<JwtConfiguration>()
                .WithProperty(x => x.TokenLifetime, Any.Instance<TimeSpan>().ToString()),
            Any.Instance<TokenValidationParameters>(),
            userManagerWrapper,
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ILogger<TokenHandler>>());
        var identityUser = Any.Instance<IdentityUser>();
        userManagerWrapper.IsInRoleAsync(identityUser, RoleNames.Admin).Returns(false);

        // Act
        var result = await tokenHandler.GenerateToken(identityUser);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(result.Token);
        jwtSecurityToken.Claims.SingleOrDefault(x => x.Value.Equals(RoleNames.Admin)).Should().BeNull();
    }

    [Test]
    public async Task GenerateToken_WithAdminRole_AddsAdminClaim()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();

        var tokenHandler = new TokenHandler(
            Any.Instance<JwtConfiguration>()
                .WithProperty(x => x.TokenLifetime, Any.Instance<TimeSpan>().ToString()),
            Any.Instance<TokenValidationParameters>(),
            userManagerWrapper,
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ILogger<TokenHandler>>());
        var identityUser = Any.Instance<IdentityUser>();
        userManagerWrapper.IsInRoleAsync(identityUser, RoleNames.Admin).Returns(true);

        // Act
        var result = await tokenHandler.GenerateToken(identityUser);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(result.Token);
        jwtSecurityToken.Claims.SingleOrDefault(x => x.Value.Equals(RoleNames.Admin)).Should().NotBeNull();
    }

    [Test]
    public async Task GenerateToken_Called_SavesRefreshTokenToDatabase()
    {
        // Arrange
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        unitOfWork.RefreshTokens.Returns(refreshTokenRepository);
        var logger = Any.Instance<ILogger<TokenHandler>>();

        var tokenHandler = new TokenHandler(
            Any.Instance<JwtConfiguration>()
                .WithProperty(x => x.TokenLifetime, Any.Instance<TimeSpan>().ToString()),
            Any.Instance<TokenValidationParameters>(),
            Any.Instance<IUserManagerWrapper>(),
            unitOfWork,
            logger);
        var identityUser = Any.Instance<IdentityUser>();

        // Act
        await tokenHandler.GenerateToken(identityUser);

        // Assert
        await refreshTokenRepository.Received(1).AddAsync(Arg.Any<RefreshToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GenerateToken_Called_ReturnsToken()
    {
        // Arrange
        var tokenHandler = new TokenHandler(
            Any.Instance<JwtConfiguration>()
                .WithProperty(x => x.TokenLifetime, Any.Instance<TimeSpan>().ToString()),
            Any.Instance<TokenValidationParameters>(),
            Any.Instance<IUserManagerWrapper>(),
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ILogger<TokenHandler>>());
        var identityUser = Any.Instance<IdentityUser>();

        // Act
        var result = await tokenHandler.GenerateToken(identityUser);

        // Assert
        result.Token.Should().NotBeEmpty();
        result.RefreshToken.Token.Should().NotBeEmpty();
    }
}