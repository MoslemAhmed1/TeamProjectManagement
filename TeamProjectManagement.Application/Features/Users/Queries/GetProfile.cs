using MediatR;
using System.ComponentModel.DataAnnotations;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Application.Mappings;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Application.Features.Users.Queries
{
    public record GetProfileQuery(Guid CallerId) : IRequest<UserViewModel>;

    public class GetProfileQueryHandler(IUserRepository userRepository) : IRequestHandler<GetProfileQuery, UserViewModel>
    {
        public async Task<UserViewModel> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetUserProfile(request.CallerId);
            if (user is null) throw new NotFoundException("User not found.");

            return user.ToViewModel();
        }
    }
}
