import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Plus, RefreshCw } from 'lucide-react';
import Card from '../ui/Card';
import Button from '../ui/Button';
import StatsCard from './StatsCard';
import { PublisherChannel, PublisherDeal } from '../../types';
import { getMyChannels } from '../../api/publishers';
import { listPublisherInbox } from '../../api/deals';
import { getMyPublisherWallet } from '../../api/wallet';
import { ApiError } from '../../api/client';
import { useI18n } from '../../i18n/I18nProvider';

interface OwnerDashboardProps {
  token: string;
}

const OwnerDashboard: React.FC<OwnerDashboardProps> = ({ token }) => {
  const { tx } = useI18n();
  const navigate = useNavigate();
  const [channels, setChannels] = useState<PublisherChannel[]>([]);
  const [inbox, setInbox] = useState<PublisherDeal[]>([]);
  const [pendingPayout, setPendingPayout] = useState(0);
  const [currency, setCurrency] = useState('RUB');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const [channelsRes, inboxRes, walletRes] = await Promise.all([
        getMyChannels(token),
        listPublisherInbox(token),
        getMyPublisherWallet(token),
      ]);
      setChannels(channelsRes);
      setInbox(inboxRes);
      setPendingPayout(walletRes.available);
      setCurrency(walletRes.currency);
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

  const pendingApplications = useMemo(
    () => inbox.filter((d) => d.status === 'PendingPublisherDecision').length,
    [inbox]
  );

  const activeChannels = useMemo(
    () => channels.filter((ch) => ch.intakeMode !== 'Paused').length,
    [channels]
  );

  return (
    <div className="space-y-6">
      {error && (
        <div className="text-sm text-red-400 bg-red-950 border border-red-800 rounded-lg px-3 py-2">
          {error}
        </div>
      )}

      <div className="flex items-center justify-between mb-2">
        <h2 className="text-2xl font-bold">{tx({ ru: 'Кабинет владельца канала', en: 'Publisher Workspace' })}</h2>
        <div className="flex gap-2">
          <Button variant="secondary" className="bg-slate-800 border border-slate-700" onClick={load} disabled={loading}>
            <RefreshCw size={16} className="mr-2" /> {tx({ ru: 'Обновить', en: 'Refresh' })}
          </Button>
          <Button icon={Plus} onClick={() => navigate('/my-channels')}>
            {tx({ ru: 'Добавить канал', en: 'Add New Channel' })}
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <StatsCard
          title={tx({ ru: 'Всего каналов', en: 'Total Channels' })}
          value={channels.length}
          subtext={<p className="text-slate-400 text-sm">{tx({ ru: 'Активных', en: 'Active' })}: {activeChannels}</p>}
        />

        <StatsCard
          title={tx({ ru: 'Новые заявки', en: 'New Applications' })}
          value={pendingApplications}
          subtext={<p className="text-slate-400 text-sm">{tx({ ru: 'Ожидают решения', en: 'Awaiting decision' })}</p>}
          className="text-cyan-400"
        />

        <StatsCard
          title={tx({ ru: 'Доступно к выводу', en: 'Ready To Withdraw' })}
          value={`${currency} ${pendingPayout.toLocaleString('ru-RU', { maximumFractionDigits: 2 })}`}
          subtext={<p className="text-slate-400 text-sm">{tx({ ru: 'Доступный баланс', en: 'Available balance' })}</p>}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2">
          <Card noPadding className="h-full">
            <div className="p-6 border-b border-slate-700 flex items-center justify-between">
              <h3 className="text-lg font-semibold">{tx({ ru: 'Мои каналы', en: 'My Channels' })}</h3>
              <span className="text-xs text-slate-400">
                {loading ? tx({ ru: 'Загрузка...', en: 'Loading...' }) : `${channels.length} ${tx({ ru: 'всего', en: 'total' })}`}
              </span>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-left">
                <thead>
                  <tr className="border-b border-slate-700 text-xs font-semibold text-slate-400 uppercase tracking-wider">
                    <th className="px-6 py-4">{tx({ ru: 'Канал', en: 'Channel' })}</th>
                    <th className="px-6 py-4">{tx({ ru: 'Тематика', en: 'Topic' })}</th>
                    <th className="px-6 py-4">{tx({ ru: 'Цена', en: 'Price' })}</th>
                    <th className="px-6 py-4">{tx({ ru: 'Режим заявок', en: 'Intake' })}</th>
                    <th className="px-6 py-4">{tx({ ru: 'Статус', en: 'Ownership' })}</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-700 text-sm">
                  {channels.length === 0 ? (
                    <tr>
                      <td className="px-6 py-4 text-slate-400" colSpan={5}>
                        {tx({ ru: 'Каналов пока нет', en: 'No channels yet' })}
                      </td>
                    </tr>
                  ) : (
                    channels.map((channel) => (
                      <tr key={channel.channelId} className="hover:bg-slate-750/50">
                        <td className="px-6 py-4">
                          <div className="font-medium text-white">{channel.title}</div>
                          <div className="text-xs text-slate-500">@{channel.telegramChannelId}</div>
                        </td>
                        <td className="px-6 py-4 text-slate-300">{channel.topic}</td>
                        <td className="px-6 py-4 text-slate-300">{channel.pricePerPostRub.toLocaleString('ru-RU')} RUB</td>
                        <td className="px-6 py-4 text-slate-300">{channel.intakeMode}</td>
                        <td className="px-6 py-4 text-slate-300">{channel.ownershipStatus}</td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </Card>
        </div>

        <div className="lg:col-span-1">
          <Card className="h-full">
            <h3 className="text-lg font-semibold mb-4">{tx({ ru: 'Последние заявки', en: 'Recent Applications' })}</h3>
            <div className="space-y-3">
              {inbox.length === 0 ? (
                <div className="text-sm text-slate-400">{tx({ ru: 'Во входящих нет сделок', en: 'No deals in inbox' })}</div>
              ) : (
                inbox.slice(0, 8).map((deal) => (
                  <div key={deal.dealId} className="p-3 rounded-lg border border-slate-700 bg-slate-900/40">
                    <p className="text-xs text-slate-500 break-all">deal: {deal.dealId}</p>
                    <p className="text-sm text-white mt-1">{deal.status}</p>
                    <p className="text-xs text-slate-400 mt-1">
                      {deal.amount.toLocaleString('ru-RU')} {deal.currency} • {deal.fundingStatus}
                    </p>
                  </div>
                ))
              )}
            </div>
          </Card>
        </div>
      </div>
    </div>
  );
};

export default OwnerDashboard;
