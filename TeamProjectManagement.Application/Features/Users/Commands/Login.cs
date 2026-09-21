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

            var accessToken = tokenService.GenerateAccessToken(user);
            var refreshTokenString = tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // TODO: should be configuration, and obtained from Auth/Token Service
            };

            await refreshTokenRepository.AddRefreshTokenAsync(refreshToken);
            await unitOfWork.SaveChangesAsync();

            return new AuthViewModel
            {
                AccessToken = accessToken,
                User = user.ToViewModel()
            };
        }
    }
}
