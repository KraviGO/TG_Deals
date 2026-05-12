import React from 'react';
import { UserPlus, Search, CreditCard, CheckCircle } from 'lucide-react';
import { useI18n } from '../../i18n/I18nProvider';

const HowItWorksSection: React.FC = () => {
  const { tx } = useI18n();
  const steps = [
    {
      icon: UserPlus,
      title: tx({ ru: 'Регистрация', en: 'Registration' }),
      desc: tx({
        ru: 'Зайдите через Telegram в один клик. Выберите роль: Рекламодатель или Владелец канала.',
        en: 'Sign in with Telegram in one click. Choose your role: Advertiser or Channel Owner.',
      }),
    },
    {
      icon: Search,
      title: tx({ ru: 'Поиск канала', en: 'Channel Search' }),
      desc: tx({
        ru: 'Используйте фильтры для поиска целевой аудитории. Изучите статистику и отзывы.',
        en: 'Use filters to find your target audience. Review analytics and feedback.',
      }),
    },
    {
      icon: CreditCard,
      title: tx({ ru: 'Безопасная оплата', en: 'Secure Payment' }),
      desc: tx({
        ru: 'Оплатите размещение. Средства замораживаются и поступают продавцу только после публикации.',
        en: 'Pay for placement. Funds are held and released only after publication.',
      }),
    },
    {
      icon: CheckCircle,
      title: tx({ ru: 'Публикация и отчет', en: 'Publication and Report' }),
      desc: tx({
        ru: 'Наш бот автоматически опубликует пост. Вы получите уведомление и отчет о размещении.',
        en: 'Our bot publishes the post automatically. You receive notifications and delivery reports.',
      }),
    },
  ];

  return (
    <section id="how-it-works" className="py-24 bg-slate-900 relative overflow-hidden">
      <div className="absolute top-0 left-0 w-full h-full bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] from-slate-800 via-slate-900 to-slate-900 opacity-50"></div>
      
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 relative z-10">
        <div className="text-center mb-16">
          <h2 className="text-3xl lg:text-4xl font-bold text-white mb-4">
            {tx({ ru: 'Как это работает?', en: 'How does it work?' })}
          </h2>
          <p className="text-slate-400 max-w-2xl mx-auto">
            {tx({
              ru: 'Простой путь от регистрации до первой успешной рекламной кампании.',
              en: 'A simple path from registration to your first successful ad campaign.',
            })}
          </p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          {steps.map((step, index) => (
            <div key={index} className="relative group">
              {/* Линия связывает шаги сценария на desktop. */}
              {index < steps.length - 1 && (
                <div className="hidden lg:block absolute top-8 left-1/2 w-full h-0.5 bg-slate-800 -z-10 group-hover:bg-cyan-900/50 transition-colors"></div>
              )}
              
              <div className="bg-slate-800 border border-slate-700 rounded-2xl p-6 h-full hover:border-cyan-500/50 transition-colors shadow-lg">
                <div className="w-16 h-16 bg-slate-900 rounded-full flex items-center justify-center border border-slate-700 mb-6 mx-auto group-hover:scale-110 transition-transform shadow-inner">
                  <step.icon className="text-cyan-400" size={32} />
                </div>
                <div className="text-center">
                  <div className="text-cyan-500 font-bold text-sm mb-2">
                    {tx({ ru: 'Шаг', en: 'Step' })} {index + 1}
                  </div>
                  <h3 className="text-xl font-bold text-white mb-3">{step.title}</h3>
                  <p className="text-slate-400 text-sm leading-relaxed">{step.desc}</p>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
};

export default HowItWorksSection;
