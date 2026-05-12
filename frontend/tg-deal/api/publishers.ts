import { PUBLISHERS_API_URL } from './config';
import { request } from './client';
import { PublisherChannel } from '../types';

export const getMyChannels = async (token: string): Promise<PublisherChannel[]> =>
  request(PUBLISHERS_API_URL, '/api/v1/publishers/me/channels', { token });

export const createChannel = async (
  token: string,
  telegramChannelId: string,
  title: string,
  topic?: string,
  language?: string,
  pricePerPostRub?: number
): Promise<PublisherChannel> =>
  request(PUBLISHERS_API_URL, '/api/v1/publishers/me/channels', {
    method: 'POST',
    token,
    body: { telegramChannelId, title, topic, language, pricePerPostRub },
  });

export const updateChannel = async (
  token: string,
  channelId: string,
  telegramChannelId: string,
  title: string,
  topic?: string,
  language?: string,
  pricePerPostRub?: number
) =>
  request(PUBLISHERS_API_URL, `/api/v1/publishers/me/channels/${channelId}`, {
    method: 'PUT',
    token,
    body: { telegramChannelId, title, topic, language, pricePerPostRub },
  });

export const confirmVerification = async (token: string, channelId: string) =>
  request(PUBLISHERS_API_URL, `/api/v1/publishers/me/channels/${channelId}/verify/confirm`, {
    method: 'POST',
    token,
  });

export const setChannelIntakeMode = async (
  token: string,
  channelId: string,
  mode: 'ActiveAuto' | 'ActiveManual' | 'Paused'
) =>
  request(PUBLISHERS_API_URL, `/api/v1/publishers/me/channels/${channelId}/intake-mode`, {
    method: 'PATCH',
    token,
    body: { mode },
  });

export const adminBanChannel = async (token: string, channelId: string) =>
  request(PUBLISHERS_API_URL, `/api/v1/admin/publishers/channels/${channelId}/ban`, {
    method: 'POST',
    token,
  });
