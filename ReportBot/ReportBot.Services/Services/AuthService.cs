using McgTgBotNet.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ReportBot.Common.Configs;
using ReportBot.Common.Exceptions;
using ReportBot.Common.Requests;
using ReportBot.Common.Responses;
using ReportBot.Services.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReportBot.Services.Services;

public class AuthService : IAuthService
{
    private readonly IWorksnapsService _worksnapsService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IWorksnapsService worksnapsService,
        IOptions<JwtSettings> jwtSettings)
    {
        _worksnapsService = worksnapsService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthSuccessResponse> SignIn(SignInRequest request)
    {
        var worksnapsUser = await _worksnapsService.GetUserAsync(request.Email);

        var userRole = await _worksnapsService.GetUserRoleAsync(worksnapsUser.Id);

        if (userRole.ToLower() != "manager")
            throw new ForbiddenException("You are not allowed to use admin dashboard");

        var result = new AuthSuccessResponse(GenerateAccessToken(worksnapsUser));

        return result;
    }

    private string GenerateAccessToken(WorksnapsUserDTO user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var claims = new List<Claim>
        {
            new Claim("worksnapsId", user.Id.ToString()),
            new Claim("username", user.Login),
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
