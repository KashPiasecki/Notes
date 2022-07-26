using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.UnitTests.TestsUtility.Mapper;
using NSubstitute.ReturnsExtensions;
using TddXt.AnyRoot.Invokable;

namespace Notes.Application.UnitTests.CQRS.Note.Commands.Update;


public class UpdateNoteCommandTests
{
    [Test]
    public async Task Handle_NonExistingEntity_ThrowsNotFoundException()
    {
        // Arrange
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);
        var mapper = TestMapperFactory.GetTestMapper();

        var updateNoteCommandHandler = new UpdateNoteCommandHandler(unitOfWork, mapper,Any.Instance<ILogger<UpdateNoteCommandHandler>>());
        var updateNoteCommand = Any.Instance<UpdateNoteCommand>();
        var cancellationToken = Any.CancellationToken();
        noteRepository.GetByIdAsync(updateNoteCommand.Id, cancellationToken).ReturnsNull();

        // Act 
        Func<Task> act = () => updateNoteCommandHandler.Handle(updateNoteCommand, cancellationToken);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Note with given id does not exist");
    }
    
    [Test]
    public async Task Handle_ExistingEntity_UpdatesNote()
    {
        // Arrange
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);
        var mapper = TestMapperFactory.GetTestMapper();

        var updateNoteCommandHandler = new UpdateNoteCommandHandler(unitOfWork, mapper,Any.Instance<ILogger<UpdateNoteCommandHandler>>());
        var updateNoteCommand = Any.Instance<UpdateNoteCommand>();
        var cancellationToken = Any.CancellationToken();
        var note = Any.Instance<Domain.Entities.Note>();
        noteRepository.GetByIdAsync(updateNoteCommand.Id, cancellationToken).Returns(note);

        // Act 
        var result = await updateNoteCommandHandler.Handle(updateNoteCommand, cancellationToken);
        
        // Assert
        await unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
        result.Title.Should().Be(updateNoteCommand.Title);
        result.Content.Should().Be(updateNoteCommand.Content);
    }
}