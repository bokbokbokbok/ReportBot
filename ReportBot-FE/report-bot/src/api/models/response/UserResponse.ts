interface UserResponse {
    id: number;
    chatId: number;
    worksnapsId: number;
    username: string;
    firstName: string;
    lastName: string;
    shiftTime: number;
    role: string;
};

export default UserResponse;