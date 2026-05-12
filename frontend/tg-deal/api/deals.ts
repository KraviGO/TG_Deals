import { DEALS_API_URL } from './config';
import { request } from './client';
import { AdminDealDispute, DealSummary, PublisherDeal } from '../types';

export interface CreateDealRequest {
  channelId: string;
  postText: string;
  desiredPublishAtUtc: string; // Строка времени желаемой публикации в ISO-формате.
  amount: number;
  currency: string;
}

export interface CreateDealResponse {
  dealId: string;
  status: string;
  fundingStatus: string;
  reservationId?: string;
}

export interface OpenDisputeResponse {
  disputeId: string;
  status: string;
}

export const createDeal = async (
  token: string,
  payload: CreateDealRequest
): Promise<CreateDealResponse> =>
  request(DEALS_API_URL, '/api/v1/deals', {
    method: 'POST',
    token,
    body: payload,
  });

export const listMyDeals = async (token: string): Promise<DealSummary[]> =>
  request(DEALS_API_URL, '/api/v1/deals/me', { token });

export const listPublisherInbox = async (token: string): Promise<PublisherDeal[]> =>
  request(DEALS_API_URL, '/api/v1/deals/publisher/inbox', { token });

export const sendPublisherDecision = async (token: string, dealId: string, accept: boolean) =>
  request(DEALS_API_URL, `/api/v1/deals/${dealId}/publisher-decision`, {
    method: 'POST',
    token,
    body: { accept },
  });

export const confirmPublished = async (
  token: string,
  dealId: string,
  postUrl?: string,
  publishedAtUtc?: string,
  publisherComment?: string
) =>
  request(DEALS_API_URL, `/api/v1/deals/${dealId}/confirm-published`, {
    method: 'POST',
    token,
    body: { postUrl, publishedAtUtc, publisherComment },
  });

export const advertiserConfirmDeal = async (token: string, dealId: string) =>
  request(DEALS_API_URL, `/api/v1/deals/${dealId}/advertiser-confirm`, {
    method: 'POST',
    token,
  });

export const cancelDeal = async (token: string, dealId: string) =>
  request(DEALS_API_URL, `/api/v1/deals/${dealId}/cancel`, {
    method: 'POST',
    token,
  });

export const openDealDispute = async (
  token: string,
  dealId: string,
  reason: string
): Promise<OpenDisputeResponse> =>
  request(DEALS_API_URL, `/api/v1/deals/${dealId}/disputes`, {
    method: 'POST',
    token,
    body: { reason },
  });

export const resolveDealDispute = async (
  token: string,
  dealId: string,
  action: 'capture' | 'release',
  resolutionNote?: string
) =>
  request(DEALS_API_URL, `/api/v1/admin/deals/${dealId}/disputes/resolve`, {
    method: 'POST',
    token,
    body: { action, resolutionNote },
  });

export const listAdminDisputes = async (
  token: string,
  status?: 'open' | 'resolved' | 'all',
  limit = 200
): Promise<AdminDealDispute[]> => {
  const params = new URLSearchParams();
  if (status) params.set('status', status);
  params.set('limit', String(limit));
  const query = params.toString();

  return request(DEALS_API_URL, `/api/v1/admin/deals/disputes${query ? `?${query}` : ''}`, {
    token,
  });
};
