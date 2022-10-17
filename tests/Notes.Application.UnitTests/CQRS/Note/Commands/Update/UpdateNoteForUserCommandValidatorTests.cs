using FluentAssertions;
using Notes.Application.CQRS.Note.Commands.Update;
using NUnit.Framework;
using TddXt.AnyRoot.Builder;
using TddXt.AnyRoot.Strings;

namespace Notes.Application.UnitTests.CQRS.Note.Commands.Update;

public class UpdateNoteForUserCommandValidatorTests
{
    [Test]
    public async Task Handle_ValidEntity_IsValid()
    {
        // Arrange
        var updateNoteForUserCommandValidator = new UpdateNoteForUserCommandValidator();
        var updateNoteForUserCommand = Any.Instance<UpdateNoteForUserCommand>();

        // Act
        var result = await updateNoteForUserCommandValidator.ValidateAsync(updateNoteForUserCommand);

        // Assert
        result.IsValid.Should().BeTrue();
    }
    
    [Test]
    public async Task Handle_EmptyTitle_IsInvalid()
    {
        // Arrange
        var updateNoteForUserCommandValidator = new UpdateNoteForUserCommandValidator();
        var createNoteCommand = Any.Instance<UpdateNoteForUserCommand>()
            .WithProperty(x => x.Title, string.Empty);

        // Act
        var result = await updateNoteForUserCommandValidator.ValidateAsync(createNoteCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("'Title' must not be empty.");
    }
    
    [Test]
    public async Task Handle_EmptyContent_IsInvalid()
    {
        // Arrange
        var updateNoteForUserCommandValidator = new UpdateNoteForUserCommandValidator();
        var updateNoteForUserCommand = Any.Instance<UpdateNoteForUserCommand>()
            .WithProperty(x => x.Content, string.Empty);

        // Act
        var result = await updateNoteForUserCommandValidator.ValidateAsync(updateNoteForUserCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("'Content' must not be empty.");
    }
    
    [Test]
    public async Task Handle_TooLongTitle_IsInvalid()
    {
        // Arrange
        var updateNoteForUserCommandValidator = new UpdateNoteForUserCommandValidator();
        var updateNoteForUserCommand = Any.Instance<UpdateNoteForUserCommand>()
            .WithProperty(x => x.Title, Any.String(51));

        // Act
        var result = await updateNoteForUserCommandValidator.ValidateAsync(updateNoteForUserCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("The length of 'Title' must be 50 characters or fewer. You entered 51 characters.");
    }
    
    [Test]
    public async Task Handle_TooLongContent_IsInvalid()
    {
        // Arrange
        var updateNoteForUserCommandValidator = new UpdateNoteForUserCommandValidator();
        var updateNoteForUserCommand = Any.Instance<UpdateNoteForUserCommand>()
            .WithProperty(x => x.Content, Any.String(256));

        // Act
        var result = await updateNoteForUserCommandValidator.ValidateAsync(updateNoteForUserCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.First().ErrorMessage.Should().Be("The length of 'Content' must be 255 characters or fewer. You entered 256 characters.");
    }
}