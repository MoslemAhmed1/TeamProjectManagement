using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record LoginUserCommand(string Identifier, string Password) : IRequest<AuthViewModel>;

    public class LoginUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuthService authService,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<LoginUserCommand, AuthViewModel>
    {
        public async Task<AuthViewModel> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdentifierAsync(request.Identifier.Trim());
            if (user is null || !authService.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedException("Invalid credentials.");

            var newAccessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = authService.HashToken(newRefreshToken),
                UserId = user.Id,
                ExpiresAt = tokenService.GetRefreshTokenExpiry()
            };

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
