using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TeamProjectManagement.Api.Common;
using TeamProjectManagement.Application.Features.Users.Commands;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ISender sender) : ControllerBase
    {
        private void SetRefreshTokenCookie(string token, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        [HttpPost("register")]
        [EnableRateLimiting("Strict")]
        public async Task<ActionResult<ApiResponse<AuthViewModel>>> Register([FromBody] RegisterUserCommand command)
        {
            var result = await sender.Send(command);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<AuthViewModel>.Created(result.Data, "User registered successfully."));
        }

        [HttpPost("login")]
        [EnableRateLimiting("Strict")]
        public async Task<ActionResult<ApiResponse<AuthViewModel>>> Login([FromBody] LoginUserCommand command)
        {
            var result = await sender.Send(command);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(ApiResponse<AuthViewModel>.Ok(result.Data, "Login successful."));
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<AuthViewModel>>> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(ApiResponse<AuthViewModel>.Unauthorized("Refresh token is missing."));

            var command = new RefreshTokenCommand(refreshToken);
            var result = await sender.Send(command);
            SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(ApiResponse<AuthViewModel>.Ok(result.Data, "Token refreshed successfully."));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var command = new LogoutCommand(refreshToken);
                await sender.Send(command);
                Response.Cookies.Delete("refreshToken");
            }

            return Ok(ApiResponse<object>.Ok(null!, "Logout successful."));
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProfile([FromBody] UpdateProfileCommand command)
        {
            var commandWithUser = command with { CallerId = User.GetUserId() };
            await sender.Send(commandWithUser);
            return Ok(ApiResponse<object>.Ok(null!, "Profile updated successfully."));
        }
    }
}
