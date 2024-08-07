using ReportBot.Common.DTOs;
using ReportBot.Common.Requests;
using ReportBot.Common.Responses;

namespace ReportBot.Services.Services.Interfaces
{
    public interface IReportService
    {
        Task<ReportDTO> AddReportAsync(CreateReportDTO report);
        Task<List<ReportDTO>> GetReportsAsync(FilterRequest filterRequest);
        Task<PageList<ReportDTO>> GetReportsForProjectAsync(int projectId, FilterRequest filterRequest, PaginationRequest paginationRequest);
        Task<List<ReportDTO>> GetReportsForUserAsync(long chatId);
        Task<Dictionary<string, ReportStatisticsResponse>> GetReportsStatisticsAsync();
        Task<SessionStatisticsResponse> GetSessionsStatiticsAsync();
    }
}
