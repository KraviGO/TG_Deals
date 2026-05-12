namespace Marketplace.ServiceAuth.Options;

/// <summary>
/// Настройки клиента для отправки X-Service-Token во внутренних HTTP-запросах.
/// Значения читаются из конфигурационной секции ServiceAuth.
/// </summary>
public sealed class ServiceAuthClientOptions
{
    public const string SectionName = "ServiceAuth";

    /// <summary>
    /// Token для заголовка X-Service-Token.
    /// Значение совпадает с Token сервера-получателя.
    /// </summary>
    public string Token { get; set; } = string.Empty;
}
