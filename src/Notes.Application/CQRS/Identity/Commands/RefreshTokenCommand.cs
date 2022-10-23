using MediatR;
using Microsoft.Extensions.Logging;
using Notes.Application.Common.Interfaces.Handlers;
using Notes.Application.Common.Interfaces.Providers;
using Notes.Application.Common.Interfaces.Repositories;
using Notes.Application.Common.Interfaces.Wrappers;
using Notes.Domain.Contracts.Identity;

namespace Notes.Application.CQRS.Identity.Commands;

public record RefreshTokenCommand(string Token, string RefreshToken) : IRequest<AuthenticationResult>;

public class RefreshTokenCommandHandler : BaseHandler<RefreshTokenCommandHandler>, IRequestHandler<RefreshTokenCommand, AuthenticationResult>
{
    private readonly ITokenHandler _tokenHandler;
    private readonly IClaimsPrincipalInfoProvider _claimsPrincipalInfoProvider;
    private readonly IUserManagerWrapper _userManagerWrapper;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenHandler tokenHandler, IClaimsPrincipalInfoProvider claimsPrincipalInfoProvider,
        IUserManagerWrapper userManagerWrapper, ILogger<RefreshTokenCommandHandler> logger) : base(unitOfWork, logger)
    {
        _tokenHandler = tokenHandler;
        _userManagerWrapper = userManagerWrapper;
        _claimsPrincipalInfoProvider = claimsPrincipalInfoProvider;
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

        var expiryDateTime = _claimsPrincipalInfoProvider.GetExpiryTime(validatedToken);
        if (expiryDateTime > DateTime.Now)
        {
            Logger.LogWarning("Token hasn't expired yet");
            return GenerateFailureResponse();
        }

        var tokenId = _claimsPrincipalInfoProvider.GetId(validatedToken);
        var storedRefreshToken = await UnitOfWork.RefreshTokens.GetAsync(request.RefreshToken, cancellationToken);
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
        UnitOfWork.RefreshTokens.Update(storedRefreshToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        var userId = _claimsPrincipalInfoProvider.GetUserId(validatedToken);
        var user = await _userManagerWrapper.FindByIdAsync(userId);
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