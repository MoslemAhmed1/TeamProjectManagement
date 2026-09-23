using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Features.Users.Commands;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Application.Interfaces.Services;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Tests.Features.Users.Commands;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokens = new();
    private readonly Mock<IAuthService> _auth = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _handler = new RegisterUserCommandHandler(
            _users.Object,
            _refreshTokens.Object,
            _auth.Object,
            _tokens.Object,
            _uow.Object,
            NullLogger<RegisterUserCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_NewUser_CreatesAccount()
    {
        var command = new RegisterUserCommand("newuser", "new@example.com", "Test123!");

        _users.Setup(r => r.ExistsByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((false, false));
        _auth.Setup(s => s.HashPassword(command.Password)).Returns("hashed-password");
        _tokens.Setup(s => s.GenerateAccessToken(It.IsAny<User>())).Returns("access-token");
        _tokens.Setup(s => s.GenerateRefreshToken()).Returns("refresh-token");
        _tokens.Setup(s => s.GetRefreshTokenExpiry()).Returns(DateTime.UtcNow.AddDays(7));
        _auth.Setup(s => s.HashToken("refresh-token")).Returns("hashed-refresh");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("access-token", result.Data.AccessToken);
        Assert.Equal("newuser", result.Data.User.Username);
        _users.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Once);
        _uow.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateUsername_ThrowsConflict()
    {
        var command = new RegisterUserCommand("testuser1", "other@example.com", "Test123!");

        _users.Setup(r => r.ExistsByUsernameOrEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((true, false));
        _auth.Setup(s => s.HashPassword(command.Password)).Returns("hashed-password");

        await Assert.ThrowsAsync<ConflictException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
