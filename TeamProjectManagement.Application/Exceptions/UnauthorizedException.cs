namespace TeamProjectManagement.Application.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Invalid credentials.") : base(message) { }
    }
}
