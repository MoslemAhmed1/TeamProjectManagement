using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record LogoutCommand(string RefreshToken) : IRequest;

    public class LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
        : IRequestHandler<LogoutCommand>
    {
        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var storedToken = await refreshTokenRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (storedToken is null || storedToken.IsRevoked)
                return;

            storedToken.IsRevoked = true;
            await unitOfWork.SaveChangesAsync();
        }
    }
}
