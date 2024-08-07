import AuthRequest from "./models/request/AuthRequest";
import AuthSuccessResponse from "./models/response/AuthSuccessResponse";
import Api from "./repository/Api";

const Auth = {
    signIn: async (request: AuthRequest): Promise<any> => {
        const response = await Api.post<AuthRequest, AuthSuccessResponse>("/auth/sign-in", request);
        if (response.success) {
            const tokens = response.data as AuthSuccessResponse;
            localStorage.setItem('accessToken', tokens.accessToken ?? '');
        }

        return response;
    }
};

export default Auth;