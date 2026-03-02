namespace Publishers.Presentation.Channels.Dtos;

public sealed record StartVerificationResponseDto(string Instruction, DateTimeOffset ExpiresAt);
