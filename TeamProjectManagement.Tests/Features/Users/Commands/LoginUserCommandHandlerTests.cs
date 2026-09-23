using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Features.Users.Commands;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Tests.Features.Users.Commands;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();
    private readonly Mock<IAuthService> _auth = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _handler = new LoginUserCommandHandler(
            _users.Object,
            _refreshTokens.Object,
            _auth.Object,
            _tokens.Object,
            _uow.Object,
            NullLogger<LoginUserCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsAccessToken()
    {
        var command = new LoginUserCommand("testuser1", "Test123!");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser1",
            Email = "testuser1@example.com",
            PasswordHash = "hashed"
        };

        _users.Setup(r => r.FindByIdentifierAsync(command.Identifier)).ReturnsAsync(user);
        _auth.Setup(s => s.VerifyPassword(command.Password, user.PasswordHash)).Returns(true);
        _tokens.Setup(s => s.GenerateAccessToken(user)).Returns("access-token");
        _tokens.Setup(s => s.GenerateRefreshToken()).Returns("refresh-token");
        _tokens.Setup(s => s.GetRefreshTokenExpiry()).Returns(DateTime.UtcNow.AddDays(7));
        _auth.Setup(s => s.HashToken("refresh-token")).Returns("hashed-refresh");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("access-token", result.Data.AccessToken);
        Assert.Equal("refresh-token", result.RefreshToken);
        _refreshTokens.Verify(r => r.AddRefreshTokenAsync(It.IsAny<RefreshToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        var command = new LoginUserCommand("testuser1", "wrong");
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser1",
            Email = "testuser1@example.com",
            PasswordHash = "hashed"
        };

        _users.Setup(r => r.FindByIdentifierAsync(command.Identifier)).ReturnsAsync(user);
        _auth.Setup(s => s.VerifyPassword(command.Password, user.PasswordHash)).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
