using Identity.Entities.Users;

namespace Identity.UseCases.Auth.Me;

public sealed record MeResult(Guid UserId, string Email, UserRole Role);
