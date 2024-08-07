import ApiResponse from "./models/response/ApiResponse";
import Api from "./repository/Api";
import UserResponse from "./models/response/UserResponse";

const User = {
    getAll: async (sorting: number): Promise<ApiResponse<UserResponse[]>> => {
        const response = await Api.get<UserResponse[]>(`/user?sorting=${sorting}`);

        return response;
    }
};

export default User;