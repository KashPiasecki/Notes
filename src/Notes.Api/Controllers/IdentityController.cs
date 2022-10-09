using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Notes.Application.CQRS.Identity.Commands;
using Notes.Domain.Contracts;
using Notes.Domain.Contracts.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace Notes.Api.Controllers;

[ApiController]
[Route($"{ApiRoutes.Base}/[controller]")]
[Produces("application/json")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;

    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("registerAdmin")]
    [Authorize(Roles = RoleNames.Admin)]
    [SwaggerOperation(Summary = "Creates admin user", Description = "Requires admin user role")]
    [SwaggerResponse(200, Type = typeof(AuthenticationSuccessResult), Description = "Register operation")]
    [SwaggerResponse(401, Description = "Unauthorized operation")]
    [SwaggerResponse(403, Description = "Forbidden operation")]
    [SwaggerResponse(404, Type = typeof(AuthenticationFailedResult), Description = "Unsuccessful operation")]
    [SwaggerResponse(422, Type = typeof(ErrorResponse), Description = "Validation Error")]
    [SwaggerResponse(500, Description = "Internal Server Error")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUserCommand registerUserCommand)
    {
        registerUserCommand.IsAdmin = true;
        return await Register(registerUserCommand);
    }
        
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Creates user", Description = "On first call it will register admin user")]
    [SwaggerResponse(200, Type = typeof(AuthenticationSuccessResult), Description = "Register operation")]
    [SwaggerResponse(404, Type = typeof(AuthenticationFailedResult), Description = "Unsuccessful operation")]
    [SwaggerResponse(422, Type = typeof(ErrorResponse), Description = "Validation Error")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand registerUserCommand)
    {
        var result = await _mediator.Send(registerUserCommand);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Login user")]
    [SwaggerResponse(200, Type = typeof(AuthenticationSuccessResult), Description = "Login operation")]
    [SwaggerResponse(404, Type = typeof(AuthenticationFailedResult), Description = "Unsuccessful operation")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand loginUserCommand)
    {
        var result = await _mediator.Send(loginUserCommand);
        return result.Success ? Ok(result) : BadRequest(result);
    }
    
    [HttpPost("refreshToken")]
    [SwaggerOperation(Summary = "Refresh token")]
    [SwaggerResponse(200, Type = typeof(AuthenticationSuccessResult), Description = "Refresh token operation")]
    [SwaggerResponse(404, Type = typeof(AuthenticationFailedResult), Description = "Unsuccessful operation")]
    [SwaggerResponse(500, Type = typeof(ErrorResponse), Description = "Internal Server Error")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        var result = await _mediator.Send(refreshTokenCommand);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}