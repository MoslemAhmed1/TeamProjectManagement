using TeamProjectManagement.Application.ViewModels;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Mappings
{
    public static partial class MappingExtensions
    {
        public static MemberViewModel ToViewModel(this ProjectMember member)
        {
            return new MemberViewModel
            {
                UserId = member.UserId,
                Username = member.User?.Username ?? "",
                Email = member.User?.Email ?? "",
                Role = member.Role.ToString()
            };
        }
    }
}
