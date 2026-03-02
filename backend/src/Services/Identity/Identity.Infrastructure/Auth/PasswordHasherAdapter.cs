using Identity.UseCases.Abstractions.Auth;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Auth;

public sealed class PasswordHasherAdapter : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(new object(), password);

    public bool Verify(string password, string passwordHash)
    {
        var res = _hasher.VerifyHashedPassword(new object(), passwordHash, password);
        return res is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
