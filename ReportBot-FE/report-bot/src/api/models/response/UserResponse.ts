interface UserResponse {
    id: number;
    chatId: number;
    worksnapsId: number;
    login: string;
    firstName: string;
    lastName: string;
    shiftTime: number;
    email: string;
    role: string;
};

export default UserResponse;