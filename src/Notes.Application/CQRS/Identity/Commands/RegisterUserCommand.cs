using System.Text.Json.Serialization;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Domain.Contracts.Constants;
using Notes.Domain.Contracts.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<AuthenticationResult>
{
    [JsonIgnore]
    public bool IsAdmin { get; set; }
}

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(4).MaximumLength(50);
        RuleFor(p => p.Password).NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(6).WithMessage("Your password length must be at least 6.")
            .MaximumLength(32).WithMessage("Your password length must not exceed 32.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
    }
}

public class RegisterUserCommandHandler : BaseHandler<RegisterUserCommandHandler>, IRequestHandler<RegisterUserCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;
    private readonly UserManager<IdentityUser> _userManager;


    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, ITokenHandler tokenHandler, UserManager<IdentityUser> userManager, ILogger<RegisterUserCommandHandler> logger) : base(unitOfWork, logger)
    {
        _tokenHandler = tokenHandler;
        _userManager = userManager;
    }

    public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (!_userManager.Users.Any())
        {
            Logger.LogInformation("No users in the database. Creating admin user {Email}", request.Email);
            request.IsAdmin = true;
        }
        
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
        
        await _userManager.AddToRoleAsync(newUser, RoleNames.User);
        if (request.IsAdmin)
        {
            await _userManager.RemoveFromRoleAsync(newUser, RoleNames.User);
            await _userManager.AddToRolesAsync(newUser, new[]{ RoleNames.Admin, RoleNames.User});
        }

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