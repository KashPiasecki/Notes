using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.UnitTests.TestsUtility.Mapper;
using NSubstitute.ReturnsExtensions;

namespace Notes.Application.UnitTests.CQRS.Note.Commands.Update;

public class UpdateNoteForUserCommandTests
{
    [Test]
    public async Task Handle_NonExistingEntity_ThrowsNotFoundException()
    {
        // Arrange
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);
        var mapper = TestMapperFactory.GetTestMapper();

        var updateNoteForUserCommandHandler =
            new UpdateNoteForUserCommandHandler(unitOfWork, mapper, Any.Instance<ILogger<UpdateNoteForUserCommandHandler>>());
        var updateNoteForUserCommand = Any.Instance<UpdateNoteForUserCommand>();
        var cancellationToken = Any.Instance<CancellationToken>();
        noteRepository.GetByIdForUserAsync(updateNoteForUserCommand.UserId!, updateNoteForUserCommand.Id, cancellationToken).ReturnsNull();

        // Act 
        Func<Task> act = () => updateNoteForUserCommandHandler.Handle(updateNoteForUserCommand, cancellationToken);

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
        var mapper = TestMapperFactory.GetTestMapper();

        var updateNoteForUserCommandHandler =
            new UpdateNoteForUserCommandHandler(unitOfWork, mapper, Any.Instance<ILogger<UpdateNoteForUserCommandHandler>>());
        var updateNoteForUserCommand = Any.Instance<UpdateNoteForUserCommand>();
        var cancellationToken = Any.Instance<CancellationToken>();
        var note = Any.Instance<Domain.Entities.Note>();
        noteRepository.GetByIdForUserAsync(updateNoteForUserCommand.UserId!, updateNoteForUserCommand.Id, cancellationToken).Returns(note);

        // Act 
        var result = await updateNoteForUserCommandHandler.Handle(updateNoteForUserCommand, cancellationToken);

        // Assert
        await unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
        await unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
        result.Title.Should().Be(updateNoteForUserCommand.Title);
        result.Content.Should().Be(updateNoteForUserCommand.Content);
    }
}