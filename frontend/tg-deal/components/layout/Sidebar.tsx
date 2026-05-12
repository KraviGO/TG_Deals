import React from 'react';
import { NavLink } from 'react-router-dom';
import { 
  LayoutDashboard, Store, FileText, Wallet, Settings, LogOut, Radio, History, Send, Shield 
} from 'lucide-react';
import { Role } from '../../types';
import { useI18n } from '../../i18n/I18nProvider';

interface SidebarProps {
  role: Role;
  isMobileMenuOpen: boolean;
  setIsMobileMenuOpen: (open: boolean) => void;
  onLogout: () => void;
}

const Sidebar: React.FC<SidebarProps> = ({ 
  role,
  isMobileMenuOpen, 
  setIsMobileMenuOpen, 
  onLogout
}) => {
  const { tx } = useI18n();
  const isPublisher = role === Role.Publisher;
  const isAdmin = role === Role.Admin;

  const navItems = isPublisher
    ? [
        { icon: LayoutDashboard, label: tx({ ru: 'Дашборд', en: 'Dashboard' }), path: '/dashboard' },
        { icon: Radio, label: tx({ ru: 'Мои каналы', en: 'My Channels' }), path: '/my-channels' },
        { icon: Send, label: tx({ ru: 'Входящие заявки', en: 'Incoming Apps' }), path: '/applications' },
        { icon: History, label: tx({ ru: 'История выплат', en: 'Deal History' }), path: '/history' },
        { icon: Settings, label: tx({ ru: 'Настройки', en: 'Settings' }), path: '/settings' },
      ]
    : isAdmin
      ? [
          { icon: LayoutDashboard, label: tx({ ru: 'Дашборд', en: 'Dashboard' }), path: '/dashboard' },
          { icon: Store, label: tx({ ru: 'Модерация каналов', en: 'Channel Moderation' }), path: '/marketplace' },
          { icon: FileText, label: tx({ ru: 'Споры', en: 'Disputes' }), path: '/deals' },
          { icon: History, label: tx({ ru: 'Реестр выплат', en: 'Payout Ledger' }), path: '/history' },
          { icon: Settings, label: tx({ ru: 'Настройки', en: 'Settings' }), path: '/settings' },
        ]
      : [
          { icon: LayoutDashboard, label: tx({ ru: 'Дашборд', en: 'Dashboard' }), path: '/dashboard' },
          { icon: Store, label: tx({ ru: 'Маркетплейс', en: 'Marketplace' }), path: '/marketplace' },
          { icon: FileText, label: tx({ ru: 'Мои сделки', en: 'My Deals' }), path: '/deals' },
          { icon: Wallet, label: tx({ ru: 'Мой баланс', en: 'My Balance' }), path: '/balance' },
          { icon: Settings, label: tx({ ru: 'Настройки', en: 'Settings' }), path: '/settings' },
        ];

  return (
    <aside 
      className={`
        fixed inset-y-0 left-0 z-50 w-64 bg-slate-900 border-r border-slate-800 transform transition-transform duration-300 ease-in-out
        ${isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full'}
        md:relative md:translate-x-0 flex flex-col
      `}
    >
      <div className="flex items-center h-16 px-6 border-b border-slate-800">
        <NavLink to="/" className="flex items-center gap-3">
          <div className="w-8 h-8 bg-cyan-500 rounded-lg flex items-center justify-center">
             <span className="font-bold text-white text-lg">TG</span>
          </div>
          <span className="text-xl font-bold tracking-tight text-white">TG Deal</span>
        </NavLink>
      </div>

      <div className="p-4">
        <div className="bg-slate-800 rounded-lg p-3 mb-6 flex items-center gap-3">
           <div className="w-10 h-10 rounded-full bg-cyan-900/50 flex items-center justify-center text-cyan-400">
              {isPublisher ? <Radio size={20} /> : isAdmin ? <Shield size={20} /> : <Store size={20} />}
           </div>
           <div>
             <p className="text-xs text-slate-400 font-medium uppercase tracking-wider">
               {isPublisher
                 ? tx({ ru: 'Панель владельца', en: 'Owner Panel' })
                 : isAdmin
                 ? tx({ ru: 'Панель администратора', en: 'Admin Panel' })
                 : tx({ ru: 'Панель рекламодателя', en: 'Advertiser Panel' })}
             </p>
             <p className="text-xs text-slate-500">
               {tx({ ru: 'Роль определяется вашим аккаунтом', en: 'Role is set by your account' })}
             </p>
           </div>
        </div>

        <nav className="space-y-1">
          {navItems.map((item) => (
            <NavLink
              key={item.path}
              to={item.path}
              onClick={() => setIsMobileMenuOpen(false)}
              className={({ isActive }) => `
                flex items-center px-3 py-2.5 rounded-lg text-sm font-medium transition-colors
                ${isActive 
                  ? 'bg-slate-800 text-cyan-400' 
                  : 'text-slate-400 hover:bg-slate-800/50 hover:text-slate-200'}
              `}
            >
              <item.icon size={20} className="mr-3" />
              {item.label}
            </NavLink>
          ))}
        </nav>
      </div>

      <div className="mt-auto p-4 border-t border-slate-800">
        <button 
          onClick={onLogout}
          className="flex items-center w-full px-3 py-2.5 rounded-lg text-sm font-medium text-slate-400 hover:bg-slate-800/50 hover:text-red-400 transition-colors"
        >
          <LogOut size={20} className="mr-3" />
          {tx({ ru: 'Выйти', en: 'Log out' })}
        </button>
      </div>
    </aside>
  );
};

export default Sidebar;
