import React, { useEffect, useMemo, useState } from 'react';
import { ArrowUpRight, ArrowRight, RefreshCw } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { User, DealSummary, CatalogChannel, Wallet } from '../../types';
import Card from '../ui/Card';
import Button from '../ui/Button';
import StatsCard from './StatsCard';
import { listMyDeals } from '../../api/deals';
import { listChannels } from '../../api/catalog';
import { getMyWallet } from '../../api/wallet';
import { ApiError } from '../../api/client';
import { dealStatusBadgeClass, dealStatusLabel } from '../../utils/deals';
import { useI18n } from '../../i18n/I18nProvider';

interface AdvertiserDashboardProps {
  user: User;
  token: string;
}

const terminalStatuses = new Set(['Completed', 'Rejected', 'CanceledByAdvertiser', 'Resolved']);

const formatAmount = (value: number) =>
  value.toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

const AdvertiserDashboard: React.FC<AdvertiserDashboardProps> = ({ user, token }) => {
  const { tx } = useI18n();
  const navigate = useNavigate();
  const [deals, setDeals] = useState<DealSummary[]>([]);
  const [wallet, setWallet] = useState<Wallet | null>(null);
  const [recommended, setRecommended] = useState<CatalogChannel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const [dealsRes, walletRes, channelsRes] = await Promise.all([
        listMyDeals(token),
        getMyWallet(token),
        listChannels({ limit: 8, sortBy: 'updatedAt', sortOrder: 'desc' }),
      ]);
      setDeals(dealsRes);
      setWallet(walletRes);
      setRecommended(channelsRes.slice(0, 4));
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось загрузить дашборд', en: 'Failed to load dashboard' }));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, [token]);

  const stats = useMemo(() => {
    const activeDeals = deals.filter((d) => !terminalStatuses.has(d.status)).length;
    const totalSpent = deals
      .filter((d) => d.fundingStatus === 'Captured')
      .reduce((acc, d) => acc + d.amount, 0);
    const reservedNow = deals
      .filter((d) => d.fundingStatus === 'Reserved')
      .reduce((acc, d) => acc + d.amount, 0);

    return {
      activeDeals,
      totalSpent,
      reservedNow,
    };
  }, [deals]);

  const recentDeals = useMemo(() => {
    return [...deals]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 6);
  }, [deals]);

  return (
    <div className="space-y-6">
      {error && (
        <div className="text-sm text-red-400 bg-red-950 border border-red-800 rounded-lg px-3 py-2">
          {error}
        </div>
      )}

      <div className="flex items-center justify-end">
        <Button variant="secondary" className="bg-slate-800 border border-slate-700" onClick={load} disabled={loading}>
          <RefreshCw size={16} className="mr-2" /> {tx({ ru: 'Обновить', en: 'Refresh' })}
        </Button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <StatsCard
          title={tx({ ru: 'Ваш баланс', en: 'Your Balance' })}
          value={`${wallet?.currency ?? 'RUB'} ${formatAmount(wallet?.available ?? (user.balance ?? 0))}`}
          icon={<ArrowUpRight size={80} />}
          subtext={<span className="text-xs text-slate-400">{tx({ ru: 'В резерве', en: 'Reserved' })}: {formatAmount(wallet?.reserved ?? 0)}</span>}
        />

        <StatsCard
          title={tx({ ru: 'Активные сделки', en: 'Active Deals' })}
          value={stats.activeDeals}
          subtext={<span className="text-xs text-slate-400">{tx({ ru: 'Текущий lifecycle сделок', en: 'Live deal lifecycle' })}</span>}
        />

        <StatsCard
          title={tx({ ru: 'Всего списано', en: 'Total Captured Spend' })}
          value={`${wallet?.currency ?? 'RUB'} ${formatAmount(stats.totalSpent)}`}
          subtext={<span className="text-xs text-slate-400">{tx({ ru: 'Сейчас в резерве', en: 'Currently reserved' })}: {formatAmount(stats.reservedNow)}</span>}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2">
          <Card noPadding className="h-full">
            <div className="p-6 flex items-center justify-between border-b border-slate-700">
              <h3 className="text-lg font-semibold">{tx({ ru: 'Последние сделки', en: 'Recent Deals' })}</h3>
              <span className="text-xs text-slate-400">
                {loading ? tx({ ru: 'Загрузка...', en: 'Loading...' }) : `${recentDeals.length} ${tx({ ru: 'строк', en: 'rows' })}`}
              </span>
            </div>
            <div className="divide-y divide-slate-700">
              {recentDeals.length === 0 ? (
                <div className="p-4 text-slate-400 text-sm">{tx({ ru: 'Пока нет сделок', en: 'No deals yet' })}</div>
              ) : (
                recentDeals.map((deal) => (
                  <div key={deal.dealId} className="p-4 flex items-center justify-between hover:bg-slate-750 transition-colors">
                    <div>
                      <p className="font-medium text-white break-all">{deal.dealId}</p>
                      <p className="text-xs text-slate-400 break-all">{tx({ ru: 'Канал', en: 'Channel' })}: {deal.channelId}</p>
                      <p className="text-xs text-slate-500">{tx({ ru: 'Публикация', en: 'Publish at' })}: {new Date(deal.desiredPublishAtUtc).toLocaleString()}</p>
                    </div>
                    <div className="flex items-center gap-4">
                      <span className={`px-3 py-1 rounded-full text-xs font-medium border ${dealStatusBadgeClass(deal.status)}`}>
                        {dealStatusLabel(deal.status)}
                      </span>
                      <p className="font-semibold text-slate-300 w-32 text-right">
                        {deal.amount.toLocaleString('ru-RU')} {deal.currency}
                      </p>
                    </div>
                  </div>
                ))
              )}
            </div>
          </Card>
        </div>

        <div className="lg:col-span-1">
          <Card noPadding className="h-full">
            <div className="p-6 flex items-center justify-between border-b border-slate-700">
              <h3 className="text-lg font-semibold">{tx({ ru: 'Рекомендации', en: 'Recommended' })}</h3>
              <span className="text-xs text-slate-400">{tx({ ru: 'Каталог', en: 'Catalog' })}</span>
            </div>
            <div className="p-4 space-y-4">
              {recommended.length === 0 ? (
                <div className="text-sm text-slate-400">{tx({ ru: 'Каналов нет', en: 'No channels' })}</div>
              ) : (
                recommended.map((channel) => (
                  <button
                    type="button"
                    key={channel.channelId}
                    onClick={() => navigate(`/marketplace?channelId=${channel.channelId}`)}
                    className="w-full text-left flex items-center justify-between p-2 rounded-lg hover:bg-slate-750 transition-colors group"
                  >
                    <div className="flex items-center gap-3">
                      <img src={`https://picsum.photos/seed/${channel.channelId}/80/80`} alt={channel.title} className="w-10 h-10 rounded-full" />
                      <div>
                        <p className="font-medium text-sm group-hover:text-cyan-400 transition-colors">{channel.title}</p>
                        <p className="text-xs text-slate-400">{channel.pricePerPostRub.toLocaleString('ru-RU')} RUB</p>
                      </div>
                    </div>
                    <div className="w-8 h-8 rounded-full bg-slate-700 flex items-center justify-center text-slate-400 group-hover:bg-cyan-500 group-hover:text-white transition-colors">
                      <ArrowRight size={14} />
                    </div>
                  </button>
                ))
              )}
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default AdvertiserDashboard;
