import React, { useEffect, useMemo, useState } from 'react';
import { RefreshCw, Search, SlidersHorizontal } from 'lucide-react';
import Button from '../components/ui/Button';
import Card from '../components/ui/Card';
import { PublisherDeal, User, Role } from '../types';
import {
  listPublisherInbox,
  openDealDispute,
  sendPublisherDecision,
} from '../api/deals';
import { ApiError } from '../api/client';
import {
  canOpenDispute,
  dealFundingLabel,
  dealStatusBadgeClass,
  dealStatusLabel,
  fundingStatusBadgeClass,
} from '../utils/deals';
import { useI18n } from '../i18n/I18nProvider';

interface IncomingApplicationsProps {
  user: User;
  token: string;
}

const formatDateTime = (value?: string) => {
  if (!value) return '—';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleString();
};

const IncomingApplications: React.FC<IncomingApplicationsProps> = ({ user, token }) => {
  const { tx } = useI18n();
  const [items, setItems] = useState<PublisherDeal[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'pending' | 'active' | 'closed'>('pending');
  const [search, setSearch] = useState('');
  const [activeDealId, setActiveDealId] = useState<string | null>(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await listPublisherInbox(token);
      setItems(res);
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось загрузить заявки', en: 'Failed to load applications' }));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user.role !== Role.Publisher) {
      setError(tx({ ru: 'Доступно только владельцам каналов', en: 'Available only for channel owners' }));
      setLoading(false);
      return;
    }
    load();
  }, [token, user.role]);

  const counts = useMemo(
    () => ({
      pending: items.filter((d) => d.status === 'PendingPublisherDecision').length,
      active: items.filter((d) => !['PendingPublisherDecision', 'Rejected', 'CanceledByAdvertiser', 'Resolved'].includes(d.status)).length,
      closed: items.filter((d) => ['Rejected', 'CanceledByAdvertiser', 'Resolved'].includes(d.status)).length,
    }),
    [items]
  );

  const filtered = useMemo(() => {
    return items.filter((deal) => {
      const byTab =
        activeTab === 'pending'
          ? deal.status === 'PendingPublisherDecision'
          : activeTab === 'active'
          ? !['PendingPublisherDecision', 'Rejected', 'CanceledByAdvertiser', 'Resolved'].includes(deal.status)
          : ['Rejected', 'CanceledByAdvertiser', 'Resolved'].includes(deal.status);

      const normalized = search.toLowerCase();
      const bySearch =
        !search ||
        deal.dealId.toLowerCase().includes(normalized) ||
        deal.channelId.toLowerCase().includes(normalized) ||
        deal.advertiserUserId.toLowerCase().includes(normalized);

      return byTab && bySearch;
    });
  }, [items, activeTab, search]);

  const performAction = async (dealId: string, action: () => Promise<void>) => {
    setActiveDealId(dealId);
    setError(null);
    setInfo(null);
    try {
      await action();
      await load();
    } finally {
      setActiveDealId(null);
    }
  };

  const handleDecision = async (dealId: string, accept: boolean) => {
    await performAction(dealId, async () => {
      await sendPublisherDecision(token, dealId, accept);
      setInfo(
        accept
          ? tx({
              ru: 'Заявка принята, деньги зарезервированы, пост опубликован в Telegram',
              en: 'Application accepted, funding reserved and post published in Telegram',
            })
          : tx({ ru: 'Заявка отклонена', en: 'Application rejected' })
      );
    }).catch((e: unknown) => {
      if (e instanceof ApiError && e.message === 'TelegramPublishFailed') {
        setError(tx({
          ru: 'Не удалось автоматически опубликовать пост. Проверьте, что бот добавлен админом канала с правами публикации.',
          en: 'Failed to publish automatically. Check that the bot is a channel admin with post permissions.',
        }));
      }
      else if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось обновить статус', en: 'Failed to update status' }));
    });
  };

  const handleOpenDispute = async (dealId: string) => {
    const reason = window.prompt(tx({ ru: 'Причина спора', en: 'Dispute reason' }));
    if (!reason) return;

    await performAction(dealId, async () => {
      await openDealDispute(token, dealId, reason);
      setInfo(tx({ ru: 'Спор открыт', en: 'Dispute opened' }));
    }).catch((e: unknown) => {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось открыть спор', en: 'Failed to open dispute' }));
    });
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between gap-4">
        <div>
          <h2 className="text-2xl font-bold">{tx({ ru: 'Входящие заявки', en: 'Incoming Applications' })}</h2>
          <p className="text-slate-400 text-sm mt-1">
            {loading
              ? tx({ ru: 'Загрузка...', en: 'Loading...' })
              : `${tx({ ru: 'Ожидают', en: 'Pending' })}: ${counts.pending}, ${tx({ ru: 'активные', en: 'active' })}: ${counts.active}, ${tx({ ru: 'закрытые', en: 'closed' })}: ${counts.closed}`}
          </p>
        </div>
        <Button variant="secondary" className="flex items-center" onClick={load} disabled={loading}>
          <RefreshCw size={16} className="mr-2" /> {tx({ ru: 'Обновить', en: 'Refresh' })}
        </Button>
      </div>

      {info && (
        <div className="text-sm text-green-300 bg-green-900/30 border border-green-800 rounded-lg px-3 py-2">
          {info}
        </div>
      )}
      {error && (
        <div className="text-sm text-red-400 bg-red-950 border border-red-800 rounded-lg px-3 py-2">
          {error}
        </div>
      )}

      <Card>
        <div className="flex flex-col md:flex-row gap-3 md:items-center md:justify-between">
          <div className="flex gap-3">
            <button
              className={`px-3 py-1.5 rounded-lg text-sm ${
                activeTab === 'pending'
                  ? 'bg-cyan-500/20 text-cyan-300 border border-cyan-700'
                  : 'bg-slate-800 text-slate-300 border border-slate-700'
              }`}
              onClick={() => setActiveTab('pending')}
            >
              {tx({ ru: 'Ожидают', en: 'Pending' })} ({counts.pending})
            </button>
            <button
              className={`px-3 py-1.5 rounded-lg text-sm ${
                activeTab === 'active'
                  ? 'bg-cyan-500/20 text-cyan-300 border border-cyan-700'
                  : 'bg-slate-800 text-slate-300 border border-slate-700'
              }`}
              onClick={() => setActiveTab('active')}
            >
              {tx({ ru: 'Активные', en: 'Active' })} ({counts.active})
            </button>
            <button
              className={`px-3 py-1.5 rounded-lg text-sm ${
                activeTab === 'closed'
                  ? 'bg-cyan-500/20 text-cyan-300 border border-cyan-700'
                  : 'bg-slate-800 text-slate-300 border border-slate-700'
              }`}
              onClick={() => setActiveTab('closed')}
            >
              {tx({ ru: 'Закрытые', en: 'Closed' })} ({counts.closed})
            </button>
          </div>

          <div className="flex gap-2 items-center">
            <div className="relative">
              <Search className="absolute left-3 top-2.5 text-slate-500" size={16} />
              <input
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder={tx({ ru: 'Поиск сделки/канала/рекламодателя', en: 'Search deal/channel/advertiser' })}
                className="bg-slate-900 border border-slate-700 rounded-lg pl-9 pr-3 py-2 text-sm text-white"
              />
            </div>
            <button className="p-2 hover:bg-slate-800 rounded text-slate-400 hover:text-white">
              <SlidersHorizontal size={18} />
            </button>
          </div>
        </div>
      </Card>

      <Card noPadding>
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead>
              <tr className="bg-slate-800/50 text-xs font-semibold text-slate-400 uppercase">
                <th className="px-6 py-4">{tx({ ru: 'Сделка', en: 'Deal' })}</th>
                <th className="px-6 py-4">Текст поста</th>
                <th className="px-6 py-4">{tx({ ru: 'Расписание', en: 'Schedule' })}</th>
                <th className="px-6 py-4">{tx({ ru: 'Статус', en: 'Status' })}</th>
                <th className="px-6 py-4">Оплата</th>
                <th className="px-6 py-4">{tx({ ru: 'Сумма', en: 'Amount' })}</th>
                <th className="px-6 py-4">Ссылка на пост</th>
                <th className="px-6 py-4 text-right">{tx({ ru: 'Действия', en: 'Actions' })}</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-700 text-sm">
              {loading ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={8}>
                    {tx({ ru: 'Загрузка...', en: 'Loading...' })}
                  </td>
                </tr>
              ) : filtered.length === 0 ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={8}>
                    {tx({ ru: 'Заявок нет', en: 'No applications' })}
                  </td>
                </tr>
              ) : (
                filtered.map((deal) => {
                  const busy = activeDealId === deal.dealId;
                  return (
                    <tr key={deal.dealId} className="hover:bg-slate-750/50 align-top">
                      <td className="px-6 py-4">
                        <div className="font-medium text-white break-all">{deal.dealId}</div>
                        <p className="text-xs text-slate-500 break-all">рекламодатель: {deal.advertiserUserId}</p>
                        <p className="text-xs text-slate-500 break-all">канал: {deal.channelId}</p>
                      </td>
                      <td className="px-6 py-4 text-slate-300">
                        <div className="max-w-md whitespace-pre-wrap break-words rounded-lg border border-slate-700 bg-slate-900/60 px-3 py-2">
                          {deal.postText}
                        </div>
                      </td>
                      <td className="px-6 py-4 text-slate-300">{formatDateTime(deal.desiredPublishAtUtc)}</td>
                      <td className="px-6 py-4">
                        <span
                          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${dealStatusBadgeClass(deal.status)}`}
                        >
                          {dealStatusLabel(deal.status)}
                        </span>
                      </td>
                      <td className="px-6 py-4">
                        <span
                          className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${fundingStatusBadgeClass(deal.fundingStatus)}`}
                        >
                          {dealFundingLabel(deal.fundingStatus)}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-slate-300">
                        {deal.amount.toLocaleString('ru-RU')} {deal.currency}
                      </td>
                      <td className="px-6 py-4 text-slate-300">
                        {deal.postUrl ? (
                          <a href={deal.postUrl} target="_blank" rel="noreferrer" className="text-cyan-400 hover:text-cyan-300">
                            Открыть
                          </a>
                        ) : (
                          '—'
                        )}
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex justify-end gap-2 flex-wrap">
                          {deal.status === 'PendingPublisherDecision' && (
                            <>
                              <Button
                                size="sm"
                                variant="secondary"
                                className="bg-slate-700 hover:bg-slate-600"
                                onClick={() => handleDecision(deal.dealId, false)}
                                disabled={busy}
                              >
                                Отклонить
                              </Button>
                              <Button size="sm" onClick={() => handleDecision(deal.dealId, true)} disabled={busy}>
                                {busy ? 'Публикуем...' : 'Принять и опубликовать'}
                              </Button>
                            </>
                          )}
                          {canOpenDispute(deal.status, deal.fundingStatus) && (
                            <Button
                              size="sm"
                              variant="secondary"
                              className="bg-slate-700 hover:bg-slate-600"
                              onClick={() => handleOpenDispute(deal.dealId)}
                              disabled={busy}
                            >
                              Открыть спор
                            </Button>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
};

export default IncomingApplications;
