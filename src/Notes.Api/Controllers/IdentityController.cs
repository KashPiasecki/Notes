using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Contracts;

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

    [HttpPost("registerAdmin")]
    [Authorize(Roles = RoleNames.Admin)]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserCommand registerUserCommand)
    {
        registerUserCommand.IsAdmin = true;
        return await Register(registerUserCommand);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand registerUserCommand)
    {
        var result = await _mediator.Send(registerUserCommand);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand loginUserCommand)
    {
        var result = await _mediator.Send(loginUserCommand);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    [HttpPost("refreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        var result = await _mediator.Send(refreshTokenCommand);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}