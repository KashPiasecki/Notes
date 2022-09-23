using MediatR;
using Microsoft.AspNetCore.Identity;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Configurations;
using Notes.Domain.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<AuthenticationResult>;

public class RegisterUserCommandHandler : BaseHandler, IRequestHandler<RegisterUserCommand, AuthenticationResult>
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtConfiguration _jwtConfiguration;

    public RegisterUserCommandHandler(IDataContext dataContext, ITokenGenerator tokenGenerator, UserManager<IdentityUser> userManager, JwtConfiguration jwtConfiguration) : base(dataContext)
    {
        _tokenGenerator = tokenGenerator;
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration;
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

        var token = _tokenGenerator.GenerateToken(newUser);
        return GenerateSuccessResponse(token);
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

    private static AuthenticationResult GenerateSuccessResponse(string token)
    {
        return new AuthenticationSuccessResult
        {
            Success = true,
            Token = token
        };
    }
}