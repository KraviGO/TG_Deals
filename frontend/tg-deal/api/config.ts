const GATEWAY_URL = import.meta.env.VITE_API_GATEWAY_URL || 'http://localhost:5010';
const BOT_USERNAME = import.meta.env.VITE_TELEGRAM_BOT_USERNAME || '@tg_deal_bot';

export const IDENTITY_API_URL = GATEWAY_URL;
export const PAYMENTS_API_URL = GATEWAY_URL;
export const DEALS_API_URL = GATEWAY_URL;
export const CATALOG_API_URL = GATEWAY_URL;
export const PUBLISHERS_API_URL = GATEWAY_URL;
export const TELEGRAM_BOT_USERNAME = BOT_USERNAME.startsWith('@') ? BOT_USERNAME : `@${BOT_USERNAME}`;

export const STORAGE_KEYS = {
  accessToken: 'tgdeal_access_token',
};
