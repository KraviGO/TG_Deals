from __future__ import annotations

import re
from datetime import timezone

from aiogram import Bot
from aiogram.client.session.aiohttp import AiohttpSession
from aiogram.enums import ChatMemberStatus, ChatType
from aiogram.exceptions import TelegramBadRequest, TelegramForbiddenError
from aiogram.types import Message

from ..config import Settings
from ..schemas import PublishPostRequest


class TelegramBotNotConfiguredError(RuntimeError):
    pass


class TelegramBotGateway:
    def __init__(self, settings: Settings) -> None:
        self._settings = settings
        if settings.telegram_bot_token:
            # Прокси нужен для серверов, с которых Telegram Bot API недоступен напрямую.
            session = AiohttpSession(proxy=settings.telegram_proxy_url) if settings.telegram_proxy_url else None
            self._bot: Bot | None = Bot(settings.telegram_bot_token, session=session)
        else:
            self._bot = None

    @property
    def is_configured(self) -> bool:
        return self._bot is not None

    @property
    def bot(self) -> Bot:
        if not self._bot:
            raise TelegramBotNotConfiguredError("Telegram bot token is not configured.")
        return self._bot

    async def close(self) -> None:
        if self._bot:
            await self._bot.session.close()

    def normalize_channel_id(self, channel_id: str) -> int | str:
        if not channel_id or not channel_id.strip():
            raise ValueError("ChannelId is required.")

        raw = channel_id.strip()

        if raw.startswith("https://t.me/"):
            raw = raw.removeprefix("https://t.me/")
            raw = raw.split("/")[0]

        if re.fullmatch(r"-?\d+", raw):
            return int(raw)

        if raw.startswith("@"):
            return raw

        if re.fullmatch(r"[A-Za-z0-9_]{5,64}", raw):
            return f"@{raw}"

        raise ValueError("Unsupported Telegram channel id format.")

    def build_post_url(self, *, chat_id: int, username: str | None, message_id: int) -> str | None:
        if username:
            return f"https://t.me/{username}/{message_id}"

        chat_str = str(chat_id)
        if chat_str.startswith("-100"):
            # Ссылка для приватных каналов и supergroup-style id.
            return f"https://t.me/c/{chat_str[4:]}/{message_id}"

        return None

    async def get_channel_info(self, channel_id: str) -> dict:
        chat_id = self.normalize_channel_id(channel_id)
        bot = self.bot

        chat = await bot.get_chat(chat_id)
        me = await bot.get_me()
        member = await bot.get_chat_member(chat.id, me.id)

        # Проверяем именно права бота в канале: владелец платформы не может публиковать
        # рекламу, если бот не администратор или ему запрещена публикация сообщений.
        is_admin = member.status in {ChatMemberStatus.ADMINISTRATOR, ChatMemberStatus.CREATOR}
        can_post = bool(getattr(member, "can_post_messages", False) or member.status == ChatMemberStatus.CREATOR)
        can_delete = bool(getattr(member, "can_delete_messages", False) or member.status == ChatMemberStatus.CREATOR)

        return {
            "channel_id": str(chat.id),
            "title": chat.title or "",
            "username": chat.username,
            "description": chat.description,
            "type": chat.type,
            "is_bot_admin": is_admin,
            "can_post_messages": can_post,
            "can_delete_messages": can_delete,
        }

    async def publish_post(self, channel_id: str, payload: PublishPostRequest) -> Message:
        chat_id = self.normalize_channel_id(channel_id)
        bot = self.bot

        if payload.photo_url:
            return await bot.send_photo(
                chat_id=chat_id,
                photo=payload.photo_url,
                caption=payload.caption or payload.text,
                parse_mode=payload.parse_mode,
            )

        if payload.video_url:
            return await bot.send_video(
                chat_id=chat_id,
                video=payload.video_url,
                caption=payload.caption or payload.text,
                parse_mode=payload.parse_mode,
            )

        return await bot.send_message(
            chat_id=chat_id,
            text=payload.text or "",
            parse_mode=payload.parse_mode,
            disable_web_page_preview=payload.disable_web_page_preview,
        )

    async def delete_post(self, channel_id: str, message_id: int) -> bool:
        chat_id = self.normalize_channel_id(channel_id)
        return await self.bot.delete_message(chat_id=chat_id, message_id=message_id)

    @staticmethod
    def extract_message_payload(message: Message, *, post_url: str | None = None) -> dict:
        content_type = getattr(message.content_type, "value", str(message.content_type))
        text = message.text
        caption = message.caption

        return {
            "channel_id": str(message.chat.id),
            "message_id": message.message_id,
            "posted_at_utc": message.date.astimezone(timezone.utc),
            "content_type": content_type,
            "text": text,
            "caption": caption,
            "media_group_id": message.media_group_id,
            "post_url": post_url,
            "raw_json": message.model_dump_json(),
        }


__all__ = [
    "TelegramBotGateway",
    "TelegramBotNotConfiguredError",
    "TelegramBadRequest",
    "TelegramForbiddenError",
    "ChatType",
]
