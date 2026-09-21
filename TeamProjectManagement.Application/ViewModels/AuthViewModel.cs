namespace TeamProjectManagement.Application.ViewModels
{
    public class AuthViewModel
    {
        public string AccessToken { get; set; } = null!;
        public UserViewModel User { get; set; } = null!;
    }
}
