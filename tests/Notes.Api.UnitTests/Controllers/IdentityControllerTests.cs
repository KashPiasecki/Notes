using MediatR;
using Notes.Api.Controllers;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Contracts.Identity;
using TddXt.AnyRoot.Builder;

namespace Notes.Api.UnitTests.Controllers;

public class IdentityControllerTests
{
    [Test]
    public async Task RegisterAdmin_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.IsAdmin, false);
        mediator.Send(registerUserCommand).Returns(Any.Instance<AuthenticationResult>());
        
        var identityController = new IdentityController(mediator);

        // Act
        await identityController.RegisterAdmin(registerUserCommand);
        
        // Assert
        await mediator.Received(1).Send(registerUserCommand);
    }

    [Test]
    public async Task RegisterAdmin_Called_SetsAdmin()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var registerUserCommand = Any.Instance<RegisterUserCommand>();
        mediator.Send(registerUserCommand).Returns(Any.Instance<AuthenticationResult>());
        
        var identityController = new IdentityController(mediator);

        // Act
        await identityController.RegisterAdmin(registerUserCommand);
        
        // Assert
        registerUserCommand.IsAdmin.Should().BeTrue();
    }
    
    [Test]
    public async Task Register_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var registerUserCommand = Any.Instance<RegisterUserCommand>();
        mediator.Send(registerUserCommand).Returns(Any.Instance<AuthenticationResult>());
        
        var identityController = new IdentityController(mediator);

        // Act
        await identityController.Register(registerUserCommand);
        
        // Assert
        await mediator.Received(1).Send(registerUserCommand);
    }

    [Test]
    public async Task Login_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var loginUserCommand = Any.Instance<LoginUserCommand>();
        mediator.Send(loginUserCommand).Returns(Any.Instance<AuthenticationResult>());
        
        var identityController = new IdentityController(mediator);

        // Act
        await identityController.Login(loginUserCommand);
        
        // Assert
        await mediator.Received(1).Send(loginUserCommand);
    }
    
    [Test]
    public async Task RefreshToken_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var refreshTokenCommand = Any.Instance<RefreshTokenCommand>();
        mediator.Send(refreshTokenCommand).Returns(Any.Instance<AuthenticationResult>());
        
        var identityController = new IdentityController(mediator);

        // Act
        await identityController.RefreshToken(refreshTokenCommand);
        
        // Assert
        await mediator.Received(1).Send(refreshTokenCommand);
    }
}