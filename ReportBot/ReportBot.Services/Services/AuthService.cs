using McgTgBotNet.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReportBot.Common.Configs;
using ReportBot.Common.Exceptions;
using ReportBot.Common.Requests;
using ReportBot.Common.Responses;
using ReportBot.DataBase.Repositories.Interfaces;
using ReportBot.Services.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReportBot.Services.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IWorksnapsService _worksnapsService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IRepository<User> userRepository,
        IWorksnapsService worksnapsService,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _worksnapsService = worksnapsService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthSuccessResponse> SignIn(SignInRequest request)
    {
        var userId = await _worksnapsService.GetUserId(request.Email);

        var user = await _userRepository.FirstOrDefaultAsync(x => x.WorksnapsId == userId)
            ?? throw new NotFoundException($"User with such id not found. Id: {userId}");

        if (user.Role != "manager")
            throw new UnauthorizedException("You are not allowed to use admin dashboard");

        var result = new AuthSuccessResponse(GenerateAccessToken(user));

        return result;
    }

    private string GenerateAccessToken(User user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim("role", user.Role!),
            new Claim("username", user.Username!),
            new Claim("worksnapsId", user.WorksnapsId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(_jwtSettings.AccessTokenLifeTime),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);
        return jwtToken;
    }
}
