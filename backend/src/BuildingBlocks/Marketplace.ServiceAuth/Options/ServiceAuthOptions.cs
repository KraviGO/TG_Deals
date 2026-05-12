using Microsoft.AspNetCore.Authentication;

namespace Marketplace.ServiceAuth.Options;

/// <summary>
/// Настройки проверки X-Service-Token на внутренних HTTP endpoint'ах.
/// Значения читаются из конфигурационной секции ServiceAuth.
/// </summary>
public sealed class ServiceAuthOptions : AuthenticationSchemeOptions
{
    /// <summary>
    /// Shared token, который внутренние клиенты передают в X-Service-Token.
    /// </summary>
    public string Token { get; set; } = default!;
}
