
import React from 'react';
import { User } from '../types';
import Sidebar from './layout/Sidebar';
import Header from './layout/Header';
import { useNotifications } from '../hooks/useNotifications';

interface LayoutProps {
  children: React.ReactNode;
  user: User;
  token: string;
  onLogout: () => void;
}

const Layout: React.FC<LayoutProps> = ({ children, user, token, onLogout }) => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = React.useState(false);
  const { notifications, unreadCount, markAllAsRead, refreshNotifications } = useNotifications(user, token);

  return (
    <div className="flex h-screen bg-slate-900 text-slate-100 font-sans">
      <Sidebar 
        role={user.role}
        isMobileMenuOpen={isMobileMenuOpen} 
        setIsMobileMenuOpen={setIsMobileMenuOpen}
        onLogout={onLogout}
      />

      <div className="flex-1 flex flex-col h-screen overflow-hidden">
        <Header 
          user={user} 
          role={user.role}
          notifications={notifications}
          unreadCount={unreadCount}
          onMarkAllRead={markAllAsRead}
          onRefreshNotifications={refreshNotifications}
          onMobileMenuClick={() => setIsMobileMenuOpen(true)} 
        />

        <main className="flex-1 overflow-y-auto p-4 md:p-8 relative">
           {children}
        </main>
      </div>

      {isMobileMenuOpen && (
        <div 
          className="fixed inset-0 bg-black/50 z-40 md:hidden"
          onClick={() => setIsMobileMenuOpen(false)}
        />
      )}
    </div>
  );
};

export default Layout;
