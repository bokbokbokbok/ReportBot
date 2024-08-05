interface ApiResponse<T> {
    success: boolean;
    data?: T;
    statusCode?: number;
    error?: any;
}

export default ApiResponse;