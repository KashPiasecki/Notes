using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.Common.Interfaces.Wrappers;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Contracts.Constants;
using Notes.Domain.Contracts.Identity;
using Notes.Domain.Contracts.Responses;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using TddXt.AnyRoot.Builder;

namespace Notes.Application.UnitTests.CQRS.Identity.Commands;

public class RegisterUserCommandTests
{
    [Test]
    public async Task Handle_AlreadyExistingUser_AuthenticationFailedResult()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();

        var registerUserCommandHandler = new RegisterUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ITokenHandler>(),
            userManagerWrapper,
            Any.Instance<ILogger<RegisterUserCommandHandler>>());
        var registerUserCommand = Any.Instance<RegisterUserCommand>();
        userManagerWrapper.HasAnyUsers().Returns(true);
        userManagerWrapper.FindByEmailAsync(registerUserCommand.Email).Returns(Any.Instance<IdentityUser>());

        // Act
        var result = (AuthenticationFailedResult)await registerUserCommandHandler.Handle(registerUserCommand, Any.Instance<CancellationToken>());

        // Assert
        result.Success.Should().BeFalse();
        result.Errors.Should().Contain("User with this email address already exists");
    }
    
    [Test]
    public async Task Handle_TransientErrorWhileCreatingUser_AuthenticationFailedResult()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();

        var registerUserCommandHandler = new RegisterUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ITokenHandler>(),
            userManagerWrapper,
            Any.Instance<ILogger<RegisterUserCommandHandler>>());
        var registerUserCommand = Any.Instance<RegisterUserCommand>();
        userManagerWrapper.HasAnyUsers().Returns(true);
        userManagerWrapper.FindByEmailAsync(registerUserCommand.Email).ReturnsNull();
        var identityUser = Any.Instance<IdentityUser>();
        userManagerWrapper.CreateIdentityUser(registerUserCommand.Email, registerUserCommand.UserName).Returns(identityUser);
        userManagerWrapper.CreateAsync(identityUser, registerUserCommand.Password).Returns(Any.Instance<IdentityResult>()
            .WithProperty(x => x.Succeeded, false));

        // Act
        var result = (AuthenticationFailedResult)await registerUserCommandHandler.Handle(registerUserCommand, Any.Instance<CancellationToken>());

        // Assert
        result.Success.Should().BeFalse();
    }
    
    [Test]
    public async Task Handle_WithoutAdmin_DoesNotAddAdminRole()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();

        var registerUserCommandHandler = new RegisterUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ITokenHandler>(),
            userManagerWrapper,
            Any.Instance<ILogger<RegisterUserCommandHandler>>());
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.IsAdmin, false);
        userManagerWrapper.HasAnyUsers().Returns(true);
        userManagerWrapper.FindByEmailAsync(registerUserCommand.Email).ReturnsNull();
        var identityUser = Any.Instance<IdentityUser>();
        userManagerWrapper.CreateIdentityUser(registerUserCommand.Email, registerUserCommand.UserName).Returns(identityUser);
        userManagerWrapper.CreateAsync(identityUser, registerUserCommand.Password).Returns(Any.Instance<IdentityResult>()
            .WithProperty(x => x.Succeeded, true));

        // Act
        await registerUserCommandHandler.Handle(registerUserCommand, Any.Instance<CancellationToken>());

        // Assert
        await userManagerWrapper.DidNotReceive().AddToRoleAsync(Arg.Any<IdentityUser>(), RoleNames.Admin);
    }
    
    [Test]
    public async Task Handle_WithAdmin_AddsAdminRole()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();

        var registerUserCommandHandler = new RegisterUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            Any.Instance<ITokenHandler>(),
            userManagerWrapper,
            Any.Instance<ILogger<RegisterUserCommandHandler>>());
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.IsAdmin, true);
        userManagerWrapper.HasAnyUsers().Returns(true);
        userManagerWrapper.FindByEmailAsync(registerUserCommand.Email).ReturnsNull();
        var identityUser = Any.Instance<IdentityUser>();
        userManagerWrapper.CreateIdentityUser(registerUserCommand.Email, registerUserCommand.UserName).Returns(identityUser);
        userManagerWrapper.CreateAsync(identityUser, registerUserCommand.Password).Returns(Any.Instance<IdentityResult>()
            .WithProperty(x => x.Succeeded, true));

        // Act
        await registerUserCommandHandler.Handle(registerUserCommand, Any.Instance<CancellationToken>());

        // Assert
        await userManagerWrapper.Received(1).AddToRoleAsync(identityUser, RoleNames.Admin);
    }

    [Test]
    public async Task Handle_NoUsersInDatabase_CreatesAdminUser()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();
        var tokenHandler = Substitute.For<ITokenHandler>();

        var registerUserCommandHandler = new RegisterUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            tokenHandler,
            userManagerWrapper,
            Any.Instance<ILogger<RegisterUserCommandHandler>>());
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.IsAdmin, false);
        userManagerWrapper.HasAnyUsers().Returns(false);
        userManagerWrapper.FindByEmailAsync(registerUserCommand.Email).ReturnsNull();
        var identityUser = Any.Instance<IdentityUser>();
        userManagerWrapper.CreateIdentityUser(registerUserCommand.Email, registerUserCommand.UserName).Returns(identityUser);
        userManagerWrapper.CreateAsync(identityUser, registerUserCommand.Password).Returns(Any.Instance<IdentityResult>()
            .WithProperty(x => x.Succeeded, true));
        var tokenResponse = Any.Instance<TokenResponse>();
        tokenHandler.GenerateToken(identityUser).Returns(tokenResponse);

        // Act
        var result = (AuthenticationSuccessResult)await registerUserCommandHandler.Handle(registerUserCommand, Any.Instance<CancellationToken>());

        // Assert
        await userManagerWrapper.Received(1).AddToRoleAsync(identityUser, RoleNames.Admin);
        result.Success.Should().BeTrue();
        result.Token.Should().Be(tokenResponse.Token);
        result.RefreshToken.Should().Be(tokenResponse.RefreshToken.Token);
    }
    
    [Test]
    public async Task Handle_UsersInDatabase_CreatesRegularUser()
    {
        // Arrange
        var userManagerWrapper = Substitute.For<IUserManagerWrapper>();
        var tokenHandler = Substitute.For<ITokenHandler>();

        var registerUserCommandHandler = new RegisterUserCommandHandler(
            Any.Instance<IUnitOfWork>(),
            tokenHandler,
            userManagerWrapper,
            Any.Instance<ILogger<RegisterUserCommandHandler>>());
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.IsAdmin, false);
        userManagerWrapper.HasAnyUsers().Returns(true);
        userManagerWrapper.FindByEmailAsync(registerUserCommand.Email).ReturnsNull();
        var identityUser = Any.Instance<IdentityUser>();
        userManagerWrapper.CreateIdentityUser(registerUserCommand.Email, registerUserCommand.UserName).Returns(identityUser);
        userManagerWrapper.CreateAsync(identityUser, registerUserCommand.Password).Returns(Any.Instance<IdentityResult>()
            .WithProperty(x => x.Succeeded, true));
        var tokenResponse = Any.Instance<TokenResponse>();
        tokenHandler.GenerateToken(identityUser).Returns(tokenResponse);

        // Act
        var result = (AuthenticationSuccessResult)await registerUserCommandHandler.Handle(registerUserCommand, Any.Instance<CancellationToken>());

        // Assert
        await userManagerWrapper.DidNotReceive().AddToRoleAsync(Arg.Any<IdentityUser>(), RoleNames.Admin);
        result.Success.Should().BeTrue();
        result.Token.Should().Be(tokenResponse.Token);
        result.RefreshToken.Should().Be(tokenResponse.RefreshToken.Token);
    }
}