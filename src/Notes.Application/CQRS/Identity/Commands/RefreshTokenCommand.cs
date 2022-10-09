using System.IdentityModel.Tokens.Jwt;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces;
using Notes.Application.Identity;
using Notes.Domain.Contracts;
using Notes.Domain.Contracts.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthenticationResult>;

public class RefreshTokenCommandHandler : BaseHandler<RefreshTokenCommandHandler>, IRequestHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;
    private readonly UserManager<IdentityUser> _userManager;

    public RefreshTokenCommandHandler(IDataContext dataContext, ITokenHandler tokenHandler, UserManager<IdentityUser> userManager,
        ILogger<RefreshTokenCommandHandler> logger) : base(dataContext, logger)
    {
        _tokenHandler = tokenHandler;
        _userManager = userManager;
    }

    public async Task<AuthenticationResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Attempting to refresh token");
        var validatedToken = _tokenHandler.GetPrincipalFromToken(request.Token);
        if (validatedToken is null)
        {
            Logger.LogError("Invalid token");
            return GenerateFailureResponse();
        }

        var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type.Equals(JwtRegisteredClaimNames.Exp)).Value);
        var expiryDateTime = _tokenHandler.GetExpirationTime(expiryDateUnix);
        if (expiryDateTime > DateTime.Now)
        {
            Logger.LogWarning("Token hasn't expired yet");
            return GenerateFailureResponse();
        }

        var tokenId = validatedToken.Claims.Single(x => x.Type.Equals(JwtClaimNames.Jti)).Value;
        var storedRefreshToken =
            await DataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token.Equals(request.RefreshToken), cancellationToken: cancellationToken);
        if (storedRefreshToken is null || storedRefreshToken.JwtId != tokenId)
        {
            Logger.LogError("Refresh token doesn't exist");
            return GenerateFailureResponse();
        }

        if (DateTime.Now > storedRefreshToken.ExpireDate)
        {
            Logger.LogWarning("Refresh token has expired");
            return GenerateFailureResponse();
        }

        if (storedRefreshToken.Invalidated)
        {
            Logger.LogWarning("Refresh token is invalidated");
            return GenerateFailureResponse();
        }

        if (storedRefreshToken.Used)
        {
            Logger.LogWarning("Refresh token is used");
            return GenerateFailureResponse();
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

    private static AuthenticationResult GenerateFailureResponse()
    {
        return new AuthenticationFailedResult
        {
            Success = false,
            Errors = new[] { "Invalid token" }
        };
    }
}