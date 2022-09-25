using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Identity;
using Notes.Domain.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<AuthenticationResult>;

public class RegisterUserCommandHandler : BaseHandler<RegisterUserCommandHandler>, IRequestHandler<RegisterUserCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterUserCommandHandler(IDataContext dataContext, ITokenHandler tokenHandler, UserManager<IdentityUser> userManager,
        ILogger<RegisterUserCommandHandler> logger) : base(dataContext, logger)
    {
        _tokenHandler = tokenHandler;
        _userManager = userManager;
    }

    public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Attempt to register user {Email}", request.Email);
        var exisingUser = await _userManager.FindByEmailAsync(request.Email);
        if (exisingUser is not null)
        {
            Logger.LogWarning("User with email {Email} already exists", request.Email);
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "User with this email address already exists" }
            };
        }

        var newUser = new IdentityUser
        {
            Email = request.Email,
            UserName = request.UserName
        };
        var createdUser = await _userManager.CreateAsync(newUser, request.Password);

        if (!createdUser.Succeeded)
        {
            Logger.LogWarning("Validation error while creating the account");
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = createdUser.Errors.Select(x => x.Description)
            };
        }

        var tokenResponse = await _tokenHandler.GenerateToken(newUser);
        Logger.LogInformation("Successfully created user {Email}", request.Email);
        return new AuthenticationSuccessResult
        {
            Success = true,
            Token = tokenResponse.Token,
            RefreshToken = tokenResponse.RefreshToken.Token
        };
    }
}