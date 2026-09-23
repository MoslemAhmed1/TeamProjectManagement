using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Features.Tasks.Commands;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Entities;

namespace TeamProjectManagement.Tests.Features.Tasks.Commands;

public class DeleteTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_Owner_DeletesTask()
    {
        var ownerId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = new ProjectTask { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Old task" };
        var project = new Project { Id = projectId, Name = "Alpha", OwnerId = ownerId };

        var projects = new Mock<IProjectRepository>();
        var tasks = new Mock<IProjectTaskRepository>();
        var uow = new Mock<IUnitOfWork>();
        var handler = new DeleteTaskCommandHandler(projects.Object, tasks.Object, uow.Object, NullLogger<DeleteTaskCommandHandler>.Instance);

        tasks.Setup(r => r.GetTaskByIdAsync(task.Id)).ReturnsAsync(task);
        projects.Setup(r => r.GetProjectByIdAsync(projectId)).ReturnsAsync(project);

        await handler.Handle(new DeleteTaskCommand(task.Id, ownerId), CancellationToken.None);

        tasks.Verify(r => r.DeleteProjectTask(task), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Member_ThrowsForbidden()
    {
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = new ProjectTask { Id = Guid.NewGuid(), ProjectId = projectId, Title = "Old task" };
        var project = new Project { Id = projectId, Name = "Alpha", OwnerId = ownerId };

        var projects = new Mock<IProjectRepository>();
        var tasks = new Mock<IProjectTaskRepository>();
        var uow = new Mock<IUnitOfWork>();
        var handler = new DeleteTaskCommandHandler(projects.Object, tasks.Object, uow.Object, NullLogger<DeleteTaskCommandHandler>.Instance);

        tasks.Setup(r => r.GetTaskByIdAsync(task.Id)).ReturnsAsync(task);
        projects.Setup(r => r.GetProjectByIdAsync(projectId)).ReturnsAsync(project);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(new DeleteTaskCommand(task.Id, memberId), CancellationToken.None));
    }
}
