using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Contracts;
using Notes.Domain.Identity;

namespace Notes.Api.Controllers;

[ApiController]
[Route($"{ApiRoutes.Base}/[controller]")]
public class IdentityController : ControllerBase 
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand registerUserCommand)
    {
        var result = await _mediator.Send(registerUserCommand);
        return result.Success ? Ok(((AuthenticationSuccessResult)result).Token) : BadRequest(((AuthenticationFailedResult)result).Errors);
    }
}