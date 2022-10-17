using FluentAssertions;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Application.UnitTests.TestsUtility.Extensions;
using NUnit.Framework;
using TddXt.AnyRoot.Builder;
using TddXt.AnyRoot.Strings;

namespace Notes.Application.UnitTests.CQRS.Identity.Commands;

public class RegisterUserCommandValidatorTests
{
    [Test]
    public async Task Handle_ValidEntity_IsValid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.ValidPassword())
            .WithProperty(x => x.Email, Any.Email());
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeTrue();
    } 
    
    [Test]
    public async Task Handle_InvalidEmail_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.ValidPassword());
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("'Email' is not a valid email address.");
    }
    
    [Test]
    public async Task Handle_EmptyUserName_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.ValidPassword())
            .WithProperty(x => x.Email, Any.Email())
            .WithProperty(x => x.UserName, string.Empty);
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("'User Name' must not be empty." );
    }
    
    [Test]
    public async Task Handle_TooShortUserName_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.ValidPassword())
            .WithProperty(x => x.Email, Any.Email())
            .WithProperty(x => x.UserName, Any.String(3));
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("The length of 'User Name' must be at least 4 characters. You entered 3 characters.");
    }
    
    [Test]
    public async Task Handle_TooLongUserName_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.ValidPassword())
            .WithProperty(x => x.Email, Any.Email())
            .WithProperty(x => x.UserName, Any.String(51));
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("The length of 'User Name' must be 50 characters or fewer. You entered 51 characters.");
    }
    
    [Test]
    public async Task Handle_EmptyPassword_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, string.Empty)
            .WithProperty(x => x.Email, Any.Email());
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("'Password' must not be empty.");
    }
    
    [Test]
    public async Task Handle_TooShortPassword_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.String(5))
            .WithProperty(x => x.Email, Any.Email());
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("The length of 'Password' must be at least 6 characters. You entered 5 characters.");
    }
    
    [Test]
    public async Task Handle_TooLongPassword_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.String(33))
            .WithProperty(x => x.Email, Any.Email());
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("The length of 'Password' must be 32 characters or fewer. You entered 33 characters.");
    }
    
    [Test]
    public async Task Handle_PasswordWithoutUpperCase_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.LowerCaseString(10))
            .WithProperty(x => x.Email, Any.Email());
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("Your password must contain at least one uppercase letter.");
    }
    
    [Test]
    public async Task Handle_PasswordWithoutLowerCase_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.UpperCaseString(10))
            .WithProperty(x => x.Email, Any.Email());
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("Your password must contain at least one lowercase letter.");
    }
    
    [Test]
    public async Task Handle_PasswordWithoutDigits_IsInvalid()
    {
        // Arrange
        var registerUserCommandValidator = new RegisterUserCommandValidator();
        var registerUserCommand = Any.Instance<RegisterUserCommand>()
            .WithProperty(x => x.Password, Any.MixedCaseString(10))
            .WithProperty(x => x.Email, Any.Email());
        
        // Act
        var result = await registerUserCommandValidator.ValidateAsync(registerUserCommand);
        
        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("Your password must contain at least one number.");
    }
    
}