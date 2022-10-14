using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Filtering;
using Notes.Application.CQRS.Note.Commands.Create;
using Notes.Application.CQRS.Note.Commands.Delete;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.CQRS.Note.Queries;
using Notes.Application.CQRS.Note.Queries.GetAll;
using Notes.Application.CQRS.Note.Queries.GetById;
using Notes.Application.CQRS.Note.Queries.GetByUserId;
using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts.Constants;
using Notes.Domain.Contracts.Responses;
using Notes.Infrastructure.Cache;
using Notes.Infrastructure.Utility.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace Notes.Api.Controllers;

[ApiController]
[Route($"{ApiRoutes.Base}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.User)]
[Produces("application/json")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IContextInfoProvider _contextInfoProvider;
    

    public NotesController(IMediator mediator, IContextInfoProvider contextInfoProvider)
    {
        _mediator = mediator;
        _contextInfoProvider = contextInfoProvider;
    }

    [HttpGet]
    [Cached(30)]
    [Authorize(Roles = RoleNames.Admin)]
    [SwaggerOperation(Summary = "Get all notes", Description = "Requires admin user role")]
    [SwaggerResponse(200, Type = typeof(PagedResponse<GetNoteDto>), Description = "Get all notes")]
    [SwaggerResponse(401, Description = "Unauthorized operation")]
    [SwaggerResponse(403, Description = "Forbidden operation")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<PagedResponse<GetNoteDto>>> GetAll(
        [FromQuery] PaginationFilterQuery paginationFilterQuery,
        [FromQuery] NoteFilterQuery noteFilterQuery)
    {
        var route = _contextInfoProvider.GetRoute();
        var request = new GetPagedNotesQuery(route, paginationFilterQuery, noteFilterQuery);
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Cached(30)]
    [Authorize(Roles = RoleNames.Admin)]
    [SwaggerOperation(Summary = "Get any note by id", Description = "Requires admin user role")]
    [SwaggerResponse(200, Type = typeof(GetNoteDto), Description = "Get single note")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(403, Description = "Forbidden Operation")]
    [SwaggerResponse(404, Type = typeof(ErrorResponse), Description = "Entity Not Found")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<GetNoteDto>> Get([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetNoteByIdQuery(id));
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = RoleNames.Admin)]
    [SwaggerOperation(Summary = "Edit any note by id", Description = "Requires admin user role")]
    [SwaggerResponse(200, Type = typeof(GetNoteDto), Description = "Edit single note")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(403, Description = "Forbidden Operation")]
    [SwaggerResponse(404, Type = typeof(ErrorResponse), Description = "Entity Not Found")]
    [SwaggerResponse(422, Type = typeof(ErrorResponse), Description = "Validation Error")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<GetNoteDto>> Update(UpdateNoteCommand updateNoteCommand)
    {
        var result = await _mediator.Send(updateNoteCommand);
        return Ok(result);
    }

    [HttpDelete]
    [Authorize(Roles = RoleNames.Admin)]
    [SwaggerOperation(Summary = "Delete any note by id", Description = "Requires admin user role")]
    [SwaggerResponse(200, Type = typeof(GetNoteDto), Description = "Delete single note")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(403, Description = "Forbidden Operation")]
    [SwaggerResponse(404, Type = typeof(ErrorResponse), Description = "Entity Not Found")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult> Delete(DeleteNoteCommand deleteNoteCommand)
    {
        await _mediator.Send(deleteNoteCommand);
        return NoContent();
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create note")]
    [SwaggerResponse(200, Type = typeof(GetNoteDto), Description = "Create note")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(422, Type = typeof(ErrorResponse), Description = "Validation Error")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateNoteCommand createNoteCommand)
    {
        createNoteCommand.UserId = _contextInfoProvider.GetUserId();
        var result = await _mediator.Send(createNoteCommand);
        return CreatedAtAction(nameof(Get), routeValues: new { id = result.Id }, value: result);
    }

    [HttpGet(ApiRoutes.User)]
    [Cached(30)]
    [SwaggerOperation(Summary = "Get notes for user")]
    [SwaggerResponse(200, Type = typeof(PagedResponse<GetNoteDto>), Description = "Get notes for user")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<PagedResponse<GetNoteDto>>> GetAllForUser(
        [FromQuery] PaginationFilterQuery paginationFilterQuery,
        [FromQuery] NoteFilterQuery noteFilterQuery)
    {
        var userId = _contextInfoProvider.GetUserId();
        var route = _contextInfoProvider.GetRoute();
        var request = new GetPagedNotesForUserQuery(userId, paginationFilterQuery, route, noteFilterQuery);
        var result = await _mediator.Send(request);
        return Ok(result);
    }

    [HttpPut(ApiRoutes.User)]
    [SwaggerOperation(Summary = "Edit users note by id")]
    [SwaggerResponse(200, Type = typeof(GetNoteDto), Description = "Edit single note for user")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(404, Type = typeof(ErrorResponse), Description = "Entity Not Found")]
    [SwaggerResponse(422, Type = typeof(ErrorResponse), Description = "Validation Error")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<GetNoteDto>> UpdateForUser(UpdateNoteForUserCommand updateNoteForUserCommand)
    {
        updateNoteForUserCommand.UserId = _contextInfoProvider.GetUserId();
        var result = await _mediator.Send(updateNoteForUserCommand);
        return Ok(result);
    }

    [HttpDelete(ApiRoutes.User)]
    [SwaggerOperation(Summary = "Delete users note by id")]
    [SwaggerResponse(200, Type = typeof(GetNoteDto), Description = "Edit single note for user")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(404, Type = typeof(ErrorResponse), Description = "Entity Not Found")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult> DeleteForUser(DeleteNoteForUserCommand deleteNoteForUserCommand)
    {
        deleteNoteForUserCommand.UserId = _contextInfoProvider.GetUserId();
        await _mediator.Send(deleteNoteForUserCommand);
        return NoContent();
    }
}