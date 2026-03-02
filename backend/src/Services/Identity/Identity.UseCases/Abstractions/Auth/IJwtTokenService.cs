using Identity.Entities.Users;

namespace Identity.UseCases.Abstractions.Auth;

public interface IJwtTokenService
{
    string CreateAccessToken(User user);
    int ExpiresInSeconds { get; }
}
