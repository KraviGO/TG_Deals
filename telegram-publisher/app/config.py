from __future__ import annotations

from functools import lru_cache
from typing import Literal

from pydantic import Field
from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    model_config = SettingsConfigDict(env_file=".env", env_file_encoding="utf-8", extra="ignore")

    app_name: str = "telegram-publisher"
    app_version: str = "0.1.0"

    host: str = "0.0.0.0"
    port: int = 8080

    service_token: str | None = Field(default=None, alias="SERVICE_TOKEN")

    telegram_bot_token: str | None = Field(default=None, alias="TELEGRAM_BOT_TOKEN")
    telegram_bot_mode: Literal["polling", "disabled"] = Field(default="polling", alias="TELEGRAM_BOT_MODE")
    telegram_proxy_url: str | None = Field(default=None, alias="TELEGRAM_PROXY_URL")

    telegram_db_path: str = Field(default="/data/telegram_publisher.db", alias="TELEGRAM_DB_PATH")


@lru_cache(maxsize=1)
def get_settings() -> Settings:
    return Settings()
