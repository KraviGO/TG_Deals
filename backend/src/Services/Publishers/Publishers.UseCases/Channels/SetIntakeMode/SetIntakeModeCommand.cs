using Publishers.Entities.Channels;

namespace Publishers.UseCases.Channels.SetIntakeMode;

public sealed record SetIntakeModeCommand(Guid PublisherUserId, Guid ChannelId, IntakeMode Mode);
