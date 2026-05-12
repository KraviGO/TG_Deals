import React, { useEffect, useMemo, useState } from 'react';
import { Filter, RefreshCw } from 'lucide-react';
import { useSearchParams } from 'react-router-dom';
import Button from '../components/ui/Button';
import MarketplaceFilters from '../components/marketplace/MarketplaceFilters';
import ChannelCard from '../components/marketplace/ChannelCard';
import BookingModal from '../components/marketplace/BookingModal';
import { CatalogChannel, Role, User } from '../types';
import { CatalogFilters, getChannel, listChannels } from '../api/catalog';
import { ApiError } from '../api/client';
import { adminBanChannel } from '../api/publishers';
import { useI18n } from '../i18n/I18nProvider';

interface MarketplaceProps {
  user: User;
  token: string;
}

const defaultFilters: CatalogFilters = {
  limit: 50,
  offset: 0,
  sortBy: 'updatedAt',
  sortOrder: 'desc',
};

const mapStatus = (intakeMode: string): 'Active' | 'Paused' | 'Under Review' => {
  if (intakeMode === 'Paused') return 'Paused';
  if (intakeMode === 'ActiveAuto' || intakeMode === 'ActiveManual') return 'Active';
  return 'Under Review';
};

const Marketplace: React.FC<MarketplaceProps> = ({ user, token }) => {
  const { tx } = useI18n();
  const isAdmin = user.role === Role.Admin;
  const [searchParams, setSearchParams] = useSearchParams();
  const [selectedChannel, setSelectedChannel] = useState<CatalogChannel | null>(null);
  const [channels, setChannels] = useState<CatalogChannel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);
  const [actionChannelId, setActionChannelId] = useState<string | null>(null);

  const [searchTerm, setSearchTerm] = useState('');
  const [topic, setTopic] = useState('');
  const [language, setLanguage] = useState('');
  const [intakeMode, setIntakeMode] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [sortBy, setSortBy] = useState<'updatedAt' | 'price' | 'title'>('updatedAt');
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>('desc');

  const [appliedFilters, setAppliedFilters] = useState<CatalogFilters>(defaultFilters);
  const preselectedChannelId = searchParams.get('channelId');

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await listChannels(appliedFilters);
      setChannels(res);
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось загрузить каналы', en: 'Failed to load channels' }));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, [appliedFilters]);

  useEffect(() => {
    if (isAdmin || !preselectedChannelId) return;

    const openChannelFromLink = async () => {
      setError(null);
      try {
        const channel = await getChannel(preselectedChannelId);
        setSelectedChannel(channel);
      } catch (e: unknown) {
        if (e instanceof ApiError) setError(e.message);
        else if (e instanceof Error) setError(e.message);
        else setError(tx({ ru: 'Не удалось открыть канал из рекомендации', en: 'Failed to open channel from recommendation' }));
      } finally {
        setSearchParams((prev) => {
          const next = new URLSearchParams(prev);
          next.delete('channelId');
          return next;
        }, { replace: true });
      }
    };

    openChannelFromLink();
  }, [isAdmin, preselectedChannelId, setSearchParams]);

  const applyFilters = () => {
    const next: CatalogFilters = {
      ...defaultFilters,
      search: searchTerm || undefined,
      topic: topic || undefined,
      language: language || undefined,
      intakeMode: intakeMode || undefined,
      minPricePerPostRub: minPrice ? Number(minPrice) : undefined,
      maxPricePerPostRub: maxPrice ? Number(maxPrice) : undefined,
      sortBy,
      sortOrder,
    };

    setAppliedFilters(next);
  };

  const clearFilters = () => {
    setSearchTerm('');
    setTopic('');
    setLanguage('');
    setIntakeMode('');
    setMinPrice('');
    setMaxPrice('');
    setSortBy('updatedAt');
    setSortOrder('desc');
    setAppliedFilters(defaultFilters);
  };

  const onSortChange = (value: string) => {
    if (value === 'priceAsc') {
      setSortBy('price');
      setSortOrder('asc');
      setAppliedFilters((prev) => ({ ...prev, sortBy: 'price', sortOrder: 'asc' }));
      return;
    }

    if (value === 'priceDesc') {
      setSortBy('price');
      setSortOrder('desc');
      setAppliedFilters((prev) => ({ ...prev, sortBy: 'price', sortOrder: 'desc' }));
      return;
    }

    if (value === 'titleAsc') {
      setSortBy('title');
      setSortOrder('asc');
      setAppliedFilters((prev) => ({ ...prev, sortBy: 'title', sortOrder: 'asc' }));
      return;
    }

    setSortBy('updatedAt');
    setSortOrder('desc');
    setAppliedFilters((prev) => ({ ...prev, sortBy: 'updatedAt', sortOrder: 'desc' }));
  };

  const sortValue =
    sortBy === 'price' && sortOrder === 'asc'
      ? 'priceAsc'
      : sortBy === 'price' && sortOrder === 'desc'
      ? 'priceDesc'
      : sortBy === 'title'
      ? 'titleAsc'
      : 'updatedDesc';

  const handleChannelAction = async (channel: CatalogChannel) => {
    if (!isAdmin) {
      setSelectedChannel(channel);
      return;
    }

    if (channel.ownershipStatus === 'Banned') {
      setInfo(
        tx({ ru: 'Канал', en: 'Channel' }) +
          ` ${channel.title} ` +
          tx({ ru: 'уже заблокирован', en: 'already banned and locked' })
      );
      return;
    }

    setActionChannelId(channel.channelId);
    setError(null);
    setInfo(null);
    try {
      await adminBanChannel(token, channel.channelId);
      setInfo(
        tx({ ru: 'Канал', en: 'Channel' }) +
          ` ${channel.title} ` +
          tx({ ru: 'заблокирован навсегда', en: 'banned and permanently locked' })
      );
      await load();
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось применить модерацию', en: 'Failed to apply moderation' }));
    } finally {
      setActionChannelId(null);
    }
  };

  const summary = useMemo(() => {
    if (loading) return tx({ ru: 'Загрузка каналов...', en: 'Loading channels...' });
    if (isAdmin) return `${tx({ ru: 'Модерация', en: 'Moderating' })} ${channels.length} ${tx({ ru: 'каналов', en: 'channels' })}`;
    return `${tx({ ru: 'Показано', en: 'Showing' })} ${channels.length} ${tx({ ru: 'каналов', en: 'channels' })}`;
  }, [channels.length, isAdmin, loading, tx]);

  return (
    <div className="flex h-full gap-6">
      <MarketplaceFilters
        searchTerm={searchTerm}
        setSearchTerm={setSearchTerm}
        topic={topic}
        setTopic={setTopic}
        language={language}
        setLanguage={setLanguage}
        intakeMode={intakeMode}
        setIntakeMode={setIntakeMode}
        minPrice={minPrice}
        setMinPrice={setMinPrice}
        maxPrice={maxPrice}
        setMaxPrice={setMaxPrice}
        onApply={applyFilters}
        onClear={clearFilters}
      />

      <div className="flex-1">
        <div className="flex items-center justify-between mb-6">
          <div>
            <h2 className="text-xl font-bold">
              {isAdmin ? tx({ ru: 'Модерация каналов', en: 'Channel Moderation' }) : tx({ ru: 'Маркетплейс', en: 'Marketplace' })}
            </h2>
            <p className="text-sm text-slate-400 mt-1">{summary}</p>
          </div>
          <div className="flex items-center gap-2">
            <Button
              variant="secondary"
              size="sm"
              className="bg-slate-800 border border-slate-700"
              onClick={load}
              disabled={loading}
            >
              <RefreshCw size={16} className="mr-2" /> {tx({ ru: 'Обновить', en: 'Refresh' })}
            </Button>
            <Button variant="secondary" size="sm" className="bg-slate-800 border border-slate-700">
              <Filter size={16} />
            </Button>
            <select
              className="bg-slate-800 border border-slate-700 rounded-lg px-3 py-1.5 text-sm text-white focus:outline-none"
              value={sortValue}
              onChange={(e) => onSortChange(e.target.value)}
            >
              <option value="updatedDesc">{tx({ ru: 'Сортировка: последние обновления', en: 'Sort by Last Updated' })}</option>
              <option value="priceAsc">{tx({ ru: 'Сортировка: цена по возрастанию', en: 'Sort by Price: Low to High' })}</option>
              <option value="priceDesc">{tx({ ru: 'Сортировка: цена по убыванию', en: 'Sort by Price: High to Low' })}</option>
              <option value="titleAsc">{tx({ ru: 'Сортировка: по названию', en: 'Sort by Title' })}</option>
            </select>
          </div>
        </div>

        {info && (
          <div className="text-sm text-green-300 bg-green-900/30 border border-green-800 rounded-lg px-3 py-2 mb-4">
            {info}
          </div>
        )}
        {error && (
          <div className="text-sm text-red-400 bg-red-950 border border-red-800 rounded-lg px-3 py-2 mb-4">
            {error}
          </div>
        )}

        <div className="space-y-4">
          {channels.map((channel) => (
            <div key={channel.channelId} className={actionChannelId === channel.channelId ? 'opacity-60 pointer-events-none' : ''}>
              <ChannelCard
                channel={{
                  id: channel.channelId,
                  name: channel.title,
                  description: `@${channel.telegramChannelId} • ${channel.language.toUpperCase()}`,
                  subscribers: 0,
                  pricePerPost: channel.pricePerPostRub,
                  category: channel.topic,
                  image: 'https://picsum.photos/seed/' + channel.channelId + '/200/200',
                  verified: channel.ownershipStatus === 'Verified',
                  avgViews: 0,
                  status: mapStatus(channel.intakeMode),
                  err: 0,
                }}
                onAction={() => handleChannelAction(channel)}
                actionLabel={isAdmin ? tx({ ru: 'Заблокировать канал', en: 'Ban Channel' }) : tx({ ru: 'Оставить заявку', en: 'Book Slot' })}
                actionVariant={isAdmin ? 'secondary' : 'primary'}
              />
            </div>
          ))}
        </div>

        <div className="text-xs text-slate-500 mt-6">
          {isAdmin
            ? tx({
                ru: 'Модерация использует /api/v1/admin/publishers/channels/{channelId}/ban (бан финальный).',
                en: 'Admin moderation uses /api/v1/admin/publishers/channels/{channelId}/ban (ban is final).',
              })
            : tx({
                ru: 'Цены указаны в RUB. Используйте фильтры и кнопку применения для обновления списка.',
                en: 'Prices are in RUB. Use filters and click Apply to reload the list.',
              })}
        </div>
      </div>

      {selectedChannel && !isAdmin && (
        <BookingModal channel={selectedChannel} onClose={() => setSelectedChannel(null)} token={token} user={user} />
      )}
    </div>
  );
};

export default Marketplace;
