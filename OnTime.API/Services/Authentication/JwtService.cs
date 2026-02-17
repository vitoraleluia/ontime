using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using OnTime.API.Constants;
using OnTime.API.Models.Domain;
using OnTime.API.Models.Settings;

namespace OnTime.API.Services.Authentication;

public class JwtService(IOptions<JwtSettings> jwtSettings) : IJwtService
{
    private readonly JwtSettings jwtSettings = jwtSettings.Value;

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(ClaimNames.IsProfessional, user.IsProfessional.ToString().ToLower()),
        };

        var token = new JwtSecurityToken(
            issuer: this.jwtSettings.Issuer,
            audience: this.jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(this.jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}