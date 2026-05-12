from __future__ import annotations

from datetime import datetime
from typing import Literal

from pydantic import BaseModel, ConfigDict, Field, field_validator, model_validator


class ApiModel(BaseModel):
    model_config = ConfigDict(populate_by_name=True)


class ChannelPostResponse(ApiModel):
    message_id: int = Field(alias="messageId")
    posted_at_utc: datetime = Field(alias="postedAtUtc")
    content_type: str = Field(alias="contentType")
    text: str | None = None
    caption: str | None = None
    media_group_id: str | None = Field(default=None, alias="mediaGroupId")
    post_url: str | None = Field(default=None, alias="postUrl")


class ListChannelPostsResponse(ApiModel):
    channel_id: str = Field(alias="channelId")
    items: list[ChannelPostResponse]


class PublishPostRequest(ApiModel):
    text: str | None = None
    caption: str | None = None
    parse_mode: Literal["HTML", "MarkdownV2"] | None = Field(default=None, alias="parseMode")
    disable_web_page_preview: bool = Field(default=False, alias="disableWebPagePreview")
    photo_url: str | None = Field(default=None, alias="photoUrl")
    video_url: str | None = Field(default=None, alias="videoUrl")

    @model_validator(mode="after")
    def validate_payload(self) -> "PublishPostRequest":
        has_text = bool(self.text and self.text.strip())
        has_photo = bool(self.photo_url and self.photo_url.strip())
        has_video = bool(self.video_url and self.video_url.strip())

        if not any([has_text, has_photo, has_video]):
            raise ValueError("Either text, photoUrl or videoUrl is required.")

        if has_photo and has_video:
            raise ValueError("Only one media field is allowed: photoUrl or videoUrl.")

        if (has_photo or has_video) and not (self.caption and self.caption.strip()):
            # Для медиа допускаем пустой caption, но publish_post подставит text как подпись.
            return self

        return self

    @field_validator("text", "caption")
    @classmethod
    def trim_optional_text(cls, value: str | None) -> str | None:
        if value is None:
            return None
        trimmed = value.strip()
        return trimmed or None


class PublishPostResponse(ApiModel):
    message_id: int = Field(alias="messageId")
    posted_at_utc: datetime = Field(alias="postedAtUtc")
    post_url: str | None = Field(default=None, alias="postUrl")


class DeletePostResponse(ApiModel):
    ok: bool


class ChannelInfoResponse(ApiModel):
    channel_id: str = Field(alias="channelId")
    title: str
    username: str | None = None
    description: str | None = None
    type: str
    is_bot_admin: bool = Field(alias="isBotAdmin")
    can_post_messages: bool = Field(alias="canPostMessages")
    can_delete_messages: bool = Field(alias="canDeleteMessages")
