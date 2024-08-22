using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportBot.Common.Extensions;
using ReportBot.Services.Services.Interfaces;

namespace ReportBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(string? projectName)
        {
            var managerId = HttpContext.GetUserId();
            var result = await _userService.GetUsersAsync(managerId, projectName);

            return Ok(result);
        }
    }
}
