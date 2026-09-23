using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamProjectManagement.Api.Common;
using TeamProjectManagement.Application.Common;
using TeamProjectManagement.Application.Features.Tasks.Queries;
using TeamProjectManagement.Application.ViewModels;

namespace TeamProjectManagement.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class MyTasksController(ISender sender) : ControllerBase
    {
        [HttpGet("my")]
        public async Task<ActionResult<ApiResponse<PagedViewModel<TaskViewModel>>>> GetMyTasks([FromQuery] TaskQueryParameters queryParameters)
        {
            var query = new GetMyTasksQuery(User.GetUserId(), queryParameters);
            var result = await sender.Send(query);
            return Ok(ApiResponse<PagedViewModel<TaskViewModel>>.Ok(result, "Your tasks retrieved successfully."));
        }
    }
}
