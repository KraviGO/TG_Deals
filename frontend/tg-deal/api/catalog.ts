import { CATALOG_API_URL } from './config';
import { request } from './client';
import { CatalogChannel } from '../types';

export interface CatalogFilters {
  limit?: number;
  offset?: number;
  search?: string;
  topic?: string;
  language?: string;
  intakeMode?: string;
  minPricePerPostRub?: number;
  maxPricePerPostRub?: number;
  sortBy?: 'updatedAt' | 'price' | 'title';
  sortOrder?: 'asc' | 'desc';
}

export const listChannels = async (filters: CatalogFilters = {}): Promise<CatalogChannel[]> => {
  const params = new URLSearchParams();

  if (typeof filters.limit === 'number') params.set('limit', String(filters.limit));
  if (typeof filters.offset === 'number') params.set('offset', String(filters.offset));
  if (filters.search) params.set('search', filters.search);
  if (filters.topic) params.set('topic', filters.topic);
  if (filters.language) params.set('language', filters.language);
  if (filters.intakeMode) params.set('intakeMode', filters.intakeMode);
  if (typeof filters.minPricePerPostRub === 'number') params.set('minPricePerPostRub', String(filters.minPricePerPostRub));
  if (typeof filters.maxPricePerPostRub === 'number') params.set('maxPricePerPostRub', String(filters.maxPricePerPostRub));
  if (filters.sortBy) params.set('sortBy', filters.sortBy);
  if (filters.sortOrder) params.set('sortOrder', filters.sortOrder);

  const query = params.toString();
  const path = query ? `/api/v1/catalog/channels?${query}` : '/api/v1/catalog/channels';

  return request(CATALOG_API_URL, path);
};

export const getChannel = async (channelId: string): Promise<CatalogChannel> =>
  request(CATALOG_API_URL, `/api/v1/catalog/channels/${channelId}`);
