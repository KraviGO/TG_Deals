using Identity.Entities.Users;
using Identity.UseCases.Abstractions.Auth;
using Identity.UseCases.Abstractions.Persistence;
using Identity.UseCases.Common;
using Microsoft.EntityFrameworkCore;

namespace Identity.UseCases.Auth.Register;

public sealed class RegisterHandler
{
    private readonly IIdentityDbContext _db;
    private readonly IPasswordHasher _hasher;

    public RegisterHandler(IIdentityDbContext db, IPasswordHasher hasher)
    {
        _db = db;
        _hasher = hasher;
    }

    public async Task<Result<RegisterResult>> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var normalizedEmail = cmd.Email.Trim().ToLowerInvariant();

        var exists = await _db.Users.AnyAsync(u => u.Email == normalizedEmail, ct);
        if (exists) return Result<RegisterResult>.Fail(Errors.EmailAlreadyExists);

        var hash = _hasher.Hash(cmd.Password);
        var user = User.Create(normalizedEmail, hash, cmd.Role);

        await _db.AddUserAsync(user, ct);
        await _db.SaveChangesAsync(ct);

        return Result<RegisterResult>.Ok(new RegisterResult(user.Id, user.Email, user.Role));
    }
}
