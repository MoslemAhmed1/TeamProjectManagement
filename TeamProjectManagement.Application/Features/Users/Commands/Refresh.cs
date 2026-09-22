using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthViewModel>;

    public class RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IAuthService authService,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RefreshTokenCommand, AuthViewModel>
    {
        public async Task<AuthViewModel> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var hashedRequestToken = authService.HashToken(request.RefreshToken);
            var storedToken = await refreshTokenRepository.GetRefreshTokenAsync(hashedRequestToken);

            if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            var user = storedToken.User;

            var newAccessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = authService.HashToken(newRefreshToken),
                UserId = user.Id,
                ExpiresAt = tokenService.GetRefreshTokenExpiry()
            };

            storedToken.IsRevoked = true;

            await refreshTokenRepository.AddRefreshTokenAsync(refreshToken);
            await unitOfWork.SaveChangesAsync();

            return new AuthViewModel
            {
                AccessToken = newAccessToken,
                User = user.ToViewModel()
            };
        }
    }
}
