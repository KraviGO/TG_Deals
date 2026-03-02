namespace Publishers.UseCases.Channels.Verification.StartVerification;

public sealed record StartVerificationCommand(Guid PublisherUserId, Guid ChannelId);
