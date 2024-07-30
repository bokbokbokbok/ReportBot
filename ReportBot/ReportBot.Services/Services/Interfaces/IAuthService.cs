
using ReportBot.Common.Requests;
using ReportBot.Common.Responses;

namespace ReportBot.Services.Services.Interfaces;

public interface IAuthService
{
    Task<AuthSuccessResponse> SignIn(SignInRequest request);
}
