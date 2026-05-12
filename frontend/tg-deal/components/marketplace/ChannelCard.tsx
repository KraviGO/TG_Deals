
import React from 'react';
import { CheckCircle, Users, DollarSign, Eye, Gauge } from 'lucide-react';
import { Channel } from '../../types';
import Card from '../ui/Card';
import Button from '../ui/Button';
import { useI18n } from '../../i18n/I18nProvider';

interface ChannelCardProps {
  channel: Channel;
  onAction: (channel: Channel) => void;
  actionLabel?: string;
  actionVariant?: 'primary' | 'secondary' | 'outline' | 'ghost';
}

const ChannelCard: React.FC<ChannelCardProps> = ({
  channel,
  onAction,
  actionLabel = 'Оставить заявку',
  actionVariant = 'primary',
}) => {
  const { tx } = useI18n();
  const statusLabel =
    channel.status === 'Active'
      ? 'Активен'
      : channel.status === 'Paused'
      ? 'Пауза'
      : 'На проверке';
  const statusColor =
    channel.status === 'Active'
      ? 'text-green-400 bg-green-500/10 border-green-500/20'
      : channel.status === 'Paused'
      ? 'text-yellow-400 bg-yellow-500/10 border-yellow-500/20'
      : 'text-slate-400 bg-slate-500/10 border-slate-500/20';

  return (
    <Card noPadding className="hover:border-slate-600 transition-colors">
      <div className="p-5 md:p-6 flex flex-col md:flex-row md:items-center gap-5">
        <div className="flex-1 min-w-0">
          <div className="flex items-start justify-between gap-3">
            <div>
              <h3 className="text-lg font-bold text-white">{channel.name}</h3>
              <p className="text-xs text-cyan-400 uppercase font-semibold tracking-wider mt-1">{channel.category}</p>
              <p className="text-sm text-slate-400 mt-2 truncate">{channel.description}</p>
            </div>
            <span className={`px-2.5 py-1 text-xs border rounded-full ${statusColor}`}>{statusLabel}</span>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-4 gap-3 mt-4">
            <div className="rounded-lg border border-slate-700/70 bg-slate-900/40 px-3 py-2">
              <div className="text-[11px] text-slate-500 uppercase">{tx({ ru: 'Цена', en: 'Price' })}</div>
              <div className="text-sm text-slate-200 flex items-center gap-1 mt-1">
                <DollarSign size={14} className="text-slate-500" />
                {channel.pricePerPost.toLocaleString('ru-RU')} ₽
              </div>
            </div>
            <div className="rounded-lg border border-slate-700/70 bg-slate-900/40 px-3 py-2">
              <div className="text-[11px] text-slate-500 uppercase">{tx({ ru: 'Подписчики', en: 'Subscribers' })}</div>
              <div className="text-sm text-slate-200 flex items-center gap-1 mt-1">
                <Users size={14} className="text-slate-500" />
                {channel.subscribers > 0 ? channel.subscribers.toLocaleString('ru-RU') : '—'}
              </div>
            </div>
            <div className="rounded-lg border border-slate-700/70 bg-slate-900/40 px-3 py-2">
              <div className="text-[11px] text-slate-500 uppercase">{tx({ ru: 'Средние просмотры', en: 'Avg Views' })}</div>
              <div className="text-sm text-slate-200 flex items-center gap-1 mt-1">
                <Eye size={14} className="text-slate-500" />
                {channel.avgViews > 0 ? channel.avgViews.toLocaleString('ru-RU') : tx({ ru: 'ожидание', en: 'pending' })}
              </div>
            </div>
            <div className="rounded-lg border border-slate-700/70 bg-slate-900/40 px-3 py-2">
              <div className="text-[11px] text-slate-500 uppercase">Вовлеченность</div>
              <div className="text-sm text-slate-200 flex items-center gap-1 mt-1">
                <Gauge size={14} className="text-slate-500" />
                {channel.err && channel.err > 0 ? `${channel.err}%` : tx({ ru: 'ожидание', en: 'pending' })}
              </div>
            </div>
          </div>
        </div>

        <div className="md:w-48 flex md:flex-col items-center justify-between gap-3 md:border-l md:border-slate-700/70 md:pl-5">
          <div className="flex items-center gap-2">
            <div className="w-14 h-14 rounded-full overflow-hidden border border-slate-600">
              <img src={channel.image} alt={channel.name} className="w-full h-full object-cover" />
            </div>
            {channel.verified && (
              <div className="text-cyan-400 bg-cyan-400/10 p-1 rounded-full" title={tx({ ru: 'Проверенный канал', en: 'Verified Channel' })}>
                <CheckCircle size={16} />
              </div>
            )}
          </div>
          <Button fullWidth onClick={() => onAction(channel)} variant={actionVariant}>
            {actionLabel}
          </Button>
        </div>
      </div>
    </Card>
  );
};

export default ChannelCard;
