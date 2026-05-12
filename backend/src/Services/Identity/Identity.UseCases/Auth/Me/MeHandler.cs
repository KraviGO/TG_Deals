using Identity.UseCases.Abstractions.Persistence;
using Marketplace.Kernel.Results;
using Identity.UseCases.Common;
using Microsoft.EntityFrameworkCore;

namespace Identity.UseCases.Auth.Me;

/// <summary>
/// Возвращает профиль текущего пользователя.
/// </summary>
public sealed class MeHandler
{
    private readonly IIdentityDbContext _db;

    public MeHandler(IIdentityDbContext db) => _db = db;

    public async Task<Result<MeResult>> Handle(MeQuery query, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == query.UserId, ct);
        if (user is null) return Result<MeResult>.Fail("NotFound");
        return Result<MeResult>.Ok(new MeResult(user.Id, user.Email, user.Role));
    }
}
