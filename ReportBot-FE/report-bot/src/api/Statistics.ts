import ApiResponse from "./models/response/ApiResponse";
import ProjectStatisticsResponse from "./models/response/ProjectStatisticsResponse";
import ReportStatisticsResponse from "./models/response/ReportStatisticsResponse";
import SessionStatisticsResponse from "./models/response/SessionStatisticsResponse";
import Api from "./repository/Api";

const Statistics = {
    projectStatistics: async (): Promise<ApiResponse<ProjectStatisticsResponse[]>> => {
        const response = await Api.get<ProjectStatisticsResponse[]>(`/project/get-project-statistics`);

        return response;
    },

    sessionStatistics: async (): Promise<ApiResponse<SessionStatisticsResponse>> => {
        const response = await Api.get<SessionStatisticsResponse>(`/report/get-sessions-statitics`);

        return response;
    },

    reportStatistics: async (): Promise<ApiResponse<Record<string, ReportStatisticsResponse>>> => {
        const response = await Api.get<Record<string, ReportStatisticsResponse>>(`/report/get-reports-statistics`);
        
        return response;
    }
};

export default Statistics;