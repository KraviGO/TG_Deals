import React, { useEffect, useMemo, useState } from 'react';
import { Search, RefreshCw, ExternalLink } from 'lucide-react';
import Button from '../components/ui/Button';
import Card from '../components/ui/Card';
import { ApiError } from '../api/client';
import {
  advertiserConfirmDeal,
  cancelDeal,
  listAdminDisputes,
  listMyDeals,
  openDealDispute,
  resolveDealDispute,
} from '../api/deals';
import { getChannel } from '../api/catalog';
import { AdminDealDispute, CatalogChannel, DealSummary, Role, User } from '../types';
import {
  canCancelDeal,
  canOpenDispute,
  dealFundingLabel,
  dealStatusBadgeClass,
  dealStatusLabel,
  fundingStatusBadgeClass,
} from '../utils/deals';
import { useI18n } from '../i18n/I18nProvider';

interface MyDealsProps {
  token: string;
  user: User;
}

const formatDateTime = (value?: string) => {
  if (!value) return '—';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleString();
};

const AdminResolveView: React.FC<{ token: string }> = ({ token }) => {
  const { tx } = useI18n();
  const [disputes, setDisputes] = useState<AdminDealDispute[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState<'all' | 'open' | 'resolved'>('all');
  const [search, setSearch] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);
  const [activeActionKey, setActiveActionKey] = useState<string | null>(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await listAdminDisputes(token, statusFilter, 300);
      setDisputes(res);
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось загрузить споры', en: 'Failed to load disputes' }));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, [token, statusFilter]);

  const filteredDisputes = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return disputes;

    return disputes.filter((item) => {
      return (
        item.disputeId.toLowerCase().includes(q) ||
        item.dealId.toLowerCase().includes(q) ||
        item.reason.toLowerCase().includes(q) ||
        item.disputeStatus.toLowerCase().includes(q) ||
        item.openedByRole.toLowerCase().includes(q)
      );
    });
  }, [disputes, search]);

  const openCount = useMemo(
    () => disputes.filter((x) => x.disputeStatus === 'Open').length,
    [disputes]
  );

  const resolveCount = useMemo(
    () => disputes.filter((x) => x.disputeStatus === 'Resolved').length,
    [disputes]
  );

  const resolveItem = async (item: AdminDealDispute, action: 'capture' | 'release') => {
    const note = window.prompt(tx({ ru: 'Комментарий решения (optional)', en: 'Resolution note (optional)' })) || undefined;
    const actionKey = `${item.disputeId}:${action}`;
    setActiveActionKey(actionKey);
    setError(null);
    setInfo(null);
    try {
      await resolveDealDispute(token, item.dealId, action, note);
      setInfo(`${tx({ ru: 'Спор', en: 'Dispute' })} ${item.disputeId} ${tx({ ru: 'успешно решен', en: 'resolved successfully' })}`);
      await load();
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось разрешить спор', en: 'Failed to resolve dispute' }));
    } finally {
      setActiveActionKey(null);
    }
  };

  const disputeBadgeClass = (status: string) => {
    if (status === 'Open') return 'bg-yellow-500/10 text-yellow-400 border-yellow-500/20';
    if (status === 'Resolved') return 'bg-green-500/10 text-green-400 border-green-500/20';
    return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
  };

  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold">{tx({ ru: 'Споры', en: 'Disputes' })}</h2>
        <p className="text-slate-400 text-sm mt-1">
          {tx({ ru: 'Все споры и интерфейс для решения: списать оплату или вернуть резерв.', en: 'All disputes and resolve interface (capture/release).' })}
        </p>
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

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card>
          <div className="text-sm text-slate-400">{tx({ ru: 'Открытые', en: 'Open' })}</div>
          <div className="text-2xl font-bold text-yellow-400 mt-1">{openCount}</div>
        </Card>
        <Card>
          <div className="text-sm text-slate-400">{tx({ ru: 'Решенные', en: 'Resolved' })}</div>
          <div className="text-2xl font-bold text-green-400 mt-1">{resolveCount}</div>
        </Card>
        <Card>
          <div className="text-sm text-slate-400">{tx({ ru: 'Всего', en: 'Total' })}</div>
          <div className="text-2xl font-bold text-white mt-1">{disputes.length}</div>
        </Card>
      </div>

      <Card>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
          <div className="relative">
            <Search className="absolute left-3 top-2.5 text-slate-500" size={18} />
            <input
              type="text"
              placeholder={tx({ ru: 'Поиск по сделке/спору/причине', en: 'Search by deal/dispute/reason' })}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg pl-10 pr-4 py-2 text-sm text-white"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>

          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value as 'all' | 'open' | 'resolved')}
            className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-sm text-white"
          >
            <option value="all">{tx({ ru: 'Все споры', en: 'All disputes' })}</option>
            <option value="open">{tx({ ru: 'Только открытые', en: 'Open only' })}</option>
            <option value="resolved">{tx({ ru: 'Только решенные', en: 'Resolved only' })}</option>
          </select>

          <div className="flex justify-end">
            <Button variant="secondary" onClick={load} disabled={loading}>
              <RefreshCw size={16} className="mr-2" /> {tx({ ru: 'Обновить', en: 'Refresh' })}
            </Button>
          </div>
        </div>
      </Card>

      <Card noPadding>
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead>
              <tr className="bg-slate-800/50 text-xs font-semibold text-slate-400 uppercase">
                <th className="px-6 py-4">Спор</th>
                <th className="px-6 py-4">Сделка</th>
                <th className="px-6 py-4">Открыт</th>
                <th className="px-6 py-4">Причина</th>
                <th className="px-6 py-4">Статус оплаты</th>
                <th className="px-6 py-4">Решение</th>
                <th className="px-6 py-4 text-right">Действия</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-700 text-sm">
              {loading ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={7}>
                    {tx({ ru: 'Загрузка...', en: 'Loading...' })}
                  </td>
                </tr>
              ) : filteredDisputes.length === 0 ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={7}>
                    {tx({ ru: 'Споры не найдены', en: 'Disputes not found' })}
                  </td>
                </tr>
              ) : (
                filteredDisputes.map((item) => (
                  <tr key={item.disputeId} className="hover:bg-slate-750/50 align-top">
                    <td className="px-6 py-4">
                      <div className="break-all text-slate-300">{item.disputeId}</div>
                      <div
                        className={`inline-flex mt-2 px-2.5 py-0.5 rounded-full text-xs font-medium border ${disputeBadgeClass(
                          item.disputeStatus
                        )}`}
                      >
                        {item.disputeStatus === 'Open' ? 'Открыт' : item.disputeStatus === 'Resolved' ? 'Решен' : item.disputeStatus}
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="break-all text-white font-medium">{item.dealId}</div>
                      <div className="text-xs text-slate-500 mt-1 break-all">канал: {item.channelId}</div>
                    </td>
                    <td className="px-6 py-4 text-slate-300">
                      <div>{formatDateTime(item.disputeCreatedAt)}</div>
                      <div className="text-xs text-slate-500 mt-1">
                        открыл: {item.openedByRole} ({item.openedByUserId})
                      </div>
                    </td>
                    <td className="px-6 py-4 text-slate-300">
                      <div className="max-w-sm whitespace-pre-wrap">{item.reason}</div>
                    </td>
                    <td className="px-6 py-4 text-slate-300">
                      <div>{dealStatusLabel(item.dealStatus)}</div>
                      <div className="text-xs text-slate-500 mt-1">
                        {dealFundingLabel(item.fundingStatus)} • {item.amount.toLocaleString('ru-RU')} {item.currency}
                      </div>
                      {item.postUrl && (
                        <a
                          href={item.postUrl}
                          target="_blank"
                          rel="noreferrer"
                          className="inline-flex items-center gap-1 text-cyan-400 hover:text-cyan-300 text-xs mt-1"
                        >
                          {tx({ ru: 'Пост', en: 'Post' })} <ExternalLink size={12} />
                        </a>
                      )}
                    </td>
                    <td className="px-6 py-4 text-slate-300">
                      {item.resolutionAction ? (
                        <>
                          <div className="font-medium text-slate-100">
                            {item.resolutionAction === 'capture' ? 'Оплата списана' : item.resolutionAction === 'release' ? 'Резерв возвращен' : item.resolutionAction}
                          </div>
                          <div className="text-xs text-slate-500 mt-1">{formatDateTime(item.resolvedAt)}</div>
                          {item.resolutionNote && (
                            <div className="text-xs text-slate-400 mt-1 max-w-xs whitespace-pre-wrap">
                              {item.resolutionNote}
                            </div>
                          )}
                        </>
                      ) : (
                        '—'
                      )}
                    </td>
                    <td className="px-6 py-4">
                      {item.disputeStatus === 'Open' ? (
                        <div className="flex justify-end gap-2">
                          <Button
                            size="sm"
                            variant="secondary"
                            onClick={() => resolveItem(item, 'release')}
                            disabled={activeActionKey !== null}
                          >
                            {activeActionKey === `${item.disputeId}:release` ? '...' : tx({ ru: 'Освободить', en: 'Release' })}
                          </Button>
                          <Button
                            size="sm"
                            onClick={() => resolveItem(item, 'capture')}
                            disabled={activeActionKey !== null}
                          >
                            {activeActionKey === `${item.disputeId}:capture` ? '...' : tx({ ru: 'Списать', en: 'Capture' })}
                          </Button>
                        </div>
                      ) : (
                        <div className="text-right text-slate-500 text-xs">{tx({ ru: 'Решен', en: 'Resolved' })}</div>
                      )}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
};

const MyDeals: React.FC<MyDealsProps> = ({ token, user }) => {
  const { tx } = useI18n();
  const [deals, setDeals] = useState<DealSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);
  const [activeDealId, setActiveDealId] = useState<string | null>(null);

  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('all');
  const [channelPreview, setChannelPreview] = useState<CatalogChannel | null>(null);

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await listMyDeals(token);
      setDeals(res);
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось загрузить сделки', en: 'Failed to load deals' }));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (user.role === Role.Admin) return;
    load();
  }, [token, user.role]);

  const uniqueStatuses = useMemo(() => {
    return Array.from(new Set(deals.map((d) => d.status))).sort();
  }, [deals]);

  const filteredDeals = useMemo(() => {
    return deals.filter((deal) => {
      const byStatus = statusFilter === 'all' || deal.status === statusFilter;
      const bySearch =
        !searchTerm ||
        deal.channelId.toLowerCase().includes(searchTerm.toLowerCase()) ||
        deal.dealId.toLowerCase().includes(searchTerm.toLowerCase());
      return byStatus && bySearch;
    });
  }, [deals, searchTerm, statusFilter]);

  const withBusyState = async (dealId: string, action: () => Promise<void>) => {
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

  const handleCancel = async (dealId: string) => {
    await withBusyState(dealId, async () => {
      await cancelDeal(token, dealId);
      setInfo(tx({ ru: 'Сделка отменена, резерв освобожден', en: 'Deal canceled, reservation released' }));
    }).catch((e: unknown) => {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось отменить сделку', en: 'Failed to cancel deal' }));
    });
  };

  const handleDispute = async (dealId: string) => {
    const reason = window.prompt(tx({ ru: 'Причина открытия спора', en: 'Dispute reason' }));
    if (!reason) return;

    await withBusyState(dealId, async () => {
      const res = await openDealDispute(token, dealId, reason);
      setInfo(`Спор открыт: ${res.status} (${res.disputeId})`);
    }).catch((e: unknown) => {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось открыть спор', en: 'Failed to open dispute' }));
    });
  };

  const handleAdvertiserConfirm = async (dealId: string) => {
    await withBusyState(dealId, async () => {
      await advertiserConfirmDeal(token, dealId);
      setInfo(tx({ ru: 'Подтверждение принято, оплата списана (captured)', en: 'Confirmation received, payment captured' }));
    }).catch((e: unknown) => {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось подтвердить выполнение сделки', en: 'Failed to confirm delivery' }));
    });
  };

  const handleChannelPreview = async (channelId: string) => {
    setError(null);
    setInfo(null);
    try {
      const channel = await getChannel(channelId);
      setChannelPreview(channel);
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось загрузить канал', en: 'Failed to load channel' }));
    }
  };

  if (user.role === Role.Admin) {
    return <AdminResolveView token={token} />;
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4">
        <div>
          <h2 className="text-2xl font-bold">{tx({ ru: 'Мои сделки', en: 'My Deals' })}</h2>
          <p className="text-slate-400 text-sm mt-1">
            {tx({ ru: 'Жизненный цикл сделки, оплата, отмена и споры.', en: 'Lifecycle, funding, cancel and dispute actions.' })}
          </p>
        </div>
        <Button variant="secondary" onClick={load} disabled={loading} className="bg-slate-800 border border-slate-700">
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
        <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
          <div className="relative">
            <Search className="absolute left-3 top-2.5 text-slate-500" size={18} />
            <input
              type="text"
              placeholder={tx({ ru: 'Поиск по каналу или сделке', en: 'Search by channelId or dealId' })}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg pl-10 pr-4 py-2 text-sm text-white"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>

          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-sm text-white"
          >
            <option value="all">{tx({ ru: 'Все статусы', en: 'All statuses' })}</option>
            {uniqueStatuses.map((status) => (
              <option key={status} value={status}>
                {dealStatusLabel(status)}
              </option>
            ))}
          </select>

          <div className="text-xs text-slate-400 flex items-center">
            {tx({ ru: 'Сделка завершается после подтверждения рекламодателя.', en: 'Deal is completed after advertiser confirmation (capture).' })}
          </div>
        </div>
      </Card>

      <Card noPadding>
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead>
              <tr className="border-b border-slate-700 text-xs font-semibold text-slate-400 uppercase tracking-wider bg-slate-800">
                <th className="px-6 py-4">Сделка</th>
                <th className="px-6 py-4">Статус</th>
                <th className="px-6 py-4">Оплата</th>
                <th className="px-6 py-4">Дата публикации</th>
                <th className="px-6 py-4">Сумма</th>
                <th className="px-6 py-4">Пост</th>
                <th className="px-6 py-4 text-right">Действия</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-700 text-sm">
              {loading ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={7}>
                    {tx({ ru: 'Загрузка...', en: 'Loading...' })}
                  </td>
                </tr>
              ) : filteredDeals.length === 0 ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={7}>
                    {tx({ ru: 'Сделки не найдены', en: 'No deals found' })}
                  </td>
                </tr>
              ) : (
                filteredDeals.map((deal) => {
                  const busy = activeDealId === deal.dealId;
                  return (
                    <tr key={deal.dealId} className="hover:bg-slate-750/50 align-top">
                      <td className="px-6 py-4">
                        <div className="font-medium text-white break-all">{deal.dealId}</div>
                        <button
                          className="text-xs text-cyan-400 hover:text-cyan-300 mt-1"
                          onClick={() => handleChannelPreview(deal.channelId)}
                        >
                          Канал: {deal.channelId}
                        </button>
                      </td>
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
                        {deal.reservationId && (
                          <div className="text-[11px] text-slate-500 mt-1 break-all">{deal.reservationId}</div>
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-300">{formatDateTime(deal.desiredPublishAtUtc)}</td>
                      <td className="px-6 py-4 text-slate-300">
                        {deal.amount.toLocaleString('ru-RU')} {deal.currency}
                      </td>
                      <td className="px-6 py-4 text-slate-300">
                        {deal.postUrl ? (
                          <a
                            href={deal.postUrl}
                            target="_blank"
                            rel="noreferrer"
                            className="inline-flex items-center gap-1 text-cyan-400 hover:text-cyan-300"
                          >
                            {tx({ ru: 'Открыть', en: 'Open' })} <ExternalLink size={12} />
                          </a>
                        ) : (
                          '—'
                        )}
                        {deal.publishedAtUtc && (
                          <div className="text-[11px] text-slate-500 mt-1">{formatDateTime(deal.publishedAtUtc)}</div>
                        )}
                      </td>
                      <td className="px-6 py-4">
                        <div className="flex justify-end flex-wrap gap-2">
                          {canCancelDeal(deal.status) && (
                            <Button
                              size="sm"
                              variant="secondary"
                              className="bg-slate-700 hover:bg-slate-600"
                              onClick={() => handleCancel(deal.dealId)}
                              disabled={busy}
                            >
                              {tx({ ru: 'Отменить', en: 'Cancel' })}
                            </Button>
                          )}
                          {canOpenDispute(deal.status, deal.fundingStatus) && (
                            <Button
                              size="sm"
                              variant="secondary"
                              className="bg-slate-700 hover:bg-slate-600"
                              onClick={() => handleDispute(deal.dealId)}
                              disabled={busy}
                            >
                              {tx({ ru: 'Открыть спор', en: 'Open Dispute' })}
                            </Button>
                          )}
                          {deal.status === 'PublishedPendingConfirm' &&
                            deal.fundingStatus === 'Reserved' && (
                              <Button
                                size="sm"
                                onClick={() => handleAdvertiserConfirm(deal.dealId)}
                                disabled={busy}
                              >
                                {tx({ ru: 'Подтвердить выполнение', en: 'Confirm Delivery' })}
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

      {channelPreview && (
        <Card>
          <div className="flex items-start justify-between gap-4">
            <div>
              <h3 className="text-lg font-semibold text-white">{tx({ ru: 'Превью канала', en: 'Channel Preview' })}</h3>
              <p className="text-sm text-slate-400">{tx({ ru: 'Данные из каталога по идентификатору канала', en: 'Fetched from catalog by channelId' })}</p>
            </div>
            <Button size="sm" variant="ghost" onClick={() => setChannelPreview(null)}>
              {tx({ ru: 'Закрыть', en: 'Close' })}
            </Button>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-3 mt-4 text-sm">
            <div className="text-slate-300">Название: {channelPreview.title}</div>
            <div className="text-slate-300">Telegram: @{channelPreview.telegramChannelId}</div>
            <div className="text-slate-300">Тематика: {channelPreview.topic}</div>
            <div className="text-slate-300">Язык: {channelPreview.language}</div>
            <div className="text-slate-300">Цена: {channelPreview.pricePerPostRub.toLocaleString('ru-RU')} ₽</div>
            <div className="text-slate-300">Режим заявок: {channelPreview.intakeMode}</div>
            <div className="text-slate-300">Статус владения: {channelPreview.ownershipStatus}</div>
            <div className="text-slate-300">Идентификатор канала: {channelPreview.channelId}</div>
          </div>
        </Card>
      )}
    </div>
  );
};

export default MyDeals;
