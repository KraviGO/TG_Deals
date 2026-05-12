from __future__ import annotations

from fastapi import Request

from .config import Settings
from .db import Storage
from .services.telegram_client import TelegramBotGateway


def get_settings_from_app(request: Request) -> Settings:
    return request.app.state.settings


def get_storage(request: Request) -> Storage:
    return request.app.state.storage


def get_gateway(request: Request) -> TelegramBotGateway:
    return request.app.state.gateway
