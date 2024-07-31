using ReportBot.Common.DTOs;
using ReportBot.Common.Requests;
using ReportBot.Common.Responses;

namespace ReportBot.Services.Services.Interfaces
{
    public interface IReportService
    {
        Task<ReportDTO> AddReportAsync(CreateReportDTO report);
        Task<PageList<ReportDTO>> GetReportsAsync(FilterRequest filterRequest, PaginationRequest paginationRequest);
        Task<PageList<ReportDTO>> GetReportsForProjectAsync(int projectId, PaginationRequest request);
        Task<List<ReportDTO>> GetReportsForUserAsync(long chatId);
    }
}
