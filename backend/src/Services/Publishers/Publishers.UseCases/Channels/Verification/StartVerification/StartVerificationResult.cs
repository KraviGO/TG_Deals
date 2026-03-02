namespace Publishers.UseCases.Channels.Verification.StartVerification;

public sealed record StartVerificationResult(string Instruction, DateTimeOffset ExpiresAt);
