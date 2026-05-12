using Identity.Entities.Users;
using Identity.UseCases.Abstractions.Auth;
using Identity.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Identity.UseCases.Common;
using Microsoft.EntityFrameworkCore;

namespace Identity.UseCases.Auth.Login;

/// <summary>
/// Проверяет пароль и выдает access token.
/// </summary>
public sealed class LoginHandler
{
    private readonly IIdentityDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public LoginHandler(IIdentityDbContext db, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Result<LoginResult>> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var email = cmd.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (user is null) return Result<LoginResult>.Fail(Errors.InvalidCredentials);

        if (user.Status == UserStatus.Suspended)
            return Result<LoginResult>.Fail(Errors.UserSuspended);

        // Неверный email и неверный пароль возвращают одинаковую ошибку.
        var ok = _hasher.Verify(cmd.Password, user.PasswordHash);
        if (!ok) return Result<LoginResult>.Fail(Errors.InvalidCredentials);

        var token = _jwt.CreateAccessToken(user);
        return Result<LoginResult>.Ok(new LoginResult(token, "Bearer", _jwt.ExpiresInSeconds));
    }
}
