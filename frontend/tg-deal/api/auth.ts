import { IDENTITY_API_URL } from './config';
import { request } from './client';
import { Role, User } from '../types';

export interface LoginResult {
  accessToken: string;
  tokenType: string;
  expiresInSeconds: number;
}

export interface MeResponse {
  userId: string;
  email: string;
  role: string;
}

export const login = async (email: string, password: string): Promise<LoginResult> =>
  request(IDENTITY_API_URL, '/api/v1/auth/login', {
    method: 'POST',
    body: { email, password },
  });

export const register = async (
  email: string,
  password: string,
  role: Role
): Promise<{ userId: string; email: string; role: string }> =>
  request(IDENTITY_API_URL, '/api/v1/auth/register', {
    method: 'POST',
    body: { email, password, role },
  });

export const me = async (token: string): Promise<MeResponse> =>
  request(IDENTITY_API_URL, '/api/v1/auth/me', {
    method: 'GET',
    token,
  });

export const mapUserFromMe = (me: MeResponse): User => ({
  id: me.userId,
  email: me.email,
  role:
    me.role === 'Publisher'
      ? Role.Publisher
      : me.role === 'Admin'
        ? Role.Admin
        : Role.Advertiser,
  name: me.email.split('@')[0],
  avatar: `https://api.dicebear.com/8.x/thumbs/svg?seed=${encodeURIComponent(
    me.email
  )}`,
});
