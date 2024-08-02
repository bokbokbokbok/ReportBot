using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportBot.Common.Extensions;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userId = HttpContext.GetUserId();
            var result = await _projectService.GetProjectsAsync(userId);

            return Ok(result);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetProjectById(int projectId)
        {
            var result = await _projectService.GetProjectByIdAsync(projectId);

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetProjectStatistics()
        {
            var userId = HttpContext.GetUserId();
            var result = await _projectService.GetProjectStatistics(userId);

            return Ok(result);
        }
    }
}
