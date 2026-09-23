using Microsoft.AspNetCore.Http;
using System.Text.Json;
using TeamProjectManagement.Api.Common;
using TeamProjectManagement.Application.Exceptions;

namespace TeamProjectManagement.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, apiResponse) = exception switch
            {
                NotFoundException e => (404, ApiResponse<object>.NotFound(e.Message)),
                ForbiddenException e => (403, ApiResponse<object>.Forbidden(e.Message)),
                ConflictException e => (409, ApiResponse<object>.Conflict(e.Message)),
                BadRequestException e => (400, ApiResponse<object>.BadRequest(e.Message)),
                UnauthorizedException e => (401, ApiResponse<object>.Unauthorized(e.Message)),
                ValidationException e => (422, new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "Validation failed.", 
                    Errors = e.Errors, 
                    StatusCode = 422 
                }),
                _ => (500, new ApiResponse<object> 
                { 
                    Success = false, 
                    Message = "An internal server error occurred.", 
                    StatusCode = 500 
                }) // Note: For prod we shouldn't expose exception.Message directly, but fine for now
            };

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsJsonAsync(apiResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
