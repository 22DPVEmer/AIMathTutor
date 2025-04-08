using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MathTutor.Application.Interfaces;
using MathTutor.Core.Entities;
using MathTutor.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MathTutor.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("firstName", user.FirstName),
            new Claim("lastName", user.LastName)
        };

        // Add roles to claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_jwtSettings.ExpirationInDays);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public (string userId, IList<string> roles) ValidateJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return (string.Empty, new List<string>());

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var roles = jwtToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();

            return (userId, roles);
        }
        catch
        {
            return (string.Empty, new List<string>());
        }
    }
} 