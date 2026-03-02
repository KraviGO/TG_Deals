using Identity.Entities.Users;

namespace Identity.UseCases.Auth.Register;

public sealed record RegisterResult(Guid UserId, string Email, UserRole Role);
