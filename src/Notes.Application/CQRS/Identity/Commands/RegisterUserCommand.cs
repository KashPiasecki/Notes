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
    [JsonIgnore] public bool IsAdmin { get; set; }
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
    private readonly IUserManagerWrapper _userManagerWrapper;


    public RegisterUserCommandHandler(IUnitOfWork unitOfWork, ITokenHandler tokenHandler, IUserManagerWrapper userManagerWrapper,
        ILogger<RegisterUserCommandHandler> logger) : base(unitOfWork, logger)
    {
        _tokenHandler = tokenHandler;
        _userManagerWrapper = userManagerWrapper;
    }

    public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (!_userManagerWrapper.HasAnyUsers())
        {
            Logger.LogInformation("No users in the database. Creating admin user {Email}", request.Email);
            request.IsAdmin = true;
        }

        Logger.LogInformation("Attempt to register user {Email}", request.Email);
        var exisingUser = await _userManagerWrapper.FindByEmailAsync(request.Email);
        if (exisingUser is not null)
        {
            Logger.LogWarning("User with email {Email} already exists", request.Email);
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "User with this email address already exists" }
            };
        }

        var identityUser = _userManagerWrapper.CreateIdentityUser(request.Email, request.UserName);
        var createdUser = await _userManagerWrapper.CreateAsync(identityUser, request.Password);
        if (!createdUser.Succeeded)
        {
            Logger.LogWarning("Validation error while creating the account");
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = createdUser.Errors.Select(x => x.Description)
            };
        }

        await _userManagerWrapper.AddToRoleAsync(identityUser, RoleNames.User);
        if (request.IsAdmin)
        {
            await _userManagerWrapper.AddToRoleAsync(identityUser, RoleNames.Admin);
        }

        var tokenResponse = await _tokenHandler.GenerateToken(identityUser);
        Logger.LogInformation("Successfully created user {Email}", request.Email);
        return new AuthenticationSuccessResult
        {
            Success = true,
            Token = tokenResponse.Token,
            RefreshToken = tokenResponse.RefreshToken.Token
        };
    }
}