using MediatR;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResult>;

    public class RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IAuthService authService,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RefreshTokenCommand, AuthResult>
    {
        public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var hashedRequestToken = authService.HashToken(request.RefreshToken);
            var storedToken = await refreshTokenRepository.GetRefreshTokenAsync(hashedRequestToken);

            if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            var user = storedToken.User;

            var (authData, refreshToken, plainRefreshToken) = AuthTokenHelper.CreateTokens(user, tokenService, authService);

            storedToken.IsRevoked = true;

            await refreshTokenRepository.AddRefreshTokenAsync(refreshToken);
            await unitOfWork.SaveChangesAsync();

            return new AuthResult(authData, plainRefreshToken, refreshToken.ExpiresAt);
        }
    }
}
