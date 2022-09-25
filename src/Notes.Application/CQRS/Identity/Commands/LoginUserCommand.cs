using MediatR;
using Microsoft.AspNetCore.Identity;
using Notes.Application.Common.Interfaces;
using Notes.Application.Identity;
using Notes.Domain.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthenticationResult>;

public class LoginUserCommandHandler : BaseHandler, IRequestHandler<LoginUserCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;

    private readonly UserManager<IdentityUser> _userManager;

    public LoginUserCommandHandler(IDataContext dataContext, ITokenHandler tokenHandler, UserManager<IdentityUser> userManager) :
        base(dataContext)
    {
        _tokenHandler = tokenHandler;
        _userManager = userManager;
    }

    public async Task<AuthenticationResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return GenerateFailureResponse();
        }

        var userHasValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

        var token = await _tokenHandler.GenerateToken(user);
        return userHasValidPassword
            ? GenerateSuccessResponse(token)
            : GenerateFailureResponse();
    }

    private static AuthenticationResult GenerateFailureResponse()
    {
        return new AuthenticationFailedResult
        {
            Success = false,
            Errors = new[] { "The email or password is invalid" }
        };
    }

    private AuthenticationResult GenerateSuccessResponse(TokenResponse token)
    {
        return new AuthenticationSuccessResult
        {
            Success = true,
            Token = token.Token,
            RefreshToken = token.RefreshToken.Token
        };
    }
}