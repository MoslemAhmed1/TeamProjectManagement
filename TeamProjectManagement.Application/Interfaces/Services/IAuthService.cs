namespace TeamProjectManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        string HashPassword(string plainTextPassword);
        bool VerifyPassword(string plainTextPassword, string hashedPassword);
    }
}
