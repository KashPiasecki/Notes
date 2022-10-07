using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.CQRS.Note.Commands.Create;
using Notes.Application.CQRS.Note.Commands.Delete;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.CQRS.Note.Queries;
using Notes.Application.CQRS.Note.Queries.GetAll;
using Notes.Application.CQRS.Note.Queries.GetById;
using Notes.Application.CQRS.Note.Queries.GetByUserId;
using Notes.Domain.Contracts;
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

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Cached(300)]
    [Authorize(Roles = RoleNames.Admin)]
    [SwaggerOperation(Summary = "Get all notes", Description = "Requires admin user role")]
    [SwaggerResponse(200, Type = typeof(IEnumerable<GetNoteDto>), Description = "Get all notes")]
    [SwaggerResponse(401, Description = "Unauthorized operation")]
    [SwaggerResponse(403, Description = "Forbidden operation")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<IEnumerable<GetNoteDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllNotesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Cached(300)]
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
        createNoteCommand.UserId = HttpContext.GetUserId();
        var result = await _mediator.Send(createNoteCommand);
        return CreatedAtAction(nameof(Get), routeValues: new { id = result.Id }, value: result);
    }

    [HttpGet(ApiRoutes.User)]
    [Cached(300)]
    [SwaggerOperation(Summary = "Get notes for user")]
    [SwaggerResponse(200, Type = typeof(IEnumerable<GetNoteDto>), Description = "Get notes for user")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<IEnumerable<GetNoteDto>>> GetForUser()
    {
        var result = await _mediator.Send(new GetNotesForUserQuery(HttpContext.GetUserId()));
        return Ok(result);
    }

    [HttpPut(ApiRoutes.User)]
    [SwaggerOperation(Summary = "Edit users note by id")]
    [SwaggerResponse(200, Type = typeof(GetNoteDto), Description = "Edit single note for user")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(404, Type = typeof(ErrorResponse), Description = "Entity Not Found")]
    [SwaggerResponse(422, Type = typeof(ErrorResponse), Description = "Validation Error")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult<GetNoteDto>> UpdateForUser(UpdateNoteCommand updateNoteCommand)
    {
        var result = await _mediator.Send(new UpdateNoteForUserCommand(updateNoteCommand.Id, updateNoteCommand.Title, updateNoteCommand.Content,
            HttpContext.GetUserId()));
        return Ok(result);
    }

    [HttpDelete(ApiRoutes.User)]
    [SwaggerOperation(Summary = "Delete users note by id")]
    [SwaggerResponse(200, Type = typeof(GetNoteDto), Description = "Edit single note for user")]
    [SwaggerResponse(401, Description = "Unauthorized Operation")]
    [SwaggerResponse(404, Type = typeof(ErrorResponse), Description = "Entity Not Found")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<ActionResult> DeleteForUser(DeleteNoteCommand deleteNoteCommand)
    {
        await _mediator.Send(new DeleteNoteForUserCommand(deleteNoteCommand.Id, HttpContext.GetUserId()));
        return NoContent();
    }
}