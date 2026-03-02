namespace ChannelCatalog.Presentation.Catalog.Dtos;

public sealed record CatalogChannelResponseDto(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string IntakeMode,
    string OwnershipStatus
);
