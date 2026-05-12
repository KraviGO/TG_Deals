from __future__ import annotations

import uuid

from fastapi import APIRouter, Depends, HTTPException, Query, status

from ..db import Storage
from ..dependencies import get_gateway, get_storage
from ..schemas import (
    ChannelInfoResponse,
    ChannelPostResponse,
    DeletePostResponse,
    ListChannelPostsResponse,
    PublishPostRequest,
    PublishPostResponse,
)
from ..security import require_service_token
from ..services.telegram_client import TelegramBadRequest, TelegramBotGateway, TelegramBotNotConfiguredError, TelegramForbiddenError

router = APIRouter(
    prefix="/api/v1/internal/telegram",
    tags=["internal-telegram"],
    dependencies=[Depends(require_service_token)],
)


def _normalize_channel_key(channel_id: str, gateway: TelegramBotGateway) -> str:
    # Внутри storage используем один формат ключа, даже если клиент прислал @name или ссылку t.me.
    normalized = gateway.normalize_channel_id(channel_id)
    return str(normalized)


def _require_configured_bot(gateway: TelegramBotGateway) -> None:
    if not gateway.is_configured:
        raise HTTPException(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            detail="TelegramBotNotConfigured",
        )


async def _resolve_channel_storage_key(channel_id: str, gateway: TelegramBotGateway) -> str:
    normalized_key = _normalize_channel_key(channel_id, gateway)
    if normalized_key.startswith("@") and gateway.is_configured:
        # Для публичного username получаем числовой chat id: Telegram может сменить username,
        # а история постов должна остаться привязанной к стабильному id канала.
        info = await gateway.get_channel_info(channel_id)
        return info["channel_id"]
    return normalized_key


@router.get("/channels/{channel_id}", response_model=ChannelInfoResponse)
async def get_channel_info(
    channel_id: str,
    gateway: TelegramBotGateway = Depends(get_gateway),
) -> ChannelInfoResponse:
    _require_configured_bot(gateway)

    try:
        info = await gateway.get_channel_info(channel_id)
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc
    except TelegramForbiddenError as exc:
        raise HTTPException(status_code=403, detail="BotHasNoAccessToChannel") from exc
    except TelegramBadRequest as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc

    return ChannelInfoResponse(**info)


@router.get("/channels/{channel_id}/posts", response_model=ListChannelPostsResponse)
async def get_channel_posts(
    channel_id: str,
    limit: int = Query(default=30, ge=1, le=200),
    gateway: TelegramBotGateway = Depends(get_gateway),
    storage: Storage = Depends(get_storage),
) -> ListChannelPostsResponse:
    try:
        channel_key = await _resolve_channel_storage_key(channel_id, gateway)
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc
    except TelegramForbiddenError as exc:
        raise HTTPException(status_code=403, detail="BotHasNoAccessToChannel") from exc
    except TelegramBadRequest as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc
    rows = await storage.get_recent_channel_posts(channel_key, limit)

    items = [
        ChannelPostResponse(
            messageId=r.message_id,
            postedAtUtc=r.posted_at_utc,
            contentType=r.content_type,
            text=r.text,
            caption=r.caption,
            mediaGroupId=r.media_group_id,
            postUrl=r.post_url,
        )
        for r in rows
    ]

    return ListChannelPostsResponse(channelId=channel_key, items=items)


@router.post("/channels/{channel_id}/posts", response_model=PublishPostResponse, status_code=status.HTTP_201_CREATED)
async def publish_post(
    channel_id: str,
    payload: PublishPostRequest,
    gateway: TelegramBotGateway = Depends(get_gateway),
    storage: Storage = Depends(get_storage),
) -> PublishPostResponse:
    _require_configured_bot(gateway)

    try:
        message = await gateway.publish_post(channel_id, payload)
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc
    except TelegramForbiddenError as exc:
        raise HTTPException(status_code=403, detail="BotHasNoAccessToChannel") from exc
    except TelegramBadRequest as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc
    except TelegramBotNotConfiguredError as exc:
        raise HTTPException(status_code=503, detail="TelegramBotNotConfigured") from exc

    # Сохраняем опубликованный пост локально, чтобы можно было показывать историю
    # и при необходимости удалять/диагностировать публикации без запроса к Telegram.
    post_url = gateway.build_post_url(chat_id=message.chat.id, username=message.chat.username, message_id=message.message_id)
    extracted = gateway.extract_message_payload(message, post_url=post_url)
    await storage.save_channel_post(id=str(uuid.uuid4()), **extracted)

    return PublishPostResponse(
        messageId=message.message_id,
        postedAtUtc=message.date,
        postUrl=post_url,
    )


@router.delete("/channels/{channel_id}/posts/{message_id}", response_model=DeletePostResponse)
async def delete_post(
    channel_id: str,
    message_id: int,
    gateway: TelegramBotGateway = Depends(get_gateway),
) -> DeletePostResponse:
    _require_configured_bot(gateway)

    try:
        ok = await gateway.delete_post(channel_id, message_id)
    except ValueError as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc
    except TelegramForbiddenError as exc:
        raise HTTPException(status_code=403, detail="BotHasNoAccessToChannel") from exc
    except TelegramBadRequest as exc:
        raise HTTPException(status_code=400, detail=str(exc)) from exc

    return DeletePostResponse(ok=ok)
