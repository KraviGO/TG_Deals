import React, { useEffect, useMemo, useState } from 'react';
import { Download, RefreshCw, ExternalLink, Lock } from 'lucide-react';
import { User, Wallet } from '../types';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import {
  createTopUp,
  listMyTopUps,
  listMyWalletTransactions,
  TopUpHistoryRecord,
  WalletTransactionRecord,
  withdrawFromWallet,
} from '../api/wallet';
import { ApiError } from '../api/client';
import { useI18n } from '../i18n/I18nProvider';

interface MyBalanceProps {
  user: User;
  wallet: Wallet | null;
  token: string;
  refreshWallet: () => Promise<void> | void;
}

const formatAmount = (value?: number) =>
  typeof value === 'number'
    ? value.toLocaleString(undefined, { minimumFractionDigits: 2, maximumFractionDigits: 2 })
    : '0.00';

const formatDateTime = (value?: string) => {
  if (!value) return '—';
  const d = new Date(value);
  if (Number.isNaN(d.getTime())) return value;
  return d.toLocaleString();
};

const MyBalance: React.FC<MyBalanceProps> = ({ user, wallet, token, refreshWallet }) => {
  const { tx } = useI18n();
  const [topUpAmount, setTopUpAmount] = useState<number>(100);
  const [withdrawAmount, setWithdrawAmount] = useState<number>(100);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isWithdrawing, setIsWithdrawing] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  const [topUps, setTopUps] = useState<TopUpHistoryRecord[]>([]);
  const [transactions, setTransactions] = useState<WalletTransactionRecord[]>([]);
  const [historyLoading, setHistoryLoading] = useState(true);

  const currency = wallet?.currency ?? 'RUB';
  const balance = useMemo(() => wallet?.available ?? 0, [wallet]);

  const loadHistory = async () => {
    setHistoryLoading(true);
    try {
      const [topUpsRes, txRes] = await Promise.all([
        listMyTopUps(token, 50),
        listMyWalletTransactions(token, 100),
      ]);
      setTopUps(topUpsRes);
      setTransactions(txRes);
    } catch (err: unknown) {
      if (err instanceof ApiError) setError(err.message);
      else if (err instanceof Error) setError(err.message);
      else setError(tx({ ru: 'Не удалось загрузить историю операций', en: 'Failed to load operations history' }));
    } finally {
      setHistoryLoading(false);
    }
  };

  useEffect(() => {
    refreshWallet();
    loadHistory();
  }, []);

  const handleTopUp = async () => {
    if (!topUpAmount || topUpAmount <= 0) {
      setError(tx({ ru: 'Введите сумму больше 0', en: 'Enter amount greater than 0' }));
      return;
    }

    setIsSubmitting(true);
    setError(null);
    setMessage(null);

    const paymentWindow = window.open('about:blank', '_blank', 'noopener,noreferrer');

    try {
      const res = await createTopUp(token, topUpAmount, currency);
      setMessage(tx({ ru: 'Переходим к платежу в YooKassa…', en: 'Redirecting to YooKassa payment…' }));
      if (paymentWindow) {
        paymentWindow.location.href = res.confirmationUrl;
      } else {
        const win = window.open(res.confirmationUrl, '_blank', 'noopener,noreferrer');
        if (!win) {
          window.location.href = res.confirmationUrl; // Последний fallback, если браузер заблокировал новое окно.
        }
      }
      await loadHistory();
    } catch (err: unknown) {
      if (err instanceof ApiError) setError(err.message);
      else if (err instanceof Error) setError(err.message);
      else setError(tx({ ru: 'Не удалось создать пополнение', en: 'Failed to create top-up' }));
      if (paymentWindow) paymentWindow.close();
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleWithdraw = async () => {
    if (!withdrawAmount || withdrawAmount <= 0) {
      setError(tx({ ru: 'Введите сумму вывода больше 0', en: 'Enter withdrawal amount greater than 0' }));
      return;
    }

    setIsWithdrawing(true);
    setError(null);
    setMessage(null);

    try {
      const res = await withdrawFromWallet(token, withdrawAmount, currency);
      setMessage(
        `${tx({ ru: 'Вывод выполнен', en: 'Withdrawal completed' })}: ${formatAmount(withdrawAmount)} ${currency}. ` +
          `${tx({ ru: 'Доступно', en: 'Available' })}: ${formatAmount(res.available)}`
      );
      await refreshWallet();
      await loadHistory();
    } catch (err: unknown) {
      if (err instanceof ApiError) setError(err.message);
      else if (err instanceof Error) setError(err.message);
      else setError(tx({ ru: 'Не удалось выполнить вывод', en: 'Failed to withdraw' }));
    } finally {
      setIsWithdrawing(false);
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between gap-3">
        <div>
          <h2 className="text-2xl font-bold">{tx({ ru: 'Мой баланс', en: 'My Balance' })}</h2>
          <p className="text-slate-400 text-sm">
            {tx({ ru: 'Все суммы в валюте кошелька', en: 'All amounts are shown in wallet currency' })} ({currency})
          </p>
          <p className="text-slate-500 text-xs mt-1">
            {tx({ ru: 'Вы вошли как', en: 'Signed in as' })} {user.email}
          </p>
        </div>
        <div className="flex gap-2">
          <Button
            variant="outline"
            className="flex items-center bg-slate-800 text-slate-300"
            onClick={async () => {
              await refreshWallet();
              await loadHistory();
            }}
          >
            <RefreshCw size={16} className="mr-2" /> {tx({ ru: 'Обновить', en: 'Refresh' })}
          </Button>
          <Button variant="outline" className="flex items-center bg-slate-800 text-slate-300" disabled>
            <Download size={16} className="mr-2" /> {tx({ ru: 'Экспорт', en: 'Export' })}
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-1 space-y-6">
          <Card>
            <p className="text-slate-400 font-medium mb-2">{tx({ ru: 'Текущий баланс', en: 'Current Balance' })}</p>
            <h3 className="text-4xl font-bold text-white mb-2">
              {currency} {formatAmount(balance)}
            </h3>
            <p className="text-sm text-slate-500">{tx({ ru: 'В резерве', en: 'Reserved' })}: {formatAmount(wallet?.reserved)}</p>
            <p className="text-sm text-slate-500">{tx({ ru: 'Итого', en: 'Total' })}: {formatAmount(wallet?.total)}</p>
          </Card>

          <Card>
            <h3 className="text-lg font-bold text-white mb-4">{tx({ ru: 'Пополнение', en: 'Top Up' })}</h3>
            <div className="mb-4">
              <label className="text-sm text-slate-400 block mb-2">{tx({ ru: 'Сумма', en: 'Amount' })}</label>
              <div className="relative">
                <span className="absolute left-3 top-2.5 text-slate-400">{currency}</span>
                <input
                  type="number"
                  min="1"
                  value={topUpAmount}
                  onChange={(e) => setTopUpAmount(Number(e.target.value))}
                  className="w-full bg-slate-900 border border-slate-700 rounded-lg pl-14 pr-4 py-2 text-white"
                />
              </div>
            </div>
            <Button fullWidth icon={Lock} onClick={handleTopUp} disabled={isSubmitting}>
              {isSubmitting ? tx({ ru: 'Обработка…', en: 'Processing…' }) : tx({ ru: 'Перейти к оплате', en: 'Proceed to Payment' })}
            </Button>
          </Card>

          <Card>
            <h3 className="text-lg font-bold text-white mb-4">{tx({ ru: 'Вывод средств', en: 'Withdraw Funds' })}</h3>
            <div className="mb-4">
              <label className="text-sm text-slate-400 block mb-2">{tx({ ru: 'Сумма', en: 'Amount' })}</label>
              <div className="relative">
                <span className="absolute left-3 top-2.5 text-slate-400">{currency}</span>
                <input
                  type="number"
                  min="1"
                  value={withdrawAmount}
                  onChange={(e) => setWithdrawAmount(Number(e.target.value))}
                  className="w-full bg-slate-900 border border-slate-700 rounded-lg pl-14 pr-4 py-2 text-white"
                />
              </div>
            </div>
            <Button fullWidth variant="secondary" onClick={handleWithdraw} disabled={isWithdrawing}>
              {isWithdrawing ? tx({ ru: 'Вывод…', en: 'Withdrawing…' }) : tx({ ru: 'Вывести', en: 'Withdraw' })}
            </Button>
          </Card>
        </div>

        <div className="lg:col-span-2 space-y-6">
          {error && <div className="text-sm text-red-400 bg-red-950 border border-red-800 rounded-lg px-3 py-2">{error}</div>}
          {message && (
            <div className="text-sm text-cyan-300 bg-cyan-900/40 border border-cyan-800 rounded-lg px-3 py-2 flex items-center gap-2">
              <ExternalLink size={14} /> {message}
            </div>
          )}

          <Card noPadding>
            <div className="p-6 border-b border-slate-700">
              <h3 className="text-lg font-bold">{tx({ ru: 'История пополнений', en: 'Top-Up History' })}</h3>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-left">
                <thead>
                  <tr className="bg-slate-800/50 text-xs font-semibold text-slate-400 uppercase">
                    <th className="px-6 py-4">Идентификатор пополнения</th>
                    <th className="px-6 py-4">Идентификатор ЮKassa</th>
                    <th className="px-6 py-4">{tx({ ru: 'Статус', en: 'Status' })}</th>
                    <th className="px-6 py-4 text-right">{tx({ ru: 'Сумма', en: 'Amount' })}</th>
                    <th className="px-6 py-4">{tx({ ru: 'Создано', en: 'Created' })}</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-700 text-sm">
                  {historyLoading ? (
                    <tr><td className="px-6 py-4 text-slate-400" colSpan={5}>{tx({ ru: 'Загрузка...', en: 'Loading...' })}</td></tr>
                  ) : topUps.length === 0 ? (
                    <tr><td className="px-6 py-4 text-slate-400" colSpan={5}>{tx({ ru: 'Пополнений пока нет', en: 'No top-ups yet' })}</td></tr>
                  ) : (
                    topUps.map((t) => (
                      <tr key={t.topUpId} className="hover:bg-slate-750/50">
                        <td className="px-6 py-4 text-slate-300 break-all">{t.topUpId}</td>
                        <td className="px-6 py-4 text-slate-300 break-all">{t.yooKassaPaymentId}</td>
                        <td className="px-6 py-4 text-slate-300">{t.status}</td>
                        <td className="px-6 py-4 text-right text-slate-100 font-medium">{formatAmount(t.amount)} {t.currency}</td>
                        <td className="px-6 py-4 text-slate-300">{formatDateTime(t.createdAt)}</td>
                      </tr>
                    ))
                  )}
                </tbody>
              </table>
            </div>
          </Card>

          <Card noPadding>
            <div className="p-6 border-b border-slate-700">
              <h3 className="text-lg font-bold">{tx({ ru: 'Транзакции кошелька', en: 'Wallet Transactions' })}</h3>
            </div>
            <div className="overflow-x-auto">
              <table className="w-full text-left">
                <thead>
                  <tr className="bg-slate-800/50 text-xs font-semibold text-slate-400 uppercase">
                    <th className="px-6 py-4">{tx({ ru: 'Дата', en: 'Date' })}</th>
                    <th className="px-6 py-4">{tx({ ru: 'Тип', en: 'Type' })}</th>
                    <th className="px-6 py-4">{tx({ ru: 'Связано', en: 'Related' })}</th>
                    <th className="px-6 py-4 text-right">{tx({ ru: 'Сумма', en: 'Amount' })}</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-700 text-sm">
                  {historyLoading ? (
                    <tr><td className="px-6 py-4 text-slate-400" colSpan={4}>{tx({ ru: 'Загрузка...', en: 'Loading...' })}</td></tr>
                  ) : transactions.length === 0 ? (
                    <tr><td className="px-6 py-4 text-slate-400" colSpan={4}>{tx({ ru: 'Транзакций пока нет', en: 'No transactions yet' })}</td></tr>
                  ) : (
                    transactions.map((tx) => {
                      const positiveTypes = ['TopUpSucceeded', 'ReserveReleased', 'ManualCredit'];
                      const positive = positiveTypes.includes(tx.type);
                      return (
                        <tr key={tx.txId} className="hover:bg-slate-750/50">
                          <td className="px-6 py-4 text-slate-300">{formatDateTime(tx.createdAt)}</td>
                          <td className="px-6 py-4 text-slate-300">{tx.type}</td>
                          <td className="px-6 py-4 text-slate-300 break-all">{tx.dealId || tx.topUpId || '—'}</td>
                          <td className={`px-6 py-4 text-right font-medium ${positive ? 'text-green-400' : 'text-red-400'}`}>
                            {positive ? '+' : '-'}{formatAmount(tx.amount)} {tx.currency}
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
      </div>
    </div>
  );
};

export default MyBalance;
