using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Commands.Delete;
using NSubstitute.ReturnsExtensions;
using TddXt.AnyRoot.Invokable;

namespace Notes.Application.UnitTests.CQRS.Note.Commands.Delete;

public class DeleteNoteCommandTests
{
    [Test]
    public async Task Handle_NonExistingEntity_ThrowsNotFoundException()
    {
        // Arrange
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);

        var deleteNoteCommandHandler = new DeleteNoteCommandHandler(unitOfWork, Any.Instance<ILogger<DeleteNoteCommandHandler>>());
        var deleteNoteCommand = Any.Instance<DeleteNoteCommand>();
        var cancellationToken = Any.CancellationToken();
        noteRepository.GetByIdAsync(deleteNoteCommand.Id, cancellationToken).ReturnsNull();

        // Act 
        Func<Task> act = () => deleteNoteCommandHandler.Handle(deleteNoteCommand, cancellationToken);
        
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

        var deleteNoteCommandHandler = new DeleteNoteCommandHandler(unitOfWork, Any.Instance<ILogger<DeleteNoteCommandHandler>>());
        var deleteNoteCommand = Any.Instance<DeleteNoteCommand>();
        var cancellationToken = Any.CancellationToken();
        var note = Any.Instance<Domain.Entities.Note>();
        noteRepository.GetByIdAsync(deleteNoteCommand.Id, cancellationToken).Returns(note);

        // Act 
        await deleteNoteCommandHandler.Handle(deleteNoteCommand, cancellationToken);
        
        // Assert
        noteRepository.Received(1).Remove(note);
        await unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
    }
}