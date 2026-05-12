namespace Publishers.Presentation.Channels.Dtos;

public sealed record CreateChannelRequestDto(
    string TelegramChannelId,
    string Title,
    string? Topic,
    string? Language,
    decimal? PricePerPostRub);
