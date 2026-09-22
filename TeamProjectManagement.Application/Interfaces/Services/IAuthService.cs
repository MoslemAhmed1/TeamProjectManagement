namespace TeamProjectManagement.Application.Interfaces.Services
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
        string HashToken(string token);
    }
}
