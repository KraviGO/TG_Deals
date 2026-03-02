namespace Identity.Presentation.Auth.Dtos;

public sealed record MeResponseDto(Guid UserId, string Email, string Role);
