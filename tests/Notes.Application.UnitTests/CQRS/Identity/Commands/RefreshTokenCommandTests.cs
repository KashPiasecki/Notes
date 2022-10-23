using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Providers;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.Common.Interfaces.Wrappers;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Contracts.Identity;
using Notes.Domain.Contracts.Responses;
using Notes.Domain.Entities;
using NSubstitute.ReturnsExtensions;
using TddXt.AnyRoot.Builder;
using TddXt.AnyRoot.Invokable;
using TddXt.AnyRoot.Strings;

namespace Notes.Application.UnitTests.CQRS.Identity.Commands;

public class RefreshTokenCommandTests
{
    [Test]
    public async Task Handle_WithNonExistingClaimsPrincipal_GeneratesFailureResponse()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            Any.Instance<IUnitOfWork>(),
            tokenHandler,
            Any.Instance<IClaimsPrincipalInfoProvider>(),
            Any.Instance<IUserManagerWrapper>(),
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).ReturnsNull();

        // Act
        var result = (AuthenticationFailedResult)await refreshTokenCommandHandler.Handle(refreshTokenCommand, Any.CancellationToken());

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("Invalid token");
    }

    [Test]
    public async Task Handle_WithUnexpiredToken_GeneratesFailureResponse()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var claimsPrincipalInfoProvider = Substitute.For<IClaimsPrincipalInfoProvider>();
        var validatedToken = Any.Instance<ClaimsPrincipal>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            Any.Instance<IUnitOfWork>(),
            tokenHandler,
            claimsPrincipalInfoProvider,
            Any.Instance<IUserManagerWrapper>(),
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).Returns(validatedToken);
        claimsPrincipalInfoProvider.GetExpiryTime(validatedToken).Returns(DateTime.MaxValue);

        // Act
        var result = (AuthenticationFailedResult)await refreshTokenCommandHandler.Handle(refreshTokenCommand, Any.CancellationToken());

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("Invalid token");
    }

    [Test]
    public async Task Handle_WithNonExistingToken_GeneratesFailureResponse()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var unitOfWork = Any.Instance<IUnitOfWork>()
            .WithProperty(x => x.RefreshTokens, refreshTokenRepository);
        var claimsPrincipalInfoProvider = Substitute.For<IClaimsPrincipalInfoProvider>();
        var validatedToken = Any.Instance<ClaimsPrincipal>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();
        var cancellationToken = Any.CancellationToken();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            unitOfWork,
            tokenHandler,
            claimsPrincipalInfoProvider,
            Any.Instance<IUserManagerWrapper>(),
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).Returns(validatedToken);
        claimsPrincipalInfoProvider.GetExpiryTime(validatedToken).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        claimsPrincipalInfoProvider.GetId(validatedToken).Returns(Any.String());
        refreshTokenRepository.GetAsync(refreshTokenCommand.Token, cancellationToken).ReturnsNull();

        // Act
        var result = (AuthenticationFailedResult)await refreshTokenCommandHandler.Handle(refreshTokenCommand, cancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("Invalid token");
    }

    [Test]
    public async Task Handle_WithTokenNotMatchingJwtId_GeneratesFailureResponse()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var unitOfWork = Any.Instance<IUnitOfWork>()
            .WithProperty(x => x.RefreshTokens, refreshTokenRepository);
        var claimsPrincipalInfoProvider = Substitute.For<IClaimsPrincipalInfoProvider>();
        var validatedToken = Any.Instance<ClaimsPrincipal>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();
        var cancellationToken = Any.CancellationToken();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            unitOfWork,
            tokenHandler,
            claimsPrincipalInfoProvider,
            Any.Instance<IUserManagerWrapper>(),
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).Returns(validatedToken);
        claimsPrincipalInfoProvider.GetExpiryTime(validatedToken).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        claimsPrincipalInfoProvider.GetId(validatedToken).Returns(Any.String());
        refreshTokenRepository.GetAsync(refreshTokenCommand.RefreshToken, cancellationToken).Returns(Any.Instance<RefreshToken>());

        // Act
        var result = (AuthenticationFailedResult)await refreshTokenCommandHandler.Handle(refreshTokenCommand, cancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("Invalid token");
    }

    [Test]
    public async Task Handle_WithExpiredToken_GeneratesFailureResponse()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var unitOfWork = Any.Instance<IUnitOfWork>()
            .WithProperty(x => x.RefreshTokens, refreshTokenRepository);
        var claimsPrincipalInfoProvider = Substitute.For<IClaimsPrincipalInfoProvider>();
        var validatedToken = Any.Instance<ClaimsPrincipal>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();
        var cancellationToken = Any.CancellationToken();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            unitOfWork,
            tokenHandler,
            claimsPrincipalInfoProvider,
            Any.Instance<IUserManagerWrapper>(),
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).Returns(validatedToken);
        claimsPrincipalInfoProvider.GetExpiryTime(validatedToken).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        var jwtId = Any.String();
        claimsPrincipalInfoProvider.GetId(validatedToken).Returns(jwtId);
        var refreshToken = Any.Instance<RefreshToken>().WithProperty(x => x.JwtId, jwtId)
            .WithProperty(x => x.ExpireDate, DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        refreshTokenRepository.GetAsync(refreshTokenCommand.RefreshToken, cancellationToken).Returns(refreshToken);

        // Act
        var result = (AuthenticationFailedResult)await refreshTokenCommandHandler.Handle(refreshTokenCommand, cancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("Invalid token");
    }

    [Test]
    public async Task Handle_WithInvalidatedToken_GeneratesFailureResponse()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var unitOfWork = Any.Instance<IUnitOfWork>()
            .WithProperty(x => x.RefreshTokens, refreshTokenRepository);
        var claimsPrincipalInfoProvider = Substitute.For<IClaimsPrincipalInfoProvider>();
        var validatedToken = Any.Instance<ClaimsPrincipal>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();
        var cancellationToken = Any.CancellationToken();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            unitOfWork,
            tokenHandler,
            claimsPrincipalInfoProvider,
            Any.Instance<IUserManagerWrapper>(),
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).Returns(validatedToken);
        claimsPrincipalInfoProvider.GetExpiryTime(validatedToken).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        var jwtId = Any.String();
        claimsPrincipalInfoProvider.GetId(validatedToken).Returns(jwtId);
        var refreshToken = Any.Instance<RefreshToken>().WithProperty(x => x.JwtId, jwtId)
            .WithProperty(x => x.ExpireDate, DateTime.Now.Add(TimeSpan.FromDays(1)))
            .WithProperty(x => x.Invalidated, true);
        refreshTokenRepository.GetAsync(refreshTokenCommand.RefreshToken, cancellationToken).Returns(refreshToken);

        // Act
        var result = (AuthenticationFailedResult)await refreshTokenCommandHandler.Handle(refreshTokenCommand, cancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("Invalid token");
    }

    [Test]
    public async Task Handle_WithUsedToken_GeneratesFailureResponse()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var unitOfWork = Any.Instance<IUnitOfWork>()
            .WithProperty(x => x.RefreshTokens, refreshTokenRepository);
        var claimsPrincipalInfoProvider = Substitute.For<IClaimsPrincipalInfoProvider>();
        var validatedToken = Any.Instance<ClaimsPrincipal>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();
        var cancellationToken = Any.CancellationToken();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            unitOfWork,
            tokenHandler,
            claimsPrincipalInfoProvider,
            Any.Instance<IUserManagerWrapper>(),
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).Returns(validatedToken);
        claimsPrincipalInfoProvider.GetExpiryTime(validatedToken).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        var jwtId = Any.String();
        claimsPrincipalInfoProvider.GetId(validatedToken).Returns(jwtId);
        var refreshToken = Any.Instance<RefreshToken>()
            .WithProperty(x => x.JwtId, jwtId)
            .WithProperty(x => x.ExpireDate, DateTime.Now.Add(TimeSpan.FromDays(1)))
            .WithProperty(x => x.Invalidated, false)
            .WithProperty(x => x.Used, true);
        refreshTokenRepository.GetAsync(refreshTokenCommand.RefreshToken, cancellationToken).Returns(refreshToken);

        // Act
        var result = (AuthenticationFailedResult)await refreshTokenCommandHandler.Handle(refreshTokenCommand, cancellationToken);

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("Invalid token");
    }
    
    [Test]
    public async Task Handle_WithValidToken_GeneratesSuccessResponse()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var unitOfWork = Any.Instance<IUnitOfWork>()
            .WithProperty(x => x.RefreshTokens, refreshTokenRepository);
        var claimsPrincipalInfoProvider = Substitute.For<IClaimsPrincipalInfoProvider>();
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();
        var validatedToken = Any.Instance<ClaimsPrincipal>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();
        var cancellationToken = Any.CancellationToken();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            unitOfWork,
            tokenHandler,
            claimsPrincipalInfoProvider,
            userManagerWrapper,
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).Returns(validatedToken);
        claimsPrincipalInfoProvider.GetExpiryTime(validatedToken).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        var jwtId = Any.String();
        claimsPrincipalInfoProvider.GetId(validatedToken).Returns(jwtId);
        var refreshToken = Any.Instance<RefreshToken>()
            .WithProperty(x => x.JwtId, jwtId)
            .WithProperty(x => x.ExpireDate, DateTime.Now.Add(TimeSpan.FromDays(1)))
            .WithProperty(x => x.Invalidated, false)
            .WithProperty(x => x.Used, false);
        refreshTokenRepository.GetAsync(refreshTokenCommand.RefreshToken, cancellationToken).Returns(refreshToken);
        var userId = Any.String();
        claimsPrincipalInfoProvider.GetUserId(validatedToken).Returns(userId);
        var user = Any.Instance<IdentityUser>();
        userManagerWrapper.FindByIdAsync(userId).Returns(user);
        var tokenResponse = Any.Instance<TokenResponse>();
        tokenHandler.GenerateToken(user).Returns(tokenResponse);
        
        // Act
        var result = (AuthenticationSuccessResult)await refreshTokenCommandHandler.Handle(refreshTokenCommand, cancellationToken);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().Be(tokenResponse.Token);
        result.RefreshToken.Should().Be(tokenResponse.RefreshToken.Token);
    }
    
    [Test]
    public async Task Handle_WithValidToken_ChangesItToUsed()
    {
        // Arrange
        var tokenHandler = Substitute.For<ITokenHandler>();
        var refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
        var unitOfWork = Any.Instance<IUnitOfWork>()
            .WithProperty(x => x.RefreshTokens, refreshTokenRepository);
        var claimsPrincipalInfoProvider = Substitute.For<IClaimsPrincipalInfoProvider>();
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();
        var validatedToken = Any.Instance<ClaimsPrincipal>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();
        var cancellationToken = Any.CancellationToken();

        var refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            unitOfWork,
            tokenHandler,
            claimsPrincipalInfoProvider,
            userManagerWrapper,
            Any.Instance<ILogger<RefreshTokenCommandHandler>>());
        tokenHandler.GetPrincipalFromToken(refreshTokenCommand.Token).Returns(validatedToken);
        claimsPrincipalInfoProvider.GetExpiryTime(validatedToken).Returns(DateTime.Now.Subtract(TimeSpan.FromDays(1)));
        var jwtId = Any.String();
        claimsPrincipalInfoProvider.GetId(validatedToken).Returns(jwtId);
        var refreshToken = Any.Instance<RefreshToken>()
            .WithProperty(x => x.JwtId, jwtId)
            .WithProperty(x => x.ExpireDate, DateTime.Now.Add(TimeSpan.FromDays(1)))
            .WithProperty(x => x.Invalidated, false)
            .WithProperty(x => x.Used, false);
        refreshTokenRepository.GetAsync(refreshTokenCommand.RefreshToken, cancellationToken).Returns(refreshToken);
        var userId = Any.String();
        claimsPrincipalInfoProvider.GetUserId(validatedToken).Returns(userId);
        var user = Any.Instance<IdentityUser>();
        userManagerWrapper.FindByIdAsync(userId).Returns(user);
        var tokenResponse = Any.Instance<TokenResponse>();
        tokenHandler.GenerateToken(user).Returns(tokenResponse);
        
        // Act
        await refreshTokenCommandHandler.Handle(refreshTokenCommand, cancellationToken);

        // Assert
        refreshToken.Used.Should().BeTrue();
    }
}