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
    [Route("api/projects/{projectId:guid}/members")]
    public class MembersController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PagedViewModel<MemberViewModel>>>> GetProjectMembers(Guid projectId, [FromQuery] QueryParameters queryParameters)
        {
            var query = new GetProjectMembersQuery(projectId, User.GetUserId(), queryParameters);
            var result = await sender.Send(query);
            return Ok(ApiResponse<PagedViewModel<MemberViewModel>>.Ok(result, "Project members retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MemberViewModel>>> AddMember(Guid projectId, [FromBody] AddProjectMemberCommand command)
        {
            var commandWithIds = command with { ProjectId = projectId, CallerId = User.GetUserId() };
            var result = await sender.Send(commandWithIds);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<MemberViewModel>.Created(result, "Member added successfully."));
        }

        [HttpDelete("{userId:guid}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveMember(Guid projectId, Guid userId)
        {
            var command = new RemoveProjectMemberCommand(projectId, userId, User.GetUserId());
            await sender.Send(command);
            return Ok(ApiResponse<object>.Ok(null!, "Member removed successfully."));
        }
    }
}
