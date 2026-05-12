namespace Marketplace.ServiceAuth.Constants;

/// <summary>
/// Имена HTTP-заголовков для service-to-service авторизации.
/// Константы фиксируют единое написание заголовков для серверов и клиентов.
/// </summary>
public static class ServiceAuthHeaderNames
{
    /// <summary>
    /// Заголовок с shared token для внутренних endpoint'ов.
    /// </summary>
    public const string ServiceToken = "X-Service-Token";
}
