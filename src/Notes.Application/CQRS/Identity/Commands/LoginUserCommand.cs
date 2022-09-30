using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Identity;
using Notes.Domain.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthenticationResult>;

public class LoginUserCommandHandler : BaseHandler<LoginUserCommandHandler>, IRequestHandler<LoginUserCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;
    private readonly UserManager<IdentityUser> _userManager;

    public LoginUserCommandHandler(IDataContext dataContext, ITokenHandler tokenHandler, UserManager<IdentityUser> userManager, ILogger<LoginUserCommandHandler> logger) :
        base(dataContext, logger)
    {
        _tokenHandler = tokenHandler;
        _userManager = userManager;
    }

    public async Task<AuthenticationResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("User {Email} trying to log in", request.Email);
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return GenerateFailureResponse(request.Email);
        }

        var userHasValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        var token = await _tokenHandler.GenerateToken(user);
        return userHasValidPassword
            ? GenerateSuccessResponse(token, request.Email)
            : GenerateFailureResponse(request.Email);
    }

    private AuthenticationResult GenerateFailureResponse(string email)
    {
        Logger.LogInformation("Unsuccessful login {Email}", email);
        return new AuthenticationFailedResult
        {
            Success = false,
            Errors = new[] { "The email or password is invalid" }
        };
    }

    private AuthenticationResult GenerateSuccessResponse(TokenResponse token, string email)
    {
        Logger.LogInformation("Successful login {Email}", email);
        return new AuthenticationSuccessResult
        {
            Success = true,
            Token = token.Token,
            RefreshToken = token.RefreshToken.Token
        };
    }
}