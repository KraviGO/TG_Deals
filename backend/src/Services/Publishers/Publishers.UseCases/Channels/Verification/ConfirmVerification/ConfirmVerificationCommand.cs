namespace Publishers.UseCases.Channels.Verification.ConfirmVerification;

public sealed record ConfirmVerificationCommand(Guid PublisherUserId, Guid ChannelId);
