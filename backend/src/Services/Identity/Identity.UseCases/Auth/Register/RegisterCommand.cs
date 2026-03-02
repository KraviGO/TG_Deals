using Identity.Entities.Users;

namespace Identity.UseCases.Auth.Register;

public sealed record RegisterCommand(string Email, string Password, UserRole Role);
