using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Common
{
    public record AuthResult(AuthViewModel Data, string RefreshToken, DateTime RefreshTokenExpiration);
}
