namespace Identity.UseCases.Auth.Login;

public sealed record LoginResult(string AccessToken, string TokenType, int ExpiresInSeconds);
