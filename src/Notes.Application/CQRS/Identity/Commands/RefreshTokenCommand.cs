using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Notes.Application.Common.Interfaces;
using Notes.Application.Identity;
using Notes.Domain.Contracts;
using Notes.Domain.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthenticationResult>;

public class RefreshTokenCommandHandler : BaseHandler, IRequestHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;
    private readonly UserManager<IdentityUser> _userManager;
    
    public RefreshTokenCommandHandler(IDataContext dataContext, ITokenHandler tokenHandler, UserManager<IdentityUser> userManager) : base(dataContext)
    {
        _tokenHandler = tokenHandler;
        _userManager = userManager;
    }

    public async Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var validatedToken = _tokenHandler.GetPrincipalFromToken(request.Token);
        if (validatedToken is null)
        {
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "Invalid token" }
            };
        }

        var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type.Equals(JwtRegisteredClaimNames.Exp)).Value);
        var expiryDateTime = _tokenHandler.GetExpirationTime(expiryDateUnix);
        var localTime = DateTime.Now;
        if (expiryDateTime > localTime)
        {
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "This token hasn't expired yet" }
            };
        }

        var tokenId = validatedToken.Claims.Single(x => x.Type.Equals(JwtClaimNames.Jti)).Value;
        
        var storedRefreshToken = await DataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token.Equals(request.RefreshToken));
        if (storedRefreshToken.JwtId != tokenId)
        {
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "This refresh token doesn't exist" }
            };
        }
            
        if (storedRefreshToken is null)
        {
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "This refresh token doesn't exist" }
            };
        }

        if (DateTime.Now > storedRefreshToken.ExpireDate)
        {
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "This refresh has expired " }
            };
        }

        if (storedRefreshToken.Invalidated)
        {
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "This refresh token is invalidated" }
            };
        }

        if (storedRefreshToken.Used)
        {
            return new AuthenticationFailedResult
            {
                Success = false,
                Errors = new[] { "This refresh token is used" }
            };
        }

        storedRefreshToken.Used = true;
        DataContext.RefreshTokens.Update(storedRefreshToken);
        await DataContext.SaveChangesAsync(cancellationToken);

        var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type.Equals(JwtClaimNames.UserId)).Value);
        var tokenResponse = await _tokenHandler.GenerateToken(user);

        return new AuthenticationSuccessResult
        {
            Success = true,
            Token = tokenResponse.Token,
            RefreshToken = tokenResponse.RefreshToken.Token
        };
    }
}