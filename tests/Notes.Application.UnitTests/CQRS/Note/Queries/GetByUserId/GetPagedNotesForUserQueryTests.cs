using FluentAssertions;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.CQRS.Note.Queries;
using Notes.Application.CQRS.Note.Queries.GetByUserId;
using Notes.Application.UnitTests.TestsUtility.Mapper;
using Notes.Domain.Contracts.Filters;
using Notes.Domain.Contracts.Responses;
using NSubstitute;
using NUnit.Framework;
using TddXt.AnyRoot.Collections;
using TddXt.AnyRoot.Numbers;
using static TddXt.AnyRoot.Root;

namespace Notes.Application.UnitTests.CQRS.Note.Queries.GetByUserId;

public class GetPagedNotesForUserQueryTests
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

        var getPagedNotesForUserQueryHandler =
            new GetPagedNotesForUserQueryHandler(unitOfWork, mapper, paginationHandler, Any.Instance<ILogger<GetPagedNotesForUserQueryHandler>>());
        var getPagedNotesForUserQuery = Any.Instance<GetPagedNotesForUserQuery>();
        var cancellationToken = Any.Instance<CancellationToken>();
        var paginationFilter = Any.Instance<PaginationFilter>();
        paginationHandler.ValidateQuery(getPagedNotesForUserQuery.PaginationFilterQuery).Returns(paginationFilter);
        noteRepository.GetAllForUserAsync(getPagedNotesForUserQuery.UserId, paginationFilter, getPagedNotesForUserQuery.NoteFilterQuery,
            cancellationToken).Returns(Any.Enumerable<Domain.Entities.Note>());
        var totalRecords = Any.Integer();
        noteRepository.CountForUserAsync(getPagedNotesForUserQuery.UserId, getPagedNotesForUserQuery.NoteFilterQuery, cancellationToken)
            .Returns(totalRecords);
        var pagedResponse = Any.Instance<PagedResponse<GetNoteDto>>();
        paginationHandler.CreatePagedResponse(Arg.Any<IEnumerable<GetNoteDto>>(), paginationFilter, totalRecords, getPagedNotesForUserQuery.Route)
            .Returns(pagedResponse);

        // Act
        var result = await getPagedNotesForUserQueryHandler.Handle(getPagedNotesForUserQuery, cancellationToken);

        // Assert
        result.Should().Be(pagedResponse);
    }
}