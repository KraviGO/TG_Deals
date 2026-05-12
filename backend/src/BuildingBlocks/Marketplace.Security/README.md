# Marketplace.Security

## Назначение

Общая JWT-аутентификация и role-based authorization для пользовательских API.
Библиотека задает единый контракт проверки access token во всех .NET сервисах,
которые принимают запросы от пользователя через API Gateway или напрямую в dev-среде.

`Marketplace.Security` не отвечает за service-to-service авторизацию. Internal
endpoint'ы с `X-Service-Token` относятся к `Marketplace.ServiceAuth`.

## Какие проблемы решает

- Все сервисы одинаково валидируют JWT от `Identity`.
- Все сервисы одинаково понимают роли пользователей.
- Claims имеют единые имена.
- Endpoint'ы без явного `[AllowAnonymous]` по умолчанию требуют аутентифицированного пользователя.
- Контроллеры получают единые helpers для извлечения `UserId` и роли из `ClaimsPrincipal`.

## Основные типы

- `Jwt/JwtAuthenticationExtensions.cs` - `AddMarketplaceJwt`.
- `Jwt/JwtOptions.cs` - `Issuer`, `Audience`, `SigningKey`, `ExpiresMinutes`.
- `Roles/RolePoliciesExtensions.cs` - policies `Advertiser`, `Publisher`, `Admin`.
- `Common/ClaimNames.cs` - имена claims, включая `sub`, `email`, `role`.
- `Common/ClaimsPrincipalExtensions.cs` - helpers `GetUserId()` и `GetRole()`.

## JWT authentication

`AddMarketplaceJwt` регистрирует стандартную JWT Bearer схему:

```csharp
builder.Services.AddMarketplaceJwt(builder.Configuration);
```

Настройки читаются из секции:

```json
{
  "Jwt": {
    "Issuer": "Marketplace.Identity",
    "Audience": "Marketplace",
    "SigningKey": "<secret>",
    "ExpiresMinutes": 60
  }
}
```

Проверка токена включает:

- `ValidateIssuer` - token должен быть выпущен ожидаемым issuer.
- `ValidateAudience` - token должен быть предназначен текущему приложению/системе.
- `ValidateLifetime` - просроченные token отклоняются.
- `ValidateIssuerSigningKey` - подпись проверяется по симметричному `SigningKey`.
- `ClockSkew = 30 seconds` - небольшой допуск на рассинхронизацию часов между сервисами.

`MapInboundClaims = false` отключает автоматическое преобразование коротких claim names
в URI-формат. Поэтому сервисы работают с исходными claim names:

- `sub`;
- `email`;
- `role`.

`NameClaimType` настроен на `sub`, а `RoleClaimType` - на `role`.

## Claims contract

Контрактные claim names описаны в `ClaimNames`:

```csharp
ClaimNames.Subject // "sub"
ClaimNames.Email   // "email"
ClaimNames.Role    // "role"
```

`Identity` при выпуске access token должен класть в token те же claim names.
Потребители токена не должны хардкодить альтернативные имена claims в разных сервисах.

Для контроллеров есть helpers:

```csharp
var userId = User.GetUserId();
var role = User.GetRole();
```

`GetUserId()` читает `sub` и fallback `ClaimTypes.NameIdentifier`. Если claim отсутствует
или не является `Guid`, возвращается `Guid.Empty`; контроллеру нужно учитывать это для
критичных сценариев.

## Role policies

`AddMarketplaceRolePolicies` регистрирует policies:

```csharp
builder.Services.AddMarketplaceRolePolicies();
```

Доступные политики:

- `Advertiser` - требует claim `role=Advertiser`.
- `Publisher` - требует claim `role=Publisher`.
- `Admin` - требует claim `role=Admin`.

Пример:

```csharp
[Authorize(Policy = "Publisher")]
public sealed class ChannelsController : ControllerBase
{
}
```

Для endpoint'ов, где достаточно любого валидного пользователя, используется:

```csharp
[Authorize]
```

## FallbackPolicy

`AddMarketplaceRolePolicies` также задает global `FallbackPolicy`:

```text
любой endpoint без [AllowAnonymous] требует authenticated user
```

Это снижает риск случайно оставить controller action публичным. Если endpoint должен быть
публичным, он обязан явно иметь:

```csharp
[AllowAnonymous]
```

Типичные публичные endpoint'ы:

- login;
- register;
- public catalog/search;
- external webhook, если он защищается отдельной проверкой.

## Как подключать в Host

Типовой порядок:

```csharp
builder.Services.AddMarketplaceJwt(builder.Configuration);
builder.Services.AddMarketplaceRolePolicies();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
```

`UseAuthentication()` должен идти до `UseAuthorization()`.
Оба middleware должны быть подключены до `MapControllers()`.

## Кто использует

`Identity`, `Publishers`, `ChannelCatalog`, `Deals`, `Payments`.

`Identity` использует тот же JWT configuration contract для выпуска token. Остальные
сервисы используют его для проверки token и применения role policies.

## Правила использования

- Пользовательские endpoint'ы защищать JWT и role policies.
- Для role-specific endpoint'ов использовать `[Authorize(Policy = "...")]`.
- Для endpoint'ов любого авторизованного пользователя использовать `[Authorize]`.
- Для публичных endpoint'ов явно ставить `[AllowAnonymous]`.
- Не использовать эту библиотеку для internal service-to-service token.
- Internal service auth находится в `Marketplace.ServiceAuth`.
- `SigningKey` хранить в secrets/environment, а не в исходном коде.
- Значения role claim должны совпадать с policy names: `Advertiser`, `Publisher`, `Admin`.
