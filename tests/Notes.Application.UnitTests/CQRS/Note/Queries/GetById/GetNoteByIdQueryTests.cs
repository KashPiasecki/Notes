using FluentAssertions;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Exceptions;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Queries.GetById;
using Notes.Application.UnitTests.TestsUtility.Mapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using static TddXt.AnyRoot.Root;

using NUnit.Framework;

namespace Notes.Application.UnitTests.CQRS.Note.Queries.GetById;

public class GetNoteByIdQueryTests
{
    [Test]
    public async Task Handle_NonExistingEntity_ThrowsNotFoundException()
    {
        // Arrange
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);
        var mapper = TestMapperFactory.GetTestMapper();

        var getNoteByIdQueryHandler = new GetNoteByIdQueryHandler(unitOfWork, mapper,Any.Instance<ILogger<GetNoteByIdQueryHandler>>());
        var getNoteByIdQuery = Any.Instance<GetNoteByIdQuery>();
        var cancellationToken = Any.Instance<CancellationToken>();
        noteRepository.GetNoteByIdAsync(getNoteByIdQuery.Id, cancellationToken).ReturnsNull();

        // Act 
        Func<Task> act = () => getNoteByIdQueryHandler.Handle(getNoteByIdQuery, cancellationToken);
        
        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Note with given id does not exist");
    }
    
    [Test]
    public async Task Handle_ExistingEntity_UpdatesNote()
    {
        // Arrange
        // Arrange
        var noteRepository = Substitute.For<INoteRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);
        var mapper = TestMapperFactory.GetTestMapper();

        var getNoteByIdQueryHandler = new GetNoteByIdQueryHandler(unitOfWork, mapper,Any.Instance<ILogger<GetNoteByIdQueryHandler>>());
        var getNoteByIdQuery = Any.Instance<GetNoteByIdQuery>();
        var cancellationToken = Any.Instance<CancellationToken>();
        var note = Any.Instance<Domain.Entities.Note>();
        noteRepository.GetByIdAsync(getNoteByIdQuery.Id, cancellationToken).Returns(note);

        // Act 
        var result = await getNoteByIdQueryHandler.Handle(getNoteByIdQuery, cancellationToken);
        
        // Assert
        result.Title.Should().Be(note.Title);
        result.Content.Should().Be(note.Content);
    }
}