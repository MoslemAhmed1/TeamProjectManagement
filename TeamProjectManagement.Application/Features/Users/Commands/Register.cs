using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record RegisterUserCommand(string Username, string Email, string Password) : IRequest<AuthViewModel>;

    public class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuthService authService,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<RegisterUserCommand, AuthViewModel>
    {
        public async Task<AuthViewModel> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var username = request.Username.Trim();
            var email = request.Email.Trim().ToLowerInvariant();

            var (usernameExists, emailExists) = await userRepository.ExistsByUsernameOrEmailAsync(username, email);
            if (usernameExists) throw new ConflictException("Username is already taken.");
            if (emailExists) throw new ConflictException("Email is already registered.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Email = email,
                PasswordHash = authService.HashPassword(request.Password)
            };

            var newAccessToken = tokenService.GenerateAccessToken(user);
            var newRefreshToken = tokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = authService.HashToken(newRefreshToken),
                UserId = user.Id,
                ExpiresAt = tokenService.GetRefreshTokenExpiry()
            };

            await unitOfWork.BeginTransactionAsync();
            try
            {
                await userRepository.AddUserAsync(user);
                await refreshTokenRepository.AddRefreshTokenAsync(refreshToken);
                await unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return new AuthViewModel
            {
                AccessToken = newAccessToken,
                User = user.ToViewModel()
            };
        }
    }
}
