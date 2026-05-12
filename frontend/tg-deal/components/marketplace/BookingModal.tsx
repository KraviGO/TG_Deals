import React, { useMemo, useState } from 'react';
import Card from '../ui/Card';
import Button from '../ui/Button';
import { CatalogChannel, Role, User } from '../../types';
import { createDeal } from '../../api/deals';
import { ApiError } from '../../api/client';
import { useI18n } from '../../i18n/I18nProvider';

interface BookingModalProps {
  channel: CatalogChannel;
  onClose: () => void;
  token: string;
  user: User;
}

const toLocalDateTimeValue = (date: Date) => {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  return `${year}-${month}-${day}T${hours}:${minutes}`;
};

const BookingModal: React.FC<BookingModalProps> = ({ channel, onClose, token, user }) => {
  const { tx } = useI18n();
  const defaultDate = useMemo(() => {
    const d = new Date();
    d.setHours(d.getHours() + 24);
    return toLocalDateTimeValue(d);
  }, []);

  const [adText, setAdText] = useState('Check out our new product!');
  const [desiredPublishAtLocal, setDesiredPublishAtLocal] = useState(defaultDate);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const handleSubmit = async () => {
    if (user.role !== Role.Advertiser) {
      setError(tx({ ru: 'Только рекламодатель может создавать сделки', en: 'Only advertisers can create deals' }));
      return;
    }

    if (!desiredPublishAtLocal) {
      setError(tx({ ru: 'Укажите дату и время публикации', en: 'Provide desired publish date and time' }));
      return;
    }

    setSubmitting(true);
    setError(null);
    setSuccess(null);

    try {
      const desiredPublishAtUtc = new Date(desiredPublishAtLocal).toISOString();
      const res = await createDeal(token, {
        channelId: channel.channelId,
        postText: adText,
        desiredPublishAtUtc,
        amount: channel.pricePerPostRub,
        currency: 'RUB',
      });
      setSuccess(
        tx({ ru: 'Сделка создана', en: 'Deal created' }) +
          `: ${res.dealId}. ${tx({ ru: 'Статус', en: 'Status' })}: ${res.status}, funding: ${res.fundingStatus}`
      );
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось создать сделку', en: 'Failed to create deal' }));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/70 backdrop-blur-sm">
      <Card className="w-full max-w-lg bg-slate-800 border-slate-700 shadow-2xl animate-fade-in" noPadding>
        <div className="p-6 border-b border-slate-700 flex justify-between items-center">
          <div>
            <h3 className="text-xl font-bold text-white">Book @ {channel.telegramChannelId}</h3>
            <p className="text-xs text-slate-400">
              {channel.title} • {channel.topic} • {channel.pricePerPostRub.toLocaleString('ru-RU')} RUB
            </p>
          </div>
          <button onClick={onClose} className="text-slate-400 hover:text-white">
            <span className="text-2xl">&times;</span>
          </button>
        </div>

        <div className="p-6 space-y-5">
          {error && (
            <div className="text-sm text-red-400 bg-red-950 border border-red-800 rounded-lg px-3 py-2">
              {error}
            </div>
          )}
          {success && (
            <div className="text-sm text-green-300 bg-green-900/40 border border-green-800 rounded-lg px-3 py-2">
              {success}
            </div>
          )}

          <div>
            <label className="text-sm font-medium text-slate-300 mb-2 block">
              {tx({ ru: 'Желаемое время публикации', en: 'Desired publish time' })}
            </label>
            <input
              type="datetime-local"
              value={desiredPublishAtLocal}
              onChange={(e) => setDesiredPublishAtLocal(e.target.value)}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-sm text-white"
            />
          </div>

          <div>
            <label className="text-sm font-medium text-slate-300 mb-2 block">
              {tx({ ru: 'Текст поста', en: 'Post text' })}
            </label>
            <textarea
              className="w-full bg-slate-900 border border-slate-700 rounded-lg p-3 text-sm text-white resize-none"
              rows={5}
              maxLength={1000}
              value={adText}
              onChange={(e) => setAdText(e.target.value)}
            />
            <div className="flex justify-end text-xs text-slate-500 mt-1">{adText.length}/1000</div>
          </div>

          <div className="bg-cyan-900/20 border border-cyan-900/50 p-3 rounded-lg text-xs text-cyan-100/90 leading-relaxed">
            {tx({
              ru: 'Средства резервируются через кошелек и списываются только после подтверждения публикации.',
              en: 'Funds are reserved through wallet funding and captured only after publisher confirms the post.',
            })}
          </div>
        </div>

        <div className="p-6 border-t border-slate-700 flex justify-end gap-3 bg-slate-800">
          <Button variant="ghost" onClick={onClose}>
            {tx({ ru: 'Закрыть', en: 'Close' })}
          </Button>
          <Button onClick={handleSubmit} disabled={submitting}>
            {submitting ? tx({ ru: 'Отправка…', en: 'Submitting…' }) : tx({ ru: 'Создать сделку', en: 'Create Deal' })}
          </Button>
        </div>
      </Card>
    </div>
  );
};

export default BookingModal;
