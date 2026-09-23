using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Common
{
    public static class AuthTokenHelper
    {
        public static (AuthViewModel authData, RefreshToken refreshToken, string plainRefreshToken) CreateTokens(
            User user,
            ITokenService tokenService,
            IAuthService authService)
        {
            var accessToken = tokenService.GenerateAccessToken(user);
            var plainRefreshToken = tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                TokenHash = authService.HashToken(plainRefreshToken),
                UserId = user.Id,
                ExpiresAt = tokenService.GetRefreshTokenExpiry()
            };

            var authData = new AuthViewModel
            {
                AccessToken = accessToken,
                User = user.ToViewModel()
            };

            return (authData, refreshToken, plainRefreshToken);
        }
    }
}
