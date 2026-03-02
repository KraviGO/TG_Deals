namespace Identity.Presentation.Auth.Dtos;

public sealed record RegisterRequestDto(string Email, string Password, string Role);
