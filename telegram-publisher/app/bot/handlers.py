from __future__ import annotations

import logging
import uuid

from aiogram import Router
from aiogram.types import Message

from ..db import Storage
from ..services.telegram_client import TelegramBotGateway


def build_channel_events_router(storage: Storage, gateway: TelegramBotGateway) -> Router:
    router = Router(name="channel_events")
    logger = logging.getLogger("telegram_publisher.bot")

    @router.channel_post()
    async def on_channel_post(message: Message) -> None:
        post_url = gateway.build_post_url(
            chat_id=message.chat.id,
            username=message.chat.username,
            message_id=message.message_id,
        )

        payload = gateway.extract_message_payload(message, post_url=post_url)
        await storage.save_channel_post(id=str(uuid.uuid4()), **payload)
        logger.info("Captured channel post: channel=%s message_id=%s", payload["channel_id"], payload["message_id"])

    @router.edited_channel_post()
    async def on_edited_channel_post(message: Message) -> None:
        post_url = gateway.build_post_url(
            chat_id=message.chat.id,
            username=message.chat.username,
            message_id=message.message_id,
        )

        payload = gateway.extract_message_payload(message, post_url=post_url)
        await storage.save_channel_post(id=str(uuid.uuid4()), **payload)
        logger.info("Captured edited channel post: channel=%s message_id=%s", payload["channel_id"], payload["message_id"])

    return router
