import ProjectResponse from "./ProjectResponse";
import UserResponse from "./UserResponse";

interface ReportResponse {
    id: number;
    created: Date;
    message: string;
    userName: string;
    dateOfShift: Date;
    timeOfShift: number;
    chatId: number;
    user: UserResponse;
    project: ProjectResponse;
};

export default ReportResponse;