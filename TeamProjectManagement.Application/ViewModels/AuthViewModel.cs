namespace TeamProjectManagement.Application.ViewModels
{
    public record AuthViewModel
    {
        public string AccessToken { get; set; } = null!;
        public UserViewModel User { get; set; } = null!;
    }
}
