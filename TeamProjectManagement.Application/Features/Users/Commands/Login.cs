using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record LoginUserCommand(
        [Required] string Identifier,
        [Required] string Password) : IRequest<AuthResult>;

    public class LoginUserCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IAuthService authService,
        ITokenService tokenService,
        IUnitOfWork unitOfWork,
        ILogger<LoginUserCommandHandler> logger)
        : IRequestHandler<LoginUserCommand, AuthResult>
    {
        public async Task<AuthResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.FindByIdentifierAsync(request.Identifier);
            if (user is null || !authService.VerifyPassword(request.Password, user.PasswordHash))
            {
                logger.LogWarning("Failed login attempt for {Identifier}", request.Identifier);
                throw new UnauthorizedException("Invalid credentials.");
            }

            var (authData, refreshToken, plainRefreshToken) = AuthTokenHelper.CreateTokens(user, tokenService, authService);

            await refreshTokenRepository.AddRefreshTokenAsync(refreshToken);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("User {UserId} logged in", user.Id);
            return new AuthResult(authData, plainRefreshToken, refreshToken.ExpiresAt);
        }
    }
}
