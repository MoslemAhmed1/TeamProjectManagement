namespace TeamProjectManagement.Domain
{
    public static class Text
    {
        public static string Required(string value) => value.Trim();

        public static string? Optional(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        public static string Identity(string value) => value.Trim().ToLowerInvariant();
    }
}
