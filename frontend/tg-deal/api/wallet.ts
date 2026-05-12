import { PAYMENTS_API_URL } from './config';
import { request } from './client';
import { PublisherLedgerEntry, PublisherWalletSummary, Wallet } from '../types';

export interface CreateTopUpResponse {
  topUpId: string;
  confirmationUrl: string;
}

export interface WalletTransactionRecord {
  txId: string;
  type: string;
  amount: number;
  currency: string;
  dealId?: string;
  topUpId?: string;
  createdAt: string;
}

export interface TopUpHistoryRecord {
  topUpId: string;
  yooKassaPaymentId: string;
  amount: number;
  currency: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface WithdrawWalletResponse {
  currency: string;
  available: number;
  reserved: number;
  total: number;
}

export interface WithdrawPublisherResponse {
  currency: string;
  requestedAmount: number;
  withdrawnAmount: number;
  available: number;
  paidOut: number;
  destinationCardMask?: string;
}

export const getMyWallet = async (token: string): Promise<Wallet> =>
  request(PAYMENTS_API_URL, '/api/v1/wallet/me', {
    method: 'GET',
    token,
  });

export const createTopUp = async (
  token: string,
  amount: number,
  currency: string
): Promise<CreateTopUpResponse> =>
  request(PAYMENTS_API_URL, '/api/v1/wallet/me/topups', {
    method: 'POST',
    token,
    body: { amount, currency },
  });

export const listMyTopUps = async (token: string, limit = 100): Promise<TopUpHistoryRecord[]> =>
  request(PAYMENTS_API_URL, `/api/v1/wallet/me/topups?limit=${limit}`, {
    method: 'GET',
    token,
  });

export const listMyWalletTransactions = async (
  token: string,
  limit = 100
): Promise<WalletTransactionRecord[]> =>
  request(PAYMENTS_API_URL, `/api/v1/wallet/me/transactions?limit=${limit}`, {
    method: 'GET',
    token,
  });

export const withdrawFromWallet = async (
  token: string,
  amount: number,
  currency: string
): Promise<WithdrawWalletResponse> =>
  request(PAYMENTS_API_URL, '/api/v1/wallet/me/withdrawals', {
    method: 'POST',
    token,
    body: { amount, currency },
  });

export const getMyPublisherWallet = async (
  token: string
): Promise<PublisherWalletSummary> =>
  request(PAYMENTS_API_URL, '/api/v1/publisher/wallet/me', {
    method: 'GET',
    token,
  });

export const listMyPublisherWalletEntries = async (
  token: string
): Promise<PublisherLedgerEntry[]> =>
  request(PAYMENTS_API_URL, '/api/v1/publisher/wallet/me/entries', {
    method: 'GET',
    token,
  });

export const withdrawPublisherBalance = async (
  token: string,
  amount: number,
  cardNumber: string
): Promise<WithdrawPublisherResponse> =>
  request(PAYMENTS_API_URL, '/api/v1/publisher/wallet/me/withdrawals', {
    method: 'POST',
    token,
    body: { amount, cardNumber },
  });
