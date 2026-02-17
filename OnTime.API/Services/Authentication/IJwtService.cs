using OnTime.API.Models.Domain;

namespace OnTime.API.Services.Authentication;

public interface IJwtService
{
    string GenerateAccessToken(User user);
}