using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        /*
         
            Functionalities Required: 
            1- Get User by Id
            2- Get User by Username/Email
            3- Exists by Username/Email 
            4- Add User
            5- Update User Profile (Username, Email, Password)

        */

        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User?> FindByIdentifierAsync(string identifier);
        Task<(bool usernameExists, bool emailExists)> ExistsByUsernameOrEmailAsync(string username, string email);
        Task AddUserAsync(User user);


    }
}
