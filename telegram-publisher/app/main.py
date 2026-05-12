from __future__ import annotations

import asyncio
import contextlib
import logging
from contextlib import asynccontextmanager

from aiogram import Dispatcher
from fastapi import FastAPI

from .bot.handlers import build_channel_events_router
from .config import Settings, get_settings
from .db import Storage
from .routers.health import router as health_router
from .routers.internal import router as internal_router
from .services.telegram_client import TelegramBotGateway


def _configure_logging() -> None:
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s %(levelname)s [%(name)s] %(message)s",
    )


async def _start_polling_task(dp: Dispatcher, gateway: TelegramBotGateway) -> None:
    await dp.start_polling(gateway.bot, allowed_updates=["channel_post", "edited_channel_post"])


@asynccontextmanager
async def lifespan(app: FastAPI):
    _configure_logging()
    logger = logging.getLogger("telegram_publisher.app")

    settings: Settings = get_settings()
    storage = Storage(settings.telegram_db_path)
    await storage.init()

    gateway = TelegramBotGateway(settings)

    app.state.settings = settings
    app.state.storage = storage
    app.state.gateway = gateway
    app.state.poll_task = None

    if settings.telegram_bot_mode == "polling" and gateway.is_configured:
        dp = Dispatcher()
        dp.include_router(build_channel_events_router(storage, gateway))
        app.state.dispatcher = dp
        app.state.poll_task = asyncio.create_task(_start_polling_task(dp, gateway))
        logger.info("Telegram polling started.")
    elif settings.telegram_bot_mode == "disabled":
        logger.info("Telegram bot mode is disabled.")
    else:
        logger.warning("TELEGRAM_BOT_TOKEN is not configured. API is available, bot actions will return 503.")

    try:
        yield
    finally:
        poll_task: asyncio.Task | None = app.state.poll_task
        if poll_task is not None:
            poll_task.cancel()
            with contextlib.suppress(asyncio.CancelledError):
                await poll_task

        await gateway.close()


app = FastAPI(
    title="Telegram Publisher API",
    version="0.1.0",
    lifespan=lifespan,
)

app.include_router(health_router)
app.include_router(internal_router)
