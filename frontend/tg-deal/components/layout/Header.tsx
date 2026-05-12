
import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Bell, Menu, RefreshCw } from 'lucide-react';
import { AppNotification, Role, User } from '../../types';
import { useI18n } from '../../i18n/I18nProvider';

interface HeaderProps {
  user: User;
  role: Role;
  notifications: AppNotification[];
  unreadCount: number;
  onMarkAllRead: () => void;
  onRefreshNotifications: () => Promise<void>;
  onMobileMenuClick: () => void;
}

const levelClass = (level: AppNotification['level']) => {
  if (level === 'success') return 'bg-green-500';
  if (level === 'warning') return 'bg-yellow-500';
  return 'bg-cyan-500';
};

const Header: React.FC<HeaderProps> = ({
  user,
  role,
  notifications,
  unreadCount,
  onMarkAllRead,
  onRefreshNotifications,
  onMobileMenuClick,
}) => {
  const { tx } = useI18n();
  const location = useLocation();
  const navigate = useNavigate();
  const isPublisher = role === Role.Publisher;
  const isAdmin = role === Role.Admin;
  const [open, setOpen] = React.useState(false);
  const [refreshing, setRefreshing] = React.useState(false);

  // Заголовок страницы зависит от текущего frontend-маршрута.
  const getPageTitle = (path: string) => {
    if (path.includes('dashboard')) return tx({ ru: 'Дашборд', en: 'Dashboard' });
    if (path.includes('marketplace')) return isAdmin ? tx({ ru: 'Модерация каналов', en: 'Channel Moderation' }) : tx({ ru: 'Маркетплейс', en: 'Marketplace' });
    if (path.includes('deals')) return isAdmin ? tx({ ru: 'Споры', en: 'Disputes' }) : tx({ ru: 'Мои сделки', en: 'My Deals' });
    if (path.includes('balance')) return tx({ ru: 'Мой баланс', en: 'My Balance' });
    if (path.includes('settings')) return tx({ ru: 'Настройки', en: 'Settings' });
    if (path.includes('channels')) return tx({ ru: 'Мои каналы', en: 'My Channels' });
    if (path.includes('applications')) return tx({ ru: 'Входящие заявки', en: 'Incoming Applications' });
    if (path.includes('history')) return tx({ ru: 'История выплат', en: 'Deal History' });
    return tx({ ru: 'Дашборд', en: 'Dashboard' });
  };

  const openNotifications = () => {
    setOpen((prev) => {
      const next = !prev;
      if (!prev && unreadCount > 0) onMarkAllRead();
      return next;
    });
  };

  const handleRefreshNotifications = async () => {
    setRefreshing(true);
    try {
      await onRefreshNotifications();
    } finally {
      setRefreshing(false);
    }
  };

  return (
    <header className="h-16 flex items-center justify-between px-6 border-b border-slate-800 bg-slate-900">
      <button 
        className="md:hidden text-slate-400 hover:text-white"
        onClick={onMobileMenuClick}
      >
        <Menu size={24} />
      </button>

      <h1 className="text-xl font-semibold text-slate-100 hidden md:block">
        {getPageTitle(location.pathname)}
      </h1>

      <div className="flex items-center gap-4">
        <div className="relative">
        <button
          className="relative p-2 text-slate-400 hover:text-white transition-colors rounded-full hover:bg-slate-800"
          onClick={openNotifications}
        >
          <Bell size={20} />
          {unreadCount > 0 && (
            <span className="absolute -top-0.5 -right-0.5 min-w-4 h-4 px-1 bg-red-500 rounded-full border border-slate-900 text-[10px] text-white leading-3 flex items-center justify-center">
              {unreadCount > 9 ? '9+' : unreadCount}
            </span>
          )}
        </button>
          {open && (
            <div className="absolute right-0 mt-2 w-[360px] max-h-[420px] overflow-hidden rounded-xl border border-slate-700 bg-slate-900 shadow-2xl z-50">
              <div className="px-4 py-3 border-b border-slate-700 flex items-center justify-between">
                <div>
                  <h3 className="text-sm font-semibold text-white">{tx({ ru: 'Уведомления', en: 'Notifications' })}</h3>
                  <p className="text-xs text-slate-400">
                    {tx({ ru: 'Новые действия по заказам и спорам', en: 'New actions for deals and disputes' })}
                  </p>
                </div>
                <button
                  onClick={handleRefreshNotifications}
                  className="text-slate-400 hover:text-white transition-colors"
                  disabled={refreshing}
                  title={tx({ ru: 'Обновить', en: 'Refresh' })}
                >
                  <RefreshCw size={15} className={refreshing ? 'animate-spin' : ''} />
                </button>
              </div>

              <div className="overflow-y-auto max-h-[340px] divide-y divide-slate-800">
                {notifications.length === 0 ? (
                  <div className="px-4 py-6 text-sm text-slate-400 text-center">
                    {tx({ ru: 'Пока нет уведомлений', en: 'No notifications yet' })}
                  </div>
                ) : (
                  notifications.map((item) => (
                    <button
                      key={item.id}
                      className="w-full text-left px-4 py-3 hover:bg-slate-800/70 transition-colors"
                      onClick={() => {
                        setOpen(false);
                        navigate(item.route);
                      }}
                    >
                      <div className="flex items-start gap-2">
                        <span className={`mt-1 w-2 h-2 rounded-full ${levelClass(item.level)}`} />
                        <div className="min-w-0">
                          <div className="text-sm text-white">{item.title}</div>
                          <div className="text-xs text-slate-400 mt-1 break-words">{item.body}</div>
                          <div className="text-[11px] text-slate-500 mt-1">
                            {new Date(item.occurredAt).toLocaleString()}
                          </div>
                        </div>
                      </div>
                    </button>
                  ))
                )}
              </div>
            </div>
          )}
        </div>
        
        <div className="flex items-center gap-3 pl-4 border-l border-slate-700">
          <div className="hidden md:block text-right">
            <p className="text-sm font-medium text-white">{user.name}</p>
            <p className="text-xs text-slate-400">
              {isPublisher
                ? tx({ ru: 'Владелец канала', en: 'Channel Owner' })
                : isAdmin
                ? tx({ ru: 'Администратор', en: 'Admin' })
                : tx({ ru: 'Рекламодатель', en: 'Advertiser' })}
            </p>
          </div>
          <img 
            src={user.avatar} 
            alt={user.name} 
            className="w-9 h-9 rounded-full border border-slate-700 object-cover"
          />
        </div>
      </div>
    </header>
  );
};

export default Header;
