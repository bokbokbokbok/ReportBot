using Microsoft.AspNetCore.Mvc;
using ReportBot.Common.Requests;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var token = await _authService.SignIn(request);

        return Ok(token);
    }
}
