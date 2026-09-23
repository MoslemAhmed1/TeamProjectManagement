using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamProjectManagement.Api.Common;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Features.Projects.Commands;
using TeamProjectManagement.Application.Features.Projects.Queries;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedViewModel<ProjectViewModel>>>> GetProjects([FromQuery] ProjectQueryParameters queryParameters)
        {
            var query = new GetMyProjectsQuery(User.GetUserId(), queryParameters);
            var result = await sender.Send(query);
            return Ok(ApiResponse<PagedViewModel<ProjectViewModel>>.Ok(result, "Projects retrieved successfully."));
        }

        [HttpGet("owned")]
        public async Task<ActionResult<ApiResponse<PagedViewModel<ProjectViewModel>>>> GetOwnedProjects([FromQuery] ProjectQueryParameters queryParameters)
        {
            var query = new GetMyOwnedProjectsQuery(User.GetUserId(), queryParameters);
            var result = await sender.Send(query);
            return Ok(ApiResponse<PagedViewModel<ProjectViewModel>>.Ok(result, "Owned projects retrieved successfully."));
        }

        [HttpGet("member")]
        public async Task<ActionResult<ApiResponse<PagedViewModel<ProjectViewModel>>>> GetMemberProjects([FromQuery] ProjectQueryParameters queryParameters)
        {
            var query = new GetMyMemberProjectsQuery(User.GetUserId(), queryParameters);
            var result = await sender.Send(query);
            return Ok(ApiResponse<PagedViewModel<ProjectViewModel>>.Ok(result, "Member projects retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProjectViewModel>>> CreateProject([FromBody] CreateProjectCommand command)
        {
            var commandWithUser = command with { CallerId = User.GetUserId() };
            var result = await sender.Send(commandWithUser);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<ProjectViewModel>.Created(result, "Project created successfully."));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ApiResponse<ProjectDetailsViewModel>>> GetProjectDetails(Guid id)
        {
            var query = new GetProjectQuery(id, User.GetUserId());
            var result = await sender.Send(query);
            return Ok(ApiResponse<ProjectDetailsViewModel>.Ok(result, "Project details retrieved successfully."));
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateProject(Guid id, [FromBody] UpdateProjectCommand command)
        {
            var commandWithUser = command with { ProjectId = id, CallerId = User.GetUserId() };
            await sender.Send(commandWithUser);
            return Ok(ApiResponse<object>.Ok(null!, "Project updated successfully."));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteProject(Guid id)
        {
            var command = new DeleteProjectCommand(id, User.GetUserId());
            await sender.Send(command);
            return Ok(ApiResponse<object>.Ok(null!, "Project deleted successfully."));
        }

        [HttpGet("{id:guid}/progress")]
        public async Task<ActionResult<ApiResponse<ProgressViewModel>>> GetProjectProgress(Guid id)
        {
            var query = new GetProjectProgressQuery(id, User.GetUserId());
            var result = await sender.Send(query);
            return Ok(ApiResponse<ProgressViewModel>.Ok(result, "Project progress retrieved successfully."));
        }
    }
}
