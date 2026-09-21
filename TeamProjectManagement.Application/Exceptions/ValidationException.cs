namespace TeamProjectManagement.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(List<string> errors) 
            : base(errors.FirstOrDefault() ?? "One or more validation failures have occurred.")
        {
            Errors = errors;
        }

        public ValidationException(string error) 
            : base(error)
        {
            Errors = new List<string> { error };
        }
    }
}
