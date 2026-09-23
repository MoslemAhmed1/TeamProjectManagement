using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using TeamProjectManagement.Application.Exceptions;
using TeamProjectManagement.Application.Features.Tasks.Commands;
using TeamProjectManagement.Application.Interfaces.Repositories;
using TeamProjectManagement.Domain.Entities;
using TeamProjectManagement.Domain.Enums;

namespace TeamProjectManagement.Tests.Features.Tasks.Commands;

public class UpdateTaskStatusCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projects = new();
    private readonly Mock<IProjectMemberRepository> _members = new();
    private readonly Mock<IProjectTaskRepository> _tasks = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly UpdateTaskStatusCommandHandler _handler;

    public UpdateTaskStatusCommandHandlerTests()
    {
        _handler = new UpdateTaskStatusCommandHandler(
            _projects.Object,
            _members.Object,
            _tasks.Object,
            _uow.Object,
            NullLogger<UpdateTaskStatusCommandHandler>.Instance);
    }

    [Fact]
    public async Task Handle_Assignee_CanUpdateStatus()
    {
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = "Write tests",
            AssignedToId = memberId,
            Status = ProjectTaskStatus.ToDo
        };
        var project = new Project { Id = projectId, Name = "Alpha", OwnerId = ownerId };

        _tasks.Setup(r => r.GetTaskByIdAsync(task.Id)).ReturnsAsync(task);
        _projects.Setup(r => r.GetProjectByIdAsync(projectId)).ReturnsAsync(project);
        _members.Setup(r => r.IsMemberAsync(projectId, memberId)).ReturnsAsync(true);

        await _handler.Handle(new UpdateTaskStatusCommand(task.Id, ProjectTaskStatus.Done, memberId), CancellationToken.None);

        Assert.Equal(ProjectTaskStatus.Done, task.Status);
        _uow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_MemberNotAssignee_ThrowsForbidden()
    {
        var ownerId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var otherMemberId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            Title = "Write tests",
            AssignedToId = memberId,
            Status = ProjectTaskStatus.ToDo
        };
        var project = new Project { Id = projectId, Name = "Alpha", OwnerId = ownerId };

        _tasks.Setup(r => r.GetTaskByIdAsync(task.Id)).ReturnsAsync(task);
        _projects.Setup(r => r.GetProjectByIdAsync(projectId)).ReturnsAsync(project);
        _members.Setup(r => r.IsMemberAsync(projectId, otherMemberId)).ReturnsAsync(true);

        await Assert.ThrowsAsync<ForbiddenException>(() =>
            _handler.Handle(new UpdateTaskStatusCommand(task.Id, ProjectTaskStatus.Done, otherMemberId), CancellationToken.None));
    }
}
