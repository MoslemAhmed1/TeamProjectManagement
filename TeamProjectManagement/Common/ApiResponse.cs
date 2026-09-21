namespace TeamProjectManagement.Api.Common
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
        public int StatusCode { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null)
            => new() { Success = true, Data = data, Message = message, StatusCode = 200 };

        public static ApiResponse<T> Created(T data, string? message = null)
            => new() { Success = true, Data = data, Message = message, StatusCode = 201 };

        public static ApiResponse<T> NoContent()
            => new() { Success = true, StatusCode = 204 };

        public static ApiResponse<T> Fail(string error, int statusCode = 400)
            => new() { Success = false, Errors = new List<string> { error }, StatusCode = statusCode };

        public static ApiResponse<T> NotFound(string error = "Resource not found.")
            => Fail(error, 404);

        public static ApiResponse<T> Forbidden(string error = "Access denied.")
            => Fail(error, 403);

        public static ApiResponse<T> Conflict(string error)
            => Fail(error, 409);

        public static ApiResponse<T> BadRequest(string error)
            => Fail(error, 400);

        public static ApiResponse<T> Unauthorized(string error = "Unauthorized.")
            => Fail(error, 401);
    }
}
