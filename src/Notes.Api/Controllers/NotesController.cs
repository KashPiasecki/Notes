using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.CQRS.Note.Commands.Create;
using Notes.Application.CQRS.Note.Commands.Delete;
using Notes.Application.CQRS.Note.Commands.Update;
using Notes.Application.CQRS.Note.Queries;
using Notes.Application.CQRS.Note.Queries.GetAll;
using Notes.Application.CQRS.Note.Queries.GetById;
using Notes.Domain.Contracts;

namespace Notes.Api.Controllers;

[ApiController]
[Route($"{ApiRoutes.Base}/[controller]")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetNoteDto>>> GetAll()
    {
        var result = await _mediator.Send(new GetAllNotesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetNoteDto>> Get([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetNoteByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateNoteCommand createNoteCommand)
    {
        var result = await _mediator.Send(createNoteCommand);
        return CreatedAtAction(nameof(Get), routeValues: new { id = result.Id }, value: result);
    }
    
    [HttpPut]
    public async Task<ActionResult<GetNoteDto>> Update(UpdateNoteCommand updateNoteCommand)
    {
        var result = await _mediator.Send(updateNoteCommand);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(DeleteNoteCommand deleteNoteCommand)
    {
        await _mediator.Send(deleteNoteCommand);
        return NoContent();
    }
}