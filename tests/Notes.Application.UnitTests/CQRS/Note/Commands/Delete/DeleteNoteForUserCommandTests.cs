using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Commands.Delete;
using NSubstitute.ReturnsExtensions;

namespace Notes.Application.UnitTests.CQRS.Note.Commands.Delete;

public class DeleteNoteForUserCommandTests
{
    [Test]
    public async Task Handle_NonExistingEntity_ThrowsNotFoundException()
    {
        // Arrange
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);

        var deleteNoteForUserCommandHandler = new DeleteNoteForUserCommandHandler(unitOfWork, Any.Instance<ILogger<DeleteNoteForUserCommandHandler>>());
        var deleteNoteForUserCommand = Any.Instance<DeleteNoteForUserCommand>();
        var cancellationToken = Any.Instance<CancellationToken>();
        noteRepository.GetByIdForUserAsync(deleteNoteForUserCommand.UserId!, deleteNoteForUserCommand.Id, cancellationToken).ReturnsNull();

        // Act 
        Func<Task> act = () => deleteNoteForUserCommandHandler.Handle(deleteNoteForUserCommand, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Note with given id does not exist");
    }
    
    [Test]
    public async Task Handle_ExistingEntity_RemovesNote()
    {
        // Arrange
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);

        var deleteNoteForUserCommandHandler = new DeleteNoteForUserCommandHandler(unitOfWork, Any.Instance<ILogger<DeleteNoteForUserCommandHandler>>());
        var deleteNoteForUserCommand = Any.Instance<DeleteNoteForUserCommand>();
        var cancellationToken = Any.Instance<CancellationToken>();
        var note = Any.Instance<Domain.Entities.Note>();
        noteRepository.GetByIdForUserAsync(deleteNoteForUserCommand.UserId!, deleteNoteForUserCommand.Id, cancellationToken).Returns(note);

        // Act 
        await deleteNoteForUserCommandHandler.Handle(deleteNoteForUserCommand, cancellationToken);

        // Assert
        noteRepository.Received(1).Remove(note);
        await unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
    }
}