using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record RegisterUserCommand(
        [Required] [StringLength(50, MinimumLength = 3)] string Username,
        [Required] [EmailAddress] string Email,
        [Required] [MinLength(6)] string Password) : IRequest<AuthResult>;

    public class RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuthService authService,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUserCommandHandler> logger)
        : IRequestHandler<RegisterUserCommand, AuthResult>
    {
        public async Task<AuthResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = authService.HashPassword(request.Password)
            };

            var (usernameExists, emailExists) = await userRepository.ExistsByUsernameOrEmailAsync(user.Username, user.Email);
            if (usernameExists) throw new ConflictException("Username is already taken.");
            if (emailExists) throw new ConflictException("Email is already registered.");

            var (authData, refreshToken, plainRefreshToken) = AuthTokenHelper.CreateTokens(user, tokenService, authService);

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

            logger.LogInformation("User {UserId} registered as {Username}", user.Id, user.Username);
            return new AuthResult(authData, plainRefreshToken, refreshToken.ExpiresAt);
        }
    }
}
