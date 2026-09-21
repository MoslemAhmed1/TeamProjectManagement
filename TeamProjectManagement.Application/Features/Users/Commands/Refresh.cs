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
            var storedToken = await refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            var user = storedToken.User;

            var newAccessToken = tokenService.GenerateAccessToken(user);
            var newRefreshTokenString = tokenService.GenerateRefreshToken();

            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = newRefreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // TODO: should be configuration, and obtained from Auth/Token Service
            };

            storedToken.IsRevoked = true;

            await refreshTokenRepository.AddRefreshTokenAsync(newRefreshToken);
            await unitOfWork.SaveChangesAsync();

            return new AuthViewModel
            {
                AccessToken = newAccessToken,
                User = user.ToViewModel()
            };
        }
    }
}
