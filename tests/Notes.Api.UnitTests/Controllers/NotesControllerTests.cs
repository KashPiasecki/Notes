using MediatR;
using Notes.Api.Controllers;
using Notes.Application.Common.Interfaces.Providers;
using Notes.Application.CQRS.Note.Commands.Create;
using Notes.Application.CQRS.Note.Commands.Delete;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.CQRS.Note.Queries;
using Notes.Application.CQRS.Note.Queries.GetAll;
using Notes.Application.CQRS.Note.Queries.GetById;
using Notes.Application.CQRS.Note.Queries.GetByUserId;
using TddXt.AnyRoot.Builder;
using TddXt.AnyRoot.Strings;

namespace Notes.Api.UnitTests.Controllers;

public class NotesControllerTests
{
    [Test]
    public async Task GetAll_Called_SendsQuery()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var contextInfoProvider = Substitute.For<IContextInfoProvider>();
        var route = Any.String();
        var getPagedNotesQuery = Any.Instance<GetPagedNotesQuery>()
            .WithProperty(x => x.Route, route);

        var notesController = new NotesController(mediator, contextInfoProvider);
        contextInfoProvider.GetRoute().Returns(route);

        // Act
        await notesController.GetAll(getPagedNotesQuery.PaginationFilterQuery, getPagedNotesQuery.NoteFilterQuery);
        
        // Assert
        await mediator.Received(1).Send(getPagedNotesQuery);
    
    }
    
    [Test]
    public async Task Get_Called_SendsQuery()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var contextInfoProvider = Substitute.For<IContextInfoProvider>();
        var getNoteByIdQuery = Any.Instance<GetNoteByIdQuery>();
    
        var notesController = new NotesController(mediator, contextInfoProvider);
    
        // Act
        await notesController.Get(getNoteByIdQuery.Id);
    
        // Assert
        await mediator.Received(1).Send(getNoteByIdQuery);
    }
    
    [Test]
    public async Task Update_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var contextInfoProvider = Substitute.For<IContextInfoProvider>();
        var updateNoteCommand = Any.Instance<UpdateNoteCommand>();
    
        var notesController = new NotesController(mediator, contextInfoProvider);
    
        // Act
        await notesController.Update(updateNoteCommand);
    
        // Assert
        await mediator.Received(1).Send(updateNoteCommand);
    }
    
    [Test]
    public async Task Delete_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var contextInfoProvider = Substitute.For<IContextInfoProvider>();
        var deleteNoteCommand = Any.Instance<DeleteNoteCommand>();
    
        var notesController = new NotesController(mediator, contextInfoProvider);
    
        // Act
        await notesController.Delete(deleteNoteCommand);
    
        // Assert
        await mediator.Received(1).Send(deleteNoteCommand);
    }
    
    [Test]
    public async Task Create_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var contextInfoProvider = Any.Instance<IContextInfoProvider>();
        var createNoteCommand = Any.Instance<CreateNoteCommand>();
        
        var notesController = new NotesController(mediator, contextInfoProvider);
        mediator.Send(createNoteCommand).Returns(Any.Instance<GetNoteDto>());
    
        // Act
        await notesController.Create(createNoteCommand);
    
        // Assert
        await mediator.Received(1).Send(createNoteCommand);
    }
    
    [Test]
    public async Task GetForUser_Called_SendsQuery()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var contextInfoProvider = Substitute.For<IContextInfoProvider>();
        var userId = Any.String();
        var route = Any.String();
        var getPagedNotesForUserQuery = Any.Instance<GetPagedNotesForUserQuery>()
            .WithProperty(x => x.UserId, userId)
            .WithProperty(x => x.Route, route);

        var notesController = new NotesController(mediator, contextInfoProvider);
        contextInfoProvider.GetUserId().Returns(userId);
        contextInfoProvider.GetRoute().Returns(route);
    
        // Act
        await notesController.GetAllForUser(getPagedNotesForUserQuery.PaginationFilterQuery, getPagedNotesForUserQuery.NoteFilterQuery);
        
        // Assert
        await mediator.Received(1).Send(getPagedNotesForUserQuery);
    }
    
    [Test]
    public async Task UpdateForUser_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var contextInfoProvider = Substitute.For<IContextInfoProvider>();
        var userId = Any.String();
        var updateNoteForUserCommand = Any.Instance<UpdateNoteForUserCommand>()
            .WithProperty(x => x.UserId, userId);
        
        var notesController = new NotesController(mediator, contextInfoProvider);
        contextInfoProvider.GetUserId().Returns(userId);
    
        // Act
        await notesController.UpdateForUser(updateNoteForUserCommand);
        
        // Assert
        await mediator.Received(1).Send(updateNoteForUserCommand);
    }
    
    [Test]
    public async Task DeleteForUser_Called_SendsCommand()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        var contextInfoProvider = Substitute.For<IContextInfoProvider>();
        var userId = Any.String();
        var deleteNoteForUserCommand = Any.Instance<DeleteNoteForUserCommand>()
            .WithProperty(x => x.UserId, userId);
        var notesController = new NotesController(mediator, contextInfoProvider);
    
        // Act
        await notesController.DeleteForUser(deleteNoteForUserCommand);
        
        // Assert
        await mediator.Received(1).Send(Arg.Any<DeleteNoteForUserCommand>());
    }
}