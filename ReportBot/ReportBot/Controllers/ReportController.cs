using Microsoft.AspNetCore.Mvc;
using ReportBot.Common.Requests;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetReports([FromQuery] FilterRequest filterRequest, [FromQuery] PaginationRequest paginationRequest)
        {
            var reports = await _reportService.GetReportsAsync(filterRequest, paginationRequest);

            return Ok(reports);
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetReportsForProject(int projectId, [FromQuery] PaginationRequest request)
        {
            var reports = await _reportService.GetReportsForProjectAsync(projectId, request);

            return Ok(reports);
        }
    }
}
