namespace Identity.Presentation.Auth.Dtos;

public sealed record RegisterResponseDto(Guid UserId, string Email, string Role);
