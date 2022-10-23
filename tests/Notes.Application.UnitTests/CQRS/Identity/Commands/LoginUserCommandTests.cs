using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.Common.Interfaces.Wrappers;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Contracts.Identity;
using Notes.Domain.Contracts.Responses;
using Notes.Domain.Entities;
using NSubstitute.ReturnsExtensions;
using TddXt.AnyRoot.Builder;
using TddXt.AnyRoot.Invokable;

namespace Notes.Application.UnitTests.CQRS.Identity.Commands;

public class LoginUserCommandTests
{
    [Test]
    public async Task Handle_WithNotExistingUser_GeneratesFailureResponse()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();
        var loginUserCommand = Any.Instance<LoginUserCommand>();
        
        var loginUserCommandHandler = new LoginUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ITokenHandler>(),
            userManagerWrapper,
            Any.Instance<ILogger<LoginUserCommandHandler>>());
        userManagerWrapper.FindByEmailAsync(loginUserCommand.Email).ReturnsNull();

        // Act
        var result = (AuthenticationFailedResult)await loginUserCommandHandler.Handle(loginUserCommand, Any.CancellationToken());

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("The email or password is invalid");
    }
    
    [Test]
    public async Task Handle_WithInvalidPassword_GeneratesFailureResponse()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();
        
        var loginUserCommand = Any.Instance<LoginUserCommand>();
        var loginUserCommandHandler = new LoginUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ITokenHandler>(),
            userManagerWrapper,
            Any.Instance<ILogger<LoginUserCommandHandler>>());
        var user = Any.Instance<IdentityUser>();
        userManagerWrapper.FindByEmailAsync(loginUserCommand.Email).Returns(user);
        userManagerWrapper.CheckPasswordAsync(user, loginUserCommand.Password).Returns(false);

        // Act
        var result = (AuthenticationFailedResult)await loginUserCommandHandler.Handle(loginUserCommand, Any.CancellationToken());

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().BeEquivalentTo("The email or password is invalid");
    }
    
    [Test]
    public async Task Handle_WithValidCredentials_GeneratesSuccessResponse()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();
        var tokenHandler = Substitute.For<ITokenHandler>();
        var loginUserCommand = Any.Instance<LoginUserCommand>();
        
        var loginUserCommandHandler = new LoginUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            tokenHandler,
            userManagerWrapper,
            Any.Instance<ILogger<LoginUserCommandHandler>>());
        var user = Any.Instance<IdentityUser>();
        var token = Any.Instance<TokenResponse>()
            .WithProperty(x => x.RefreshToken, Any.Instance<RefreshToken>());
        userManagerWrapper.FindByEmailAsync(loginUserCommand.Email).Returns(user);
        userManagerWrapper.CheckPasswordAsync(user, loginUserCommand.Password).Returns(true);
        tokenHandler.GenerateToken(user).Returns(token);
        
        // Act
        var result = (AuthenticationSuccessResult)await loginUserCommandHandler.Handle(loginUserCommand, Any.CancellationToken());

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().Be(token.Token);
        result.RefreshToken.Should().Be(token.RefreshToken.Token);
    }
}