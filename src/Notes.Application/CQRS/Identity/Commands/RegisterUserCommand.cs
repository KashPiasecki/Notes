using MediatR;
using Microsoft.AspNetCore.Identity;
using Notes.Application.Common.Interfaces;
using Notes.Application.Identity;
using Notes.Domain.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<AuthenticationResult>;

public class RegisterUserCommandHandler : BaseHandler, IRequestHandler<RegisterUserCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterUserCommandHandler(IDataContext dataContext, ITokenHandler tokenHandler, UserManager<IdentityUser> userManager) : base(dataContext)
    {
        _tokenHandler = tokenHandler;
        _userManager = userManager;
    }

    public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var exisingUser = await _userManager.FindByEmailAsync(request.Email);
        if (exisingUser is not null)
        {
            return GenerateFailureByExistResponse();
        }
        
        var newUser = new IdentityUser
        {
            Email = request.Email,
            UserName = request.UserName
        };
        var createdUser = await _userManager.CreateAsync(newUser, request.Password);
        
        if (!createdUser.Succeeded)
        {
            return GenerateFailureByPasswordRequirementsResponse(createdUser);
        }

        var tokenResponse = await _tokenHandler.GenerateToken(newUser);
        return GenerateSuccessResponse(tokenResponse);
    }

    private static AuthenticationResult GenerateFailureByPasswordRequirementsResponse(IdentityResult createdUser)
    {
        return new AuthenticationFailedResult
        {
            Success = false,
            Errors = createdUser.Errors.Select(x => x.Description)
        };
    }

    private static AuthenticationResult GenerateFailureByExistResponse()
    {
        return new AuthenticationFailedResult
        {
            Success = false,
            Errors = new[] { "User with this email address already exists" }
        };
    }

    private static AuthenticationResult GenerateSuccessResponse(TokenResponse token)
    {
        return new AuthenticationSuccessResult
        {
            Success = true,
            Token = token.Token,
            RefreshToken = token.RefreshToken.Token
        };
    }
}