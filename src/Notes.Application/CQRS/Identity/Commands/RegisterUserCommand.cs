using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Notes.Application.Common.Interfaces;
using Notes.Domain.Configurations;
using Notes.Domain.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record RegisterUserCommand(string UserName, string Email, string Password) : IRequest<AuthenticationResult>;

public class RegisterUserCommandHandler : BaseHandler, IRequestHandler<RegisterUserCommand, AuthenticationResult>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtConfiguration _jwtConfiguration;

    public RegisterUserCommandHandler(IDataContext dataContext, UserManager<IdentityUser> userManager, JwtConfiguration jwtConfiguration) : base(dataContext)
    {
        _userManager = userManager;
        _jwtConfiguration = jwtConfiguration;
    }

    public async Task<AuthenticationResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var exisingUser = await _userManager.FindByEmailAsync(request.Email);
        if (exisingUser is not null)
        {
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
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = createdUser.Errors.Select(x => x.Description)
            };
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtConfiguration.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , newUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, newUser.Email),
                new Claim(JwtRegisteredClaimNames.Iss, newUser.Id)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthenticationSuccessResult {
            Success = true,
            Token = tokenHandler.WriteToken(token)
        };
    }
}