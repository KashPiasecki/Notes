using FluentAssertions;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Commands.Create;
using Notes.Application.UnitTests.TestsUtility.Mapper;
using NSubstitute;
using NUnit.Framework;
using TddXt.AnyRoot.Builder;

namespace Notes.Application.UnitTests.CQRS.Note.Commands.Create;

public class CreateNoteCommandTests
{
    [Test]
    public async Task Handle_Called_CreatesNote()
    {
        // Arrange
        var mapper = TestMapperFactory.GetTestMapper();
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);

        var createNoteCommandHandler = new CreateNoteCommandHandler(unitOfWork, mapper, Any.Instance<ILogger<CreateNoteCommandHandler>>());
        var createNoteCommand = Any.Instance<CreateNoteCommand>();
        var cancellationToken = Any.Instance<CancellationToken>();
        var note = Any.Instance<Domain.Entities.Note>()
            .WithProperty(x => x.Content, createNoteCommand.Content)
            .WithProperty(x => x.Title, createNoteCommand.Title)
            .WithProperty(x => x.UserId, createNoteCommand.UserId);
        noteRepository.AddAsync(Arg.Any<Domain.Entities.Note>(), cancellationToken).Returns(note);

        // Act
        var result = await createNoteCommandHandler.Handle(createNoteCommand, cancellationToken);

        // Assert
        await unitOfWork.Received(1).SaveChangesAsync(cancellationToken);
        result.Title.Should().Be(createNoteCommand.Title);
        result.Content.Should().Be(createNoteCommand.Content);
        result.UserId.Should().Be(createNoteCommand.UserId);
    }
}