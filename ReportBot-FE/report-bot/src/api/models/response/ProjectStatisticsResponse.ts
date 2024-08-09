import ProjectResponse from "./ProjectResponse";

interface ProjectStatisticsResponse {
    project: ProjectResponse;
    totalMinutesPerWeek: number;
    totalMinutesPerDay: number;
}

export default ProjectStatisticsResponse;