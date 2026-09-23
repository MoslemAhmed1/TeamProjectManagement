using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamProjectManagement.Api.Common;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Features.Tasks.Commands;
using TeamProjectManagement.Application.Features.Tasks.Queries;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/projects/{projectId:guid}/tasks")]
    public class TasksController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedViewModel<TaskViewModel>>>> GetProjectTasks(Guid projectId, [FromQuery] TaskQueryParameters queryParameters)
        {
            var query = new GetProjectTasksQuery(projectId, User.GetUserId(), queryParameters);
            var result = await sender.Send(query);
            return Ok(ApiResponse<PagedViewModel<TaskViewModel>>.Ok(result, "Tasks retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TaskViewModel>>> CreateTask(Guid projectId, [FromBody] CreateTaskCommand command)
        {
            var commandWithIds = command with { ProjectId = projectId, CallerId = User.GetUserId() };
            var result = await sender.Send(commandWithIds);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<TaskViewModel>.Created(result, "Task created successfully."));
        }

        [HttpGet("{taskId:guid}")]
        public async Task<ActionResult<ApiResponse<TaskViewModel>>> GetTaskById(Guid projectId, Guid taskId)
        {
            var query = new GetTaskQuery(taskId, User.GetUserId());
            var result = await sender.Send(query);
            return Ok(ApiResponse<TaskViewModel>.Ok(result, "Task retrieved successfully."));
        }

        [HttpPut("{taskId:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateTask(Guid projectId, Guid taskId, [FromBody] UpdateTaskCommand command)
        {
            var commandWithIds = command with { TaskId = taskId, CallerId = User.GetUserId() };
            await sender.Send(commandWithIds);
            return Ok(ApiResponse<object>.Ok(null!, "Task updated successfully."));
        }

        [HttpDelete("{taskId:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTask(Guid projectId, Guid taskId)
        {
            var command = new DeleteTaskCommand(taskId, User.GetUserId());
            await sender.Send(command);
            return Ok(ApiResponse<object>.Ok(null!, "Task deleted successfully."));
        }

        [HttpPatch("{taskId:guid}/status")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateTaskStatus(Guid projectId, Guid taskId, [FromBody] UpdateTaskStatusCommand command)
        {
            var commandWithIds = command with { TaskId = taskId, CallerId = User.GetUserId() };
            await sender.Send(commandWithIds);
            return Ok(ApiResponse<object>.Ok(null!, "Task status updated successfully."));
        }

        [HttpPost("{taskId:guid}/assign")]
        public async Task<ActionResult<ApiResponse<object>>> AssignTask(Guid projectId, Guid taskId, [FromBody] AssignTaskCommand command)
        {
            var commandWithIds = command with { TaskId = taskId, CallerId = User.GetUserId() };
            await sender.Send(commandWithIds);
            return Ok(ApiResponse<object>.Ok(null!, "Task assigned successfully."));
        }

        [HttpPost("{taskId:guid}/unassign")]
        public async Task<ActionResult<ApiResponse<object>>> UnassignTask(Guid projectId, Guid taskId)
        {
            var command = new UnassignTaskCommand(taskId, User.GetUserId());
            await sender.Send(command);
            return Ok(ApiResponse<object>.Ok(null!, "Task unassigned successfully."));
        }
    }
}
