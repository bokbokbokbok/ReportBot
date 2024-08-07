import UserResponse from "./UserResponse";

interface ProjectResponse {
    id: number;
    worksnapsId: number;
    name: string;
    description: string;
    status: string;
    users: UserResponse[];
};

export default ProjectResponse;