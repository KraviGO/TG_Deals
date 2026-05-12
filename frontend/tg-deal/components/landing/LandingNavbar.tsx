import React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '../ui/Button';
import { useI18n } from '../../i18n/I18nProvider';

const LandingNavbar: React.FC = () => {
  const { tx } = useI18n();
  const navigate = useNavigate();
  return (
    <nav className="border-b border-slate-800 bg-slate-900/80 backdrop-blur-md sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
        <div className="flex items-center gap-2 cursor-pointer" onClick={() => navigate('/')}>
          <div className="w-8 h-8 bg-cyan-500 rounded flex items-center justify-center font-bold text-slate-900">TG</div>
          <span className="font-bold text-xl tracking-tight text-white">TG Deal</span>
        </div>
        <div className="hidden md:flex items-center gap-8 text-sm font-medium text-slate-400">
          <a href="#marketplace" className="hover:text-cyan-400 transition-colors">
            {tx({ ru: 'Маркетплейс', en: 'Marketplace' })}
          </a>
          <a href="#how-it-works" className="hover:text-cyan-400 transition-colors">
            {tx({ ru: 'Как это работает', en: 'How it works' })}
          </a>
          <Button size="sm" onClick={() => navigate('/login')}>
            {tx({ ru: 'Войти через Telegram', en: 'Sign in with Telegram' })}
          </Button>
        </div>
      </div>
    </nav>
  );
};

export default LandingNavbar;
