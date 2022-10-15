using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.Common.Interfaces.Wrappers;
using Notes.Domain.Contracts.Identity;
using Notes.Domain.Contracts.Responses;

namespace Notes.Application.CQRS.Identity.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<AuthenticationResult>;

public class LoginUserCommandHandler : BaseHandler<LoginUserCommandHandler>, IRequestHandler<LoginUserCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;
    private readonly IUserManagerWrapper _userManagerWrapper;

    public LoginUserCommandHandler(IUnitOfWork unitOfWork, ITokenHandler tokenHandler, IUserManagerWrapper userManagerWrapper, ILogger<LoginUserCommandHandler> logger) : base(unitOfWork, logger)
    {
        _tokenHandler = tokenHandler;
        _userManagerWrapper = userManagerWrapper;
    }

    public async Task<AuthenticationResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("User {Email} trying to log in", request.Email);
        var user = await _userManagerWrapper.FindByEmailAsync(request.Email);
        if (user is null)
        {
            return GenerateFailureResponse(request.Email);
        }

        var userHasValidPassword = await _userManagerWrapper.CheckPasswordAsync(user, request.Password);

        if (!userHasValidPassword)
        {
            return GenerateFailureResponse(request.Email);
        }
        
        var token = await _tokenHandler.GenerateToken(user);
        return GenerateSuccessResponse(token, request.Email);
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