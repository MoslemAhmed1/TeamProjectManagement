using MediatR;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record UpdateProfileCommand(Guid CallerId, string Username, string Email, string? Password) : IRequest;

    public class UpdateProfileCommandHandler(
        IUserRepository userRepository,
        IAuthService authService,
        IUnitOfWork unitOfWork)
        : IRequestHandler<UpdateProfileCommand>
    {
        public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserByIdAsync(request.CallerId);
            if (user is null) throw new NotFoundException("User not found.");

            var username = request.Username.Trim();
            var email = request.Email.Trim().ToLowerInvariant();

            var (usernameExists, emailExists) = await userRepository.ExistsByUsernameOrEmailAsync(username, email);
            if (usernameExists && user.Username.ToLowerInvariant() != username.ToLowerInvariant()) 
                throw new ConflictException("Username is already taken.");
            if (emailExists && user.Email.ToLowerInvariant() != email.ToLowerInvariant()) 
                throw new ConflictException("Email is already registered.");

            user.Username = username;
            user.Email = email;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = authService.HashPassword(request.Password);
            }

            await unitOfWork.SaveChangesAsync();
        }
    }
}
