using MediatR;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record LogoutCommand(string RefreshToken) : IRequest;

    public class LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IAuthService authService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<LogoutCommand>
    {
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var hashedRequestToken = authService.HashToken(request.RefreshToken);
            var storedToken = await refreshTokenRepository.GetRefreshTokenAsync(hashedRequestToken);

            if (storedToken is null || storedToken.IsRevoked)
                return;

            storedToken.IsRevoked = true;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
