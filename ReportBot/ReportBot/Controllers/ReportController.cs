using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportBot.Common.Requests;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReports([FromQuery] FilterRequest filterRequest)
        {
            var result = await _reportService.GetReportsAsync(filterRequest);

            return Ok(result);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetReportsForProject(int projectId, [FromQuery] FilterRequest filterRequest)
        {
            var result = await _reportService.GetReportsForProjectAsync(projectId, filterRequest);

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetSessionsStatitics()
        {
            var result = await _reportService.GetSessionsStatiticsAsync();

            return Ok(result);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReportsStatistics()
        {
            var result = await _reportService.GetReportsStatisticsAsync();

            return Ok(result);
        }
    }
}
