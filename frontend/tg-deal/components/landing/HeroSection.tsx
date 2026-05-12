
import React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '../ui/Button';
import { Send } from 'lucide-react';
import { useI18n } from '../../i18n/I18nProvider';

const HeroSection: React.FC = () => {
  const { tx } = useI18n();
  const navigate = useNavigate();
  return (
    <section className="relative pt-20 pb-32 overflow-hidden">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col lg:flex-row items-center gap-16">
        <div className="lg:w-1/2 z-10">
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-cyan-500/10 text-cyan-400 text-sm font-medium mb-6 border border-cyan-500/20">
            <span className="relative flex h-2 w-2">
              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-cyan-400 opacity-75"></span>
            <span className="relative inline-flex rounded-full h-2 w-2 bg-cyan-500"></span>
            </span>
            {tx({ ru: 'Платформа №1 для Telegram рекламы', en: '#1 platform for Telegram ads' })}
          </div>
          <h1 className="text-4xl lg:text-6xl font-bold tracking-tight text-white mb-6 leading-[1.15]">
            {tx({ ru: 'Биржа рекламы Telegram.', en: 'Telegram advertising marketplace.' })}
            <span className="block text-transparent bg-clip-text bg-gradient-to-r from-cyan-400 to-blue-500 mt-2">
              {tx({ ru: 'Найдите канал за 5 минут.', en: 'Find a channel in 5 minutes.' })}
            </span>
          </h1>
          <p className="text-lg text-slate-400 mb-8 max-w-lg leading-relaxed">
            {tx({
              ru: 'Прямые сделки, прозрачная статистика и автоматическое размещение постов. Мы соединяем тысячи проверенных каналов с рекламодателями.',
              en: 'Direct deals, transparent stats, and automatic post placements. We connect thousands of verified channels with advertisers.',
            })}
          </p>
          <div className="flex flex-col sm:flex-row gap-4">
            <Button size="lg" onClick={() => navigate('/login')} className="group">
              <Send className="mr-2 group-hover:-translate-y-0.5 group-hover:translate-x-0.5 transition-transform" size={18} />
              {tx({ ru: 'Начать работу / Войти', en: 'Get Started / Sign in' })}
            </Button>
            <Button size="lg" variant="outline" onClick={() => document.getElementById('how-it-works')?.scrollIntoView({ behavior: 'smooth' })}>
              {tx({ ru: 'Узнать больше', en: 'Learn more' })}
            </Button>
          </div>
          
          <div className="mt-10 flex items-center gap-6 text-sm text-slate-500">
             <div className="flex -space-x-2">
                {[1,2,3,4].map(i => (
                  <img key={i} className="inline-block h-8 w-8 rounded-full ring-2 ring-slate-900" src={`https://picsum.photos/id/${50+i}/50/50`} alt=""/>
                ))}
             </div>
             <div>
                <span className="font-bold text-white">2,000+</span>{' '}
                {tx({ ru: 'Владельцев каналов', en: 'Channel owners' })}
             </div>
          </div>
        </div>
        
        <div className="lg:w-1/2 relative w-full flex justify-center">
           {/* Декоративное свечение фона. */}
           <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-[120%] h-[120%] bg-cyan-500/20 rounded-full blur-[100px] pointer-events-none"></div>
           
           {/* Баннер с визуальным примером сделки. */}
           <div className="relative w-full max-w-lg aspect-square bg-slate-900 rounded-3xl overflow-hidden border-2 border-slate-700/50 shadow-2xl group">
              {/* Неоновая рамка вокруг превью. */}
              <div className="absolute inset-4 border-2 border-cyan-500/30 rounded-2xl z-20 shadow-[0_0_20px_rgba(6,182,212,0.2)] group-hover:border-cyan-400/50 group-hover:shadow-[0_0_30px_rgba(6,182,212,0.4)] transition-all duration-500"></div>
              
              {/* Фоновое технологичное изображение. */}
              <div className="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1518770660439-4636190af475?q=80&w=1000&auto=format&fit=crop')] bg-cover bg-center opacity-40 mix-blend-luminosity"></div>
              <div className="absolute inset-0 bg-gradient-to-t from-slate-900 via-slate-900/50 to-transparent"></div>

              {/* Анимированные кольца вокруг центрального блока. */}
              <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-3/4 h-3/4 border border-cyan-500/20 rounded-full animate-[spin_10s_linear_infinite]"></div>
              <div className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 w-2/3 h-2/3 border border-purple-500/20 rounded-full animate-[spin_15s_linear_infinite_reverse]"></div>

              {/* Центральный контент баннера. */}
              <div className="absolute inset-0 flex flex-col items-center justify-center z-30 p-8 text-center">
                 {/* 3D Paper Plane Icon */}
                 <div className="mb-6 relative">
                    <div className="absolute inset-0 bg-cyan-500 blur-xl opacity-40"></div>
                    <Send size={80} className="text-cyan-400 relative z-10 transform -rotate-12 drop-shadow-[0_0_15px_rgba(34,211,238,0.8)]" strokeWidth={1.5} />
                 </div>

                 {/* Текст внутри mockup-поста. */}
                 <h2 className="text-4xl md:text-5xl font-black text-white tracking-wider mb-2 drop-shadow-lg">
                    TELEGRAM <span className="text-cyan-400">РЕКЛАМА</span>
                 </h2>
                 <p className="text-xs md:text-sm font-medium text-slate-300 tracking-[0.2em] uppercase">
                    ОХВАТ МИЛЛИОНОВ. РОСТ БИЗНЕСА.
                 </p>
              </div>

              {/* Плавающие декоративные элементы. */}
              <div className="absolute top-1/4 right-1/4 w-2 h-2 bg-purple-400 rounded-full animate-pulse"></div>
              <div className="absolute bottom-1/3 left-1/4 w-1.5 h-1.5 bg-cyan-400 rounded-full animate-pulse delay-700"></div>
              <div className="absolute top-20 left-10 p-2 bg-slate-800/50 backdrop-blur rounded-lg border border-slate-700/50">
                 <div className="w-4 h-4 text-slate-400">💬</div>
              </div>
              <div className="absolute bottom-20 right-10 p-2 bg-slate-800/50 backdrop-blur rounded-lg border border-slate-700/50">
                 <div className="w-4 h-4 text-cyan-400">🎯</div>
              </div>
           </div>
        </div>
      </div>
    </section>
  );
};

export default HeroSection;
