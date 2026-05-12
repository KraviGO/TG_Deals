import React, { useEffect, useMemo, useState } from 'react';
import { HashRouter, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import Landing from './pages/Landing';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Marketplace from './pages/Marketplace';
import MyDeals from './pages/MyDeals';
import MyBalance from './pages/MyBalance';
import MyChannels from './pages/MyChannels';
import IncomingApplications from './pages/IncomingApplications';
import DealHistory from './pages/DealHistory';
import Settings from './pages/Settings';
import { Role, User, Wallet } from './types';
import { readToken, persistToken } from './api/client';
import { login as loginApi, register as registerApi, me, mapUserFromMe } from './api/auth';
import { getMyWallet } from './api/wallet';
import { useI18n } from './i18n/I18nProvider';

const App: React.FC = () => {
  const { tx } = useI18n();
  const [token, setToken] = useState<string | null>(() => readToken());
  const [user, setUser] = useState<User | null>(null);
  const [wallet, setWallet] = useState<Wallet | null>(null);
  const [loading, setLoading] = useState(true);
  const [authError, setAuthError] = useState<string | null>(null);

  const isAuthenticated = useMemo(() => !!token && !!user, [token, user]);
  const canAccess = (roles: Role[]) => !!user && roles.includes(user.role);

  const loadProfile = async (accessToken: string) => {
    setLoading(true);
    try {
      const profile = await me(accessToken);
      const mappedUser = { ...mapUserFromMe(profile), balance: 0 };
      setUser(mappedUser);

      if (mappedUser.role === Role.Advertiser) {
        const walletRes = await getMyWallet(accessToken);
        setWallet(walletRes);
        setUser((prev) => (prev ? { ...prev, balance: walletRes.available } : prev));
      } else {
        setWallet(null);
      }
      setAuthError(null);
    } catch (err: any) {
      setAuthError(err.message || tx({ ru: 'Не удалось загрузить профиль', en: 'Failed to load profile' }));
      persistToken(null);
      setToken(null);
      setUser(null);
      setWallet(null);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!token) {
      setLoading(false);
      return;
    }
    loadProfile(token);
  }, [token]);

  const handleAuthSuccess = (accessToken: string) => {
    persistToken(accessToken);
    setToken(accessToken);
  };

  const handleLogout = () => {
    persistToken(null);
    setToken(null);
    setUser(null);
    setWallet(null);
  };

  const refreshWallet = async () => {
    if (!token || !user || user.role !== Role.Advertiser) return;
    try {
      const walletRes = await getMyWallet(token);
      setWallet(walletRes);
      setUser((prev) => (prev ? { ...prev, balance: walletRes.available } : prev));
    } catch (err: any) {
      setAuthError(err.message || tx({ ru: 'Не удалось обновить баланс', en: 'Failed to refresh balance' }));
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-slate-900 text-white flex items-center justify-center">
        <div className="text-center space-y-2">
          <div className="w-10 h-10 border-4 border-cyan-500 border-t-transparent rounded-full animate-spin mx-auto" />
          <p className="text-slate-400 text-sm">
            {tx({ ru: 'Загружаем рабочее пространство...', en: 'Loading your workspace...' })}
          </p>
        </div>
      </div>
    );
  }

  return (
    <HashRouter>
      {authError && (
        <div className="bg-red-900 text-red-100 text-sm px-4 py-3 text-center">
          {authError}
        </div>
      )}
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route
          path="/login"
          element={
            <Login
              onAuthSuccess={handleAuthSuccess}
              loginApi={loginApi}
              registerApi={registerApi}
            />
          }
        />

        {/* Закрытые маршруты: внутрь пускаем только пользователя с JWT. */}
        <Route
          path="/dashboard"
          element={
            isAuthenticated && user ? (
              <Layout user={user} token={token!} onLogout={handleLogout}>
                <Dashboard user={user} token={token!} />
              </Layout>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />
        <Route
          path="/marketplace"
          element={
            isAuthenticated && user && canAccess([Role.Advertiser, Role.Admin]) ? (
              <Layout user={user} token={token!} onLogout={handleLogout}>
                <Marketplace user={user} token={token!} />
              </Layout>
            ) : (
              <Navigate to={isAuthenticated ? '/dashboard' : '/login'} replace />
            )
          }
        />
        <Route
          path="/deals"
          element={
            isAuthenticated && user && canAccess([Role.Advertiser, Role.Admin]) ? (
              <Layout user={user} token={token!} onLogout={handleLogout}>
                <MyDeals token={token!} user={user} />
              </Layout>
            ) : (
              <Navigate to={isAuthenticated ? '/dashboard' : '/login'} replace />
            )
          }
        />
        <Route
          path="/balance"
          element={
            isAuthenticated && user && canAccess([Role.Advertiser]) ? (
              <Layout user={user} token={token!} onLogout={handleLogout}>
                <MyBalance
                  user={user}
                  token={token!}
                  wallet={wallet}
                  refreshWallet={refreshWallet}
                />
              </Layout>
            ) : (
              <Navigate to={isAuthenticated ? '/dashboard' : '/login'} replace />
            )
          }
        />
        <Route
          path="/my-channels"
          element={
            isAuthenticated && user && canAccess([Role.Publisher]) ? (
              <Layout user={user} token={token!} onLogout={handleLogout}>
                <MyChannels user={user} token={token!} />
              </Layout>
            ) : (
              <Navigate to={isAuthenticated ? '/dashboard' : '/login'} replace />
            )
          }
        />
        <Route
          path="/applications"
          element={
            isAuthenticated && user && canAccess([Role.Publisher]) ? (
              <Layout user={user} token={token!} onLogout={handleLogout}>
                <IncomingApplications user={user} token={token!} />
              </Layout>
            ) : (
              <Navigate to={isAuthenticated ? '/dashboard' : '/login'} replace />
            )
          }
        />
        <Route
          path="/history"
          element={
            isAuthenticated && user && canAccess([Role.Publisher, Role.Admin]) ? (
              <Layout user={user} token={token!} onLogout={handleLogout}>
                <DealHistory token={token!} user={user} />
              </Layout>
            ) : (
              <Navigate to={isAuthenticated ? '/dashboard' : '/login'} replace />
            )
          }
        />
        <Route
          path="/settings"
          element={
            isAuthenticated && user ? (
              <Layout user={user} token={token!} onLogout={handleLogout}>
                <Settings user={user} />
              </Layout>
            ) : (
              <Navigate to="/login" replace />
            )
          }
        />

        {/* Запасной маршрут для неизвестных адресов SPA. */}
        <Route path="*" element={<Navigate to={isAuthenticated ? '/dashboard' : '/'} replace />} />
      </Routes>
    </HashRouter>
  );
};

export default App;
