using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Mappings
{
    public static partial class MappingExtensions
    {
        public static UserViewModel ToViewModel(this User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
