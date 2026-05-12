import { STORAGE_KEYS } from './config';

export class ApiError extends Error {
  status: number;
  details?: unknown;

  constructor(message: string, status: number, details?: unknown) {
    super(message);
    this.status = status;
    this.details = details;
  }
}

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE';

interface RequestOptions {
  method?: HttpMethod;
  token?: string | null;
  body?: unknown;
}

const buildHeaders = (token?: string | null): HeadersInit => {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  };

  if (token) headers['Authorization'] = `Bearer ${token}`;
  return headers;
};

const handleResponse = async (res: Response) => {
  const text = await res.text();
  let data: any = null;
  if (text) {
    try {
      data = JSON.parse(text);
    } catch {
      data = { message: text };
    }
  }

  if (!res.ok) {
    const message =
      (data && (data.error || data.message)) ||
      `Request failed with status ${res.status}`;
    throw new ApiError(message, res.status, data);
  }

  return data;
};

export const request = async (
  baseUrl: string,
  path: string,
  { method = 'GET', token, body }: RequestOptions = {}
) => {
  const res = await fetch(`${baseUrl}${path}`, {
    method,
    headers: buildHeaders(token),
    body: body ? JSON.stringify(body) : undefined,
  });

  return handleResponse(res);
};

export const persistToken = (token: string | null) => {
  if (!token) {
    localStorage.removeItem(STORAGE_KEYS.accessToken);
    return;
  }
  localStorage.setItem(STORAGE_KEYS.accessToken, token);
};

export const readToken = (): string | null =>
  localStorage.getItem(STORAGE_KEYS.accessToken);
