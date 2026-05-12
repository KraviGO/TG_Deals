import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Send, Radio, ShieldCheck, Shield } from 'lucide-react';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import { Role } from '../types';
import { ApiError } from '../api/client';
import { LoginResult } from '../api/auth';
import { useI18n } from '../i18n/I18nProvider';

interface LoginProps {
  onAuthSuccess: (accessToken: string) => void;
  loginApi: (email: string, password: string) => Promise<LoginResult>;
  registerApi: (email: string, password: string, role: Role) => Promise<any>;
}

const Login: React.FC<LoginProps> = ({ onAuthSuccess, loginApi, registerApi }) => {
  const { tx } = useI18n();
  const navigate = useNavigate();
  const [mode, setMode] = useState<'login' | 'register'>('login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [role, setRole] = useState<Role>(Role.Advertiser);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      if (mode === 'register') {
        await registerApi(email, password, role);
      }

      const res = await loginApi(email, password);
      onAuthSuccess(res.accessToken);
      navigate('/dashboard');
    } catch (err: unknown) {
      if (err instanceof ApiError) {
        setError(err.message);
      } else if (err instanceof Error) {
        setError(err.message);
      } else {
        setError(tx({ ru: 'Что-то пошло не так', en: 'Something went wrong' }));
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-900 flex flex-col items-center justify-center p-4">
      <div className="flex items-center gap-2 mb-8 animate-fade-in-down">
        <div className="w-10 h-10 bg-cyan-500 rounded-lg flex items-center justify-center">
          <Send className="text-slate-900 -ml-1 mt-1 transform -rotate-12" size={24} />
        </div>
        <span className="text-2xl font-bold text-white tracking-tight">TG Deal</span>
      </div>

      <Card className="w-full max-w-lg bg-slate-800 border border-slate-700">
        <div className="flex justify-between items-center mb-6">
          <div>
            <h1 className="text-2xl font-bold text-white">
              {mode === 'login' ? tx({ ru: 'Вход', en: 'Sign in' }) : tx({ ru: 'Регистрация', en: 'Create account' })}
            </h1>
            <p className="text-slate-400 text-sm">
              {tx({ ru: 'Используйте данные вашего аккаунта', en: 'Use your marketplace credentials' })}
            </p>
          </div>
          <div className="flex gap-2">
            <button
              className={`px-3 py-1.5 rounded-md text-sm font-medium ${mode === 'login' ? 'bg-cyan-600 text-white' : 'text-slate-400 hover:text-white'}`}
              onClick={() => setMode('login')}
              type="button"
            >
              {tx({ ru: 'Войти', en: 'Login' })}
            </button>
            <button
              className={`px-3 py-1.5 rounded-md text-sm font-medium ${mode === 'register' ? 'bg-cyan-600 text-white' : 'text-slate-400 hover:text-white'}`}
              onClick={() => setMode('register')}
              type="button"
            >
              {tx({ ru: 'Регистрация', en: 'Register' })}
            </button>
          </div>
        </div>

        <form className="space-y-4" onSubmit={handleSubmit}>
          <div>
            <label className="block text-sm font-medium text-slate-300 mb-2">{tx({ ru: 'Email', en: 'Email' })}</label>
            <input
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white focus:outline-none focus:border-cyan-500"
              placeholder={tx({ ru: 'you@example.com', en: 'you@example.com' })}
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-slate-300 mb-2">{tx({ ru: 'Пароль', en: 'Password' })}</label>
            <input
              type="password"
              required
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="w-full bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white focus:outline-none focus:border-cyan-500"
              placeholder="••••••••"
            />
          </div>

          {mode === 'register' && (
            <div>
              <label className="block text-sm font-medium text-slate-300 mb-2">{tx({ ru: 'Роль', en: 'Role' })}</label>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                <button
                  type="button"
                  onClick={() => setRole(Role.Advertiser)}
                  className={`flex items-center gap-2 border rounded-lg px-3 py-2 text-left ${
                    role === Role.Advertiser
                      ? 'border-cyan-500 bg-cyan-500/10 text-white'
                      : 'border-slate-700 bg-slate-900 text-slate-300 hover:border-slate-600'
                  }`}
                >
                  <Send size={18} />
                  <div>
                    <p className="text-sm font-semibold">{tx({ ru: 'Рекламодатель', en: 'Advertiser' })}</p>
                    <p className="text-xs text-slate-400">{tx({ ru: 'Покупает размещения', en: 'Buy placements' })}</p>
                  </div>
                </button>
                <button
                  type="button"
                  onClick={() => setRole(Role.Publisher)}
                  className={`flex items-center gap-2 border rounded-lg px-3 py-2 text-left ${
                    role === Role.Publisher
                      ? 'border-cyan-500 bg-cyan-500/10 text-white'
                      : 'border-slate-700 bg-slate-900 text-slate-300 hover:border-slate-600'
                  }`}
                >
                  <Radio size={18} />
                  <div>
                    <p className="text-sm font-semibold">{tx({ ru: 'Владелец канала', en: 'Channel Owner' })}</p>
                    <p className="text-xs text-slate-400">{tx({ ru: 'Принимает сделки', en: 'Accept deals' })}</p>
                  </div>
                </button>
                <button
                  type="button"
                  onClick={() => setRole(Role.Admin)}
                  className={`flex items-center gap-2 border rounded-lg px-3 py-2 text-left ${
                    role === Role.Admin
                      ? 'border-cyan-500 bg-cyan-500/10 text-white'
                      : 'border-slate-700 bg-slate-900 text-slate-300 hover:border-slate-600'
                  }`}
                >
                  <Shield size={18} />
                  <div>
                    <p className="text-sm font-semibold">{tx({ ru: 'Админ', en: 'Admin' })}</p>
                    <p className="text-xs text-slate-400">{tx({ ru: 'Модерация', en: 'Moderation' })}</p>
                  </div>
                </button>
              </div>
            </div>
          )}

          {error && <div className="text-sm text-red-400 bg-red-950 border border-red-800 rounded-lg px-3 py-2">{error}</div>}

          <Button type="submit" fullWidth disabled={submitting}>
            {submitting
              ? tx({ ru: 'Подождите…', en: 'Please wait…' })
              : mode === 'login'
              ? tx({ ru: 'Войти', en: 'Sign in' })
              : tx({ ru: 'Зарегистрироваться и войти', en: 'Register & Sign in' })}
          </Button>
        </form>

        <div className="mt-6 p-3 rounded-lg bg-slate-900/60 border border-slate-800 text-sm text-slate-400 flex items-center gap-2">
          <ShieldCheck size={16} className="text-cyan-400" />
          {tx({
            ru: 'JWT выдаются напрямую backend-сервисом Identity.',
            en: 'We issue JWTs directly from the backend Identity service.',
          })}
        </div>
      </Card>
    </div>
  );
};

export default Login;
