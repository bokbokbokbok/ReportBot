import ApiResponse from "./models/response/ApiResponse";
import Api from "./repository/Api";
import UserResponse from "./models/response/UserResponse";

const User = {
    getAll: async (): Promise<ApiResponse<UserResponse[]>> => {
        const response = await Api.get<UserResponse[]>("/users");

        return response;
    }
};

export default User;