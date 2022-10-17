using FluentAssertions;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Queries;
using Notes.Application.CQRS.Note.Queries.GetAll;
using Notes.Application.UnitTests.TestsUtility.Mapper;
using Notes.Domain.Contracts.Filters;
using Notes.Domain.Contracts.Responses;
using NSubstitute;
using NUnit.Framework;
using TddXt.AnyRoot.Collections;
using TddXt.AnyRoot.Numbers;

namespace Notes.Application.UnitTests.CQRS.Note.Queries.GetAll;

public class GetPagedNotesQueryTests
{
    [Test]
    public async Task Handle_Called_ReturnsPagedNotes()
    {
        // Arrange
        var mapper = TestMapperFactory.GetTestMapper();
        var noteRepository = Substitute.For<INoteRepository>();
        var paginationHandler = Substitute.For<IPaginationHandler>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.Notes.Returns(noteRepository);

        var getAllNotesQueryHandler =
            new GetAllNotesQueryHandler(unitOfWork, mapper, paginationHandler, Any.Instance<ILogger<GetAllNotesQueryHandler>>());
        var getPagedNotesQuery = Any.Instance<GetPagedNotesQuery>();
        var cancellationToken = Any.Instance<CancellationToken>();
        var paginationFilter = Any.Instance<PaginationFilter>();
        paginationHandler.ValidateQuery(getPagedNotesQuery.PaginationFilterQuery).Returns(paginationFilter);
        noteRepository.GetAllAsync(paginationFilter, getPagedNotesQuery.NoteFilterQuery, cancellationToken).Returns(Any.Enumerable<Domain.Entities.Note>());
        var totalRecords = Any.Integer();
        noteRepository.CountAsync(getPagedNotesQuery.NoteFilterQuery, cancellationToken).Returns(totalRecords);
        var pagedResponse = Any.Instance<PagedResponse<GetNoteDto>>();
        paginationHandler.CreatePagedResponse(Arg.Any<IEnumerable<GetNoteDto>>(), paginationFilter, totalRecords, getPagedNotesQuery.Route)
            .Returns(pagedResponse);
        
        // Act
        var result = await getAllNotesQueryHandler.Handle(getPagedNotesQuery, cancellationToken);

        // Assert
        result.Should().Be(pagedResponse);
    }
}