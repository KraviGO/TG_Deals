namespace ChannelCatalog.Presentation.Catalog.Dtos;

public sealed record CatalogChannelResponseDto(
    Guid ChannelId,
    string TelegramChannelId,
    string Title,
    string Topic,
    string Language,
    decimal PricePerPostRub,
    string IntakeMode,
    string OwnershipStatus
);
