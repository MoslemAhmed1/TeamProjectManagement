using MediatR;
using System.ComponentModel.DataAnnotations;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;

namespace TeamProjectManagement.Application.Features.Users.Commands
{
    public record UpdateProfileCommand(
        Guid CallerId,
        [Required] [StringLength(50, MinimumLength = 3)] string Username,
        [Required] [EmailAddress] string Email,
        [MinLength(6)] string? Password) : IRequest;

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

            var originalUsername = user.Username;
            var originalEmail = user.Email;

            user.Username = request.Username;
            user.Email = request.Email;

            if (user.Username != originalUsername || user.Email != originalEmail)
            {
                var (usernameExists, emailExists) = await userRepository.ExistsByUsernameOrEmailAsync(user.Username, user.Email);
                if (user.Username != originalUsername && usernameExists)
                    throw new ConflictException("Username is already taken.");
                if (user.Email != originalEmail && emailExists)
                    throw new ConflictException("Email is already registered.");
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.PasswordHash = authService.HashPassword(request.Password);
            }

            await unitOfWork.SaveChangesAsync();
        }
    }
}
