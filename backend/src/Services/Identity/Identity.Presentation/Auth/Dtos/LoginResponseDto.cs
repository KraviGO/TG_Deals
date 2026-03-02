namespace Identity.Presentation.Auth.Dtos;

public sealed record LoginResponseDto(string AccessToken, string TokenType, int ExpiresInSeconds);
