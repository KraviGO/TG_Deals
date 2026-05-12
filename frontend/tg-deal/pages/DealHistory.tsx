import React, { useEffect, useMemo, useState } from 'react';
import { Download, RefreshCw } from 'lucide-react';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import { PublisherLedgerEntry, PublisherWalletSummary, Role, User } from '../types';
import { ApiError } from '../api/client';
import {
  getMyPublisherWallet,
  listMyPublisherWalletEntries,
  withdrawPublisherBalance,
} from '../api/wallet';
import { useI18n } from '../i18n/I18nProvider';

interface DealHistoryProps {
  token: string;
  user: User;
}

const formatAmount = (value?: number) =>
  typeof value === 'number'
    ? value.toLocaleString('ru-RU', { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    : '0.00';

const formatDate = (value?: string) => {
  if (!value) return '—';
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;
  return date.toLocaleString();
};

const statusBadge = (status: string) => {
  if (status === 'Accrued') return 'bg-cyan-500/10 text-cyan-400 border-cyan-500/20';
  return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
};

const ledgerStatusLabel = (status: string) => {
  switch (status) {
    case 'Accrued':
      return 'Начислено';
    case 'PaidOut':
      return 'Выведено';
    default:
      return status;
  }
};

const DealHistory: React.FC<DealHistoryProps> = ({ token, user }) => {
  const { tx } = useI18n();
  const [wallet, setWallet] = useState<PublisherWalletSummary | null>(null);
  const [entries, setEntries] = useState<PublisherLedgerEntry[]>([]);
  const [loading, setLoading] = useState(true);
  const [withdrawing, setWithdrawing] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);
  const [withdrawAmount, setWithdrawAmount] = useState<string>('');
  const [cardNumber, setCardNumber] = useState('');

  const load = async () => {
    setLoading(true);
    setError(null);
    try {
      const [walletRes, entriesRes] = await Promise.all([
        getMyPublisherWallet(token),
        listMyPublisherWalletEntries(token),
      ]);
      setWallet(walletRes);
      setEntries(entriesRes);
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось загрузить реестр выплат', en: 'Failed to load payout ledger' }));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, [token]);

  const currency = wallet?.currency || 'RUB';
  const totals = useMemo(
    () => ({
      available: wallet?.available ?? 0,
      paidOut: wallet?.paidOut ?? 0,
      totalAccrued: wallet?.totalAccrued ?? 0,
    }),
    [wallet]
  );

  const withdrawableTotal = totals.available;

  const handleWithdraw = async () => {
    const amount = Number(withdrawAmount);
    if (!Number.isFinite(amount) || amount <= 0) {
      setError('Введите корректную сумму вывода');
      return;
    }

    if (amount > withdrawableTotal) {
      setError('Сумма больше доступного к выводу остатка');
      return;
    }

    const digits = cardNumber.replace(/\D/g, '');
    if (digits.length < 12 || digits.length > 19) {
      setError('Введите корректный номер карты');
      return;
    }

    setWithdrawing(true);
    setError(null);
    setInfo(null);
    try {
      const res = await withdrawPublisherBalance(token, amount, digits);
      setInfo(
        `Запрошено: ${formatAmount(res.requestedAmount)} ${res.currency}. ` +
          `Выведено: ${formatAmount(res.withdrawnAmount)} ${res.currency}` +
          `${res.destinationCardMask ? ` на карту ${res.destinationCardMask}` : ''}.`
      );
      setWithdrawAmount('');
      setCardNumber('');
      await load();
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось выполнить вывод', en: 'Failed to withdraw' }));
    } finally {
      setWithdrawing(false);
    }
  };

  if (user.role !== Role.Publisher && user.role !== Role.Admin) {
    return (
      <Card>
        <p className="text-slate-300">{tx({ ru: 'Страница доступна владельцу канала или администратору.', en: 'This page is available for Publisher/Admin only.' })}</p>
      </Card>
    );
  }

  const canWithdraw = user.role === Role.Publisher;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold">{tx({ ru: 'Реестр выплат', en: 'Payout Ledger' })}</h2>
          <p className="text-slate-400 text-sm mt-1">
            {tx({ ru: 'Начисления по сделкам и общий доступный баланс для вывода', en: 'Deal accruals and total available payout balance' })}
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="secondary" className="bg-slate-800 border border-slate-700" onClick={load}>
            <RefreshCw size={16} className="mr-2" /> {tx({ ru: 'Обновить', en: 'Refresh' })}
          </Button>
          <Button variant="outline" className="bg-slate-800 text-slate-200" disabled>
            <Download size={16} className="mr-2" /> {tx({ ru: 'Экспорт CSV', en: 'Export CSV' })}
          </Button>
        </div>
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

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card className="flex flex-col justify-between">
          <p className="text-slate-400 font-medium text-sm">Доступно к выводу</p>
          <h3 className="text-2xl font-bold text-cyan-300 mt-2">
            {currency} {formatAmount(totals.available)}
          </h3>
        </Card>
        <Card className="flex flex-col justify-between">
          <p className="text-slate-400 font-medium text-sm">Уже выведено</p>
          <h3 className="text-2xl font-bold text-green-300 mt-2">
            {currency} {formatAmount(totals.paidOut)}
          </h3>
        </Card>
        <Card className="flex flex-col justify-between">
          <p className="text-slate-400 font-medium text-sm">Всего начислено</p>
          <h3 className="text-2xl font-bold text-white mt-2">
            {currency} {formatAmount(totals.totalAccrued)}
          </h3>
        </Card>
      </div>

      {canWithdraw && (
        <Card>
          <div className="flex flex-col gap-4">
            <div>
              <h3 className="text-lg font-semibold text-white">{tx({ ru: 'Вывод на карту', en: 'Withdraw To Card' })}</h3>
              <p className="text-sm text-slate-400">
                {tx({ ru: 'Доступно к выводу', en: 'Available for payout' })}: {formatAmount(withdrawableTotal)} {currency}
              </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
              <input
                type="number"
                min={1}
                step="0.01"
                value={withdrawAmount}
                onChange={(e) => setWithdrawAmount(e.target.value)}
                placeholder={tx({ ru: `Сумма (${currency})`, en: `Amount (${currency})` })}
                className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white"
              />
              <input
                value={cardNumber}
                onChange={(e) => setCardNumber(e.target.value)}
                placeholder={tx({ ru: 'Номер карты', en: 'Card number' })}
                className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white"
              />
              <div className="flex justify-end">
                <Button onClick={handleWithdraw} disabled={withdrawing}>
                  {withdrawing ? tx({ ru: 'Вывод...', en: 'Withdrawing...' }) : tx({ ru: 'Вывести', en: 'Withdraw' })}
                </Button>
              </div>
            </div>
          </div>
        </Card>
      )}

      <Card noPadding>
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead>
              <tr className="bg-slate-800/50 text-xs font-semibold text-slate-400 uppercase">
                <th className="px-6 py-4">Запись</th>
                <th className="px-6 py-4">{tx({ ru: 'Сделка', en: 'Deal' })}</th>
                <th className="px-6 py-4">Сумма сделки</th>
                <th className="px-6 py-4">Комиссия</th>
                <th className="px-6 py-4">Доход владельца</th>
                <th className="px-6 py-4">{tx({ ru: 'Статус', en: 'Status' })}</th>
                <th className="px-6 py-4">{tx({ ru: 'Создано', en: 'Created' })}</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-700 text-sm">
              {loading ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={7}>
                    {tx({ ru: 'Загрузка...', en: 'Loading...' })}
                  </td>
                </tr>
              ) : entries.length === 0 ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={7}>
                    {tx({ ru: 'Начислений пока нет', en: 'No accrual entries yet' })}
                  </td>
                </tr>
              ) : (
                entries.map((entry) => {
                  return (
                    <tr key={entry.entryId} className="hover:bg-slate-750/50 align-top">
                      <td className="px-6 py-4 text-slate-300 break-all">{entry.entryId}</td>
                      <td className="px-6 py-4 text-slate-300 break-all">{entry.dealId}</td>
                      <td className="px-6 py-4 text-slate-300">{formatAmount(entry.grossAmount)}</td>
                      <td className="px-6 py-4 text-slate-300">{formatAmount(entry.platformFeeAmount)}</td>
                      <td className="px-6 py-4 text-slate-100 font-medium">{formatAmount(entry.publisherAmount)}</td>
                      <td className="px-6 py-4">
                        <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium border ${statusBadge(entry.status)}`}>
                          {ledgerStatusLabel(entry.status)}
                        </span>
                        {entry.availableAt && (
                          <div className="text-[11px] text-slate-500 mt-1">
                            доступно: {formatDate(entry.availableAt)}
                          </div>
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-300">{formatDate(entry.createdAt)}</td>
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

export default DealHistory;
