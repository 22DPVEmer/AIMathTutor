using MathTutor.Core.Entities;

namespace MathTutor.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateJwtToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    (string userId, IList<string> roles) ValidateJwtToken(string token);
} 