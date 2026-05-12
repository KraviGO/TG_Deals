import React from 'react';
import { Check, X } from 'lucide-react';

const ComparisonSection: React.FC = () => {
  const data = [
    {
      platform: 'Telega.in',
      auto: 'Нужны ручные шаги',
      tg: 'Есть бот, но размещать надо вручную',
      fraud: 'Простая защита',
      analytics: 'Показы и клики',
      commission: '10-20% сверху',
      usability: 'Сложновато'
    },
    {
      platform: 'Teletarget',
      auto: 'Есть авто-функции, но не везде',
      tg: 'Бот помогает, но публикации вручную',
      fraud: 'Фильтр плохих каналов',
      analytics: 'Статистика каналов',
      commission: '10-20%',
      usability: 'Для простых задач'
    },
    {
      platform: 'AdGram',
      auto: 'Автоматизации почти нет',
      tg: 'Бот с минимумом функций',
      fraud: 'Нет защиты',
      analytics: 'Почти нет',
      commission: '10-20%',
      usability: 'Ограниченно'
    },
    {
      platform: 'TG Deal',
      isOur: true,
      auto: 'Полная автоматизация',
      tg: 'Публикует напрямую без участия человека',
      fraud: 'Сам анализирует, убирает фейки',
      analytics: 'Результат: лиды, цена за действие',
      commission: '5-10% + подписка',
      usability: 'Быстро и просто'
    }
  ];

  return (
    <section id="comparison" className="py-24 bg-slate-900">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center mb-16">
          <h2 className="text-3xl lg:text-4xl font-bold text-white mb-4">Сравнение с конкурентами</h2>
          <p className="text-slate-400">Почему TG Deal — лучший выбор для вашего бизнеса.</p>
        </div>

        <div className="overflow-x-auto pb-4">
          <div className="min-w-[1000px] bg-slate-800 rounded-2xl border border-slate-700 overflow-hidden shadow-2xl">
            <div className="grid grid-cols-7 bg-slate-900 border-b border-slate-700 text-sm font-semibold text-slate-400 p-4 sticky top-0">
               <div className="col-span-1">Платформа</div>
               <div className="col-span-1">Автоматизация</div>
               <div className="col-span-1">Telegram</div>
               <div className="col-span-1">Антифрод</div>
               <div className="col-span-1">Аналитика</div>
               <div className="col-span-1">Комиссии</div>
               <div className="col-span-1">Удобство</div>
            </div>

            {data.map((row, idx) => (
              <div 
                key={idx} 
                className={`grid grid-cols-7 p-6 text-sm items-center border-b border-slate-700 last:border-0 transition-colors
                  ${row.isOur ? 'bg-cyan-900/20 border-l-4 border-l-cyan-500' : 'hover:bg-slate-750'}
                `}
              >
                 <div className="col-span-1 font-bold text-white flex items-center gap-2">
                    {row.platform}
                    {row.isOur && <span className="bg-cyan-500 text-xs px-2 py-0.5 rounded text-white">Выбор</span>}
                 </div>
                 <div className={`col-span-1 ${row.isOur ? 'text-green-400 font-medium' : 'text-slate-300'}`}>{row.auto}</div>
                 <div className={`col-span-1 ${row.isOur ? 'text-green-400 font-medium' : 'text-slate-300'}`}>{row.tg}</div>
                 <div className={`col-span-1 ${row.isOur ? 'text-green-400 font-medium' : 'text-slate-300'}`}>{row.fraud}</div>
                 <div className={`col-span-1 ${row.isOur ? 'text-green-400 font-medium' : 'text-slate-300'}`}>{row.analytics}</div>
                 <div className={`col-span-1 ${row.isOur ? 'text-white font-bold' : 'text-slate-300'}`}>{row.commission}</div>
                 <div className={`col-span-1 ${row.isOur ? 'text-green-400 font-medium' : 'text-slate-300'}`}>{row.usability}</div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
};

export default ComparisonSection;