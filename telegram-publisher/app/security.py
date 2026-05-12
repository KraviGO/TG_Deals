from __future__ import annotations

import hmac

from fastapi import Depends, Header, HTTPException, status

from .config import Settings, get_settings


async def require_service_token(
    x_service_token: str | None = Header(default=None, alias="X-Service-Token"),
    settings: Settings = Depends(get_settings),
) -> None:
    expected = settings.service_token
    if not expected:
        # Запрет запуска без токена — все internal-маршруты должны быть защищены.
        raise HTTPException(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            detail="ServiceTokenNotConfigured",
        )

    if not x_service_token or not hmac.compare_digest(x_service_token, expected):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="InvalidServiceToken",
        )
