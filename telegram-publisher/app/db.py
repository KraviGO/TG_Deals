from __future__ import annotations

from contextlib import asynccontextmanager
from dataclasses import dataclass
from datetime import datetime, timezone
from typing import Any

import aiosqlite


@dataclass(slots=True)
class StoredChannelPost:
    id: str
    channel_id: str
    message_id: int
    posted_at_utc: datetime
    content_type: str
    text: str | None
    caption: str | None
    media_group_id: str | None
    post_url: str | None
    raw_json: str


class Storage:
    def __init__(self, db_path: str) -> None:
        self._db_path = db_path

    @asynccontextmanager
    async def _conn(self):
        db = await aiosqlite.connect(self._db_path)
        db.row_factory = aiosqlite.Row
        try:
            yield db
        finally:
            await db.close()

    async def init(self) -> None:
        async with self._conn() as db:
            await db.execute("PRAGMA journal_mode=WAL;")
            await db.execute(
                """
                CREATE TABLE IF NOT EXISTS channel_posts (
                    id TEXT PRIMARY KEY,
                    channel_id TEXT NOT NULL,
                    message_id INTEGER NOT NULL,
                    posted_at_utc TEXT NOT NULL,
                    content_type TEXT NOT NULL,
                    text TEXT NULL,
                    caption TEXT NULL,
                    media_group_id TEXT NULL,
                    post_url TEXT NULL,
                    raw_json TEXT NOT NULL,
                    UNIQUE(channel_id, message_id)
                );
                """
            )
            await db.execute(
                """
                CREATE INDEX IF NOT EXISTS ix_channel_posts_channel_posted
                ON channel_posts(channel_id, posted_at_utc DESC);
                """
            )
            await db.commit()

    async def save_channel_post(
        self,
        *,
        id: str,
        channel_id: str,
        message_id: int,
        posted_at_utc: datetime,
        content_type: str,
        text: str | None,
        caption: str | None,
        media_group_id: str | None,
        post_url: str | None,
        raw_json: str,
    ) -> None:
        async with self._conn() as db:
            await db.execute(
                """
                INSERT INTO channel_posts(
                    id, channel_id, message_id, posted_at_utc,
                    content_type, text, caption, media_group_id,
                    post_url, raw_json
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                ON CONFLICT(channel_id, message_id) DO UPDATE SET
                    posted_at_utc=excluded.posted_at_utc,
                    content_type=excluded.content_type,
                    text=excluded.text,
                    caption=excluded.caption,
                    media_group_id=excluded.media_group_id,
                    post_url=excluded.post_url,
                    raw_json=excluded.raw_json;
                """,
                (
                    id,
                    channel_id,
                    message_id,
                    posted_at_utc.astimezone(timezone.utc).isoformat(),
                    content_type,
                    text,
                    caption,
                    media_group_id,
                    post_url,
                    raw_json,
                ),
            )
            await db.commit()

    async def get_recent_channel_posts(self, channel_id: str, limit: int) -> list[StoredChannelPost]:
        async with self._conn() as db:
            cursor = await db.execute(
                """
                SELECT id, channel_id, message_id, posted_at_utc, content_type,
                       text, caption, media_group_id, post_url, raw_json
                FROM channel_posts
                WHERE channel_id = ?
                ORDER BY posted_at_utc DESC
                LIMIT ?;
                """,
                (channel_id, limit),
            )
            rows = await cursor.fetchall()

        return [self._map_post(r) for r in rows]

    @staticmethod
    def _map_post(row: aiosqlite.Row | dict[str, Any]) -> StoredChannelPost:
        return StoredChannelPost(
            id=row["id"],
            channel_id=row["channel_id"],
            message_id=row["message_id"],
            posted_at_utc=datetime.fromisoformat(row["posted_at_utc"]),
            content_type=row["content_type"],
            text=row["text"],
            caption=row["caption"],
            media_group_id=row["media_group_id"],
            post_url=row["post_url"],
            raw_json=row["raw_json"],
        )
