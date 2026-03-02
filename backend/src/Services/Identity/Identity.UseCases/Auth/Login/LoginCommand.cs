namespace Identity.UseCases.Auth.Login;

public sealed record LoginCommand(string Email, string Password);
