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
using Notes.Infrastructure.Utility.Extensions;

namespace Notes.Api.Controllers;

[ApiController]
[Route($"{ApiRoutes.Base}/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RoleNames.User)]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<IEnumerable<GetNoteDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllNotesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<GetNoteDto>> Get([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetNoteByIdQuery(id));
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult<GetNoteDto>> Update(UpdateNoteCommand updateNoteCommand)
    {
        var result = await _mediator.Send(updateNoteCommand);
        return Ok(result);
    }

    [HttpDelete]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<ActionResult> Delete(DeleteNoteCommand deleteNoteCommand)
    {
        await _mediator.Send(deleteNoteCommand);
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateNoteCommand createNoteCommand)
    {
        createNoteCommand.UserId = HttpContext.GetUserId();
        var result = await _mediator.Send(createNoteCommand);
        return CreatedAtAction(nameof(Get), routeValues: new { id = result.Id }, value: result);
    }

    [HttpGet(ApiRoutes.User)]
    public async Task<ActionResult<IEnumerable<GetNoteDto>>> GetForUser()
    {
        var result = await _mediator.Send(new GetNotesForUserQuery(HttpContext.GetUserId()));
        return Ok(result);
    }

    [HttpPut(ApiRoutes.User)]
    public async Task<ActionResult<GetNoteDto>> UpdateForUser(UpdateNoteCommand updateNoteCommand)
    {
        var result = await _mediator.Send(new UpdateNoteForUserCommand(updateNoteCommand.Id, updateNoteCommand.Title, updateNoteCommand.Content,
            HttpContext.GetUserId()));
        return Ok(result);
    }

    [HttpDelete(ApiRoutes.User)]
    public async Task<ActionResult> DeleteForUser(DeleteNoteCommand deleteNoteCommand)
    {
        await _mediator.Send(new DeleteNoteForUserCommand(deleteNoteCommand.Id, HttpContext.GetUserId()));
        return NoContent();
    }
}