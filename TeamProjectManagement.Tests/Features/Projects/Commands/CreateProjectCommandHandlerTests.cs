using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TeamProjectManagement.Application.Features.Projects.Commands;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Tests.Features.Projects.Commands;

public class CreateProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_CreatesProjectAndOwnerMembership()
    {
        var projects = new Mock<IProjectRepository>();
        var members = new Mock<IProjectMemberRepository>();
        var uow = new Mock<IUnitOfWork>();
        var handler = new CreateProjectCommandHandler(projects.Object, members.Object, uow.Object, NullLogger<CreateProjectCommandHandler>.Instance);
        var ownerId = Guid.NewGuid();

        var result = await handler.Handle(
            new CreateProjectCommand("Sprint Board", "Current sprint", ownerId),
            CancellationToken.None);

        Assert.Equal("Sprint Board", result.Name);
        Assert.Equal(ownerId, result.OwnerId);
        projects.Verify(r => r.AddProjectAsync(It.IsAny<Project>()), Times.Once);
        members.Verify(r => r.AddMemberAsync(It.IsAny<ProjectMember>()), Times.Once);
        uow.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
}
