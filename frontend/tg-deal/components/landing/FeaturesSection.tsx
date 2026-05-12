import React from 'react';
import { Target, TrendingUp, ShieldCheck, Zap } from 'lucide-react';
import { useI18n } from '../../i18n/I18nProvider';

const FeaturesSection: React.FC = () => {
  const { tx } = useI18n();
  return (
    <section id="marketplace" className="py-24 bg-slate-900 border-t border-slate-800">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="text-center max-w-3xl mx-auto mb-16">
          <h2 className="text-3xl lg:text-4xl font-bold mb-6 text-white">
            {tx({ ru: 'Почему выбирают TG Deal?', en: 'Why choose TG Deal?' })}
          </h2>
          <p className="text-slate-400 text-lg">
            {tx({
              ru: 'Мы создали бесшовный опыт взаимодействия как для рекламодателей, так и для владельцев каналов.',
              en: 'We built a seamless experience both for advertisers and channel owners.',
            })}
          </p>
        </div>

        <div className="grid md:grid-cols-2 gap-8">
          {/* Блок для рекламодателей. */}
          <div className="bg-gradient-to-b from-slate-800 to-slate-800/50 p-8 rounded-2xl border border-slate-700 hover:border-cyan-500/50 transition-all duration-300 group hover:-translate-y-1 shadow-lg">
            <div className="flex items-center gap-4 mb-6">
               <div className="w-12 h-12 bg-slate-900 rounded-xl flex items-center justify-center group-hover:bg-cyan-900/30 transition-colors border border-slate-700">
                 <Target className="text-cyan-400" size={24} />
               </div>
               <h3 className="text-2xl font-bold text-white">{tx({ ru: 'Для Рекламодателей', en: 'For Advertisers' })}</h3>
            </div>
            
            <ul className="space-y-4">
               <li className="flex items-start">
                  <ShieldCheck className="text-green-400 mt-1 mr-3 flex-shrink-0" size={18} />
                  <p className="text-slate-300">
                    <strong className="text-white">{tx({ ru: 'Безопасная сделка:', en: 'Secure deal:' })}</strong>{' '}
                    {tx({
                      ru: 'Деньги холдируются и переводятся автору только после успешной публикации.',
                      en: 'Funds are held and transferred only after successful publication.',
                    })}
                  </p>
               </li>
               <li className="flex items-start">
                  <Zap className="text-yellow-400 mt-1 mr-3 flex-shrink-0" size={18} />
                  <p className="text-slate-300">
                    <strong className="text-white">{tx({ ru: 'Умные фильтры:', en: 'Smart filters:' })}</strong>{' '}
                    {tx({
                      ru: 'Поиск по тематике, цене, ERR и вовлеченности аудитории.',
                      en: 'Search by topic, price, ERR, and audience engagement.',
                    })}
                  </p>
               </li>
               <li className="flex items-start">
                  <TrendingUp className="text-blue-400 mt-1 mr-3 flex-shrink-0" size={18} />
                  <p className="text-slate-300">
                    <strong className="text-white">{tx({ ru: 'Детальная аналитика:', en: 'Detailed analytics:' })}</strong>{' '}
                    {tx({
                      ru: 'Прозрачные данные по каждому каналу перед покупкой.',
                      en: 'Transparent channel data before purchase.',
                    })}
                  </p>
               </li>
            </ul>
          </div>

          {/* Блок для владельцев каналов. */}
          <div className="bg-gradient-to-b from-slate-800 to-slate-800/50 p-8 rounded-2xl border border-slate-700 hover:border-cyan-500/50 transition-all duration-300 group hover:-translate-y-1 shadow-lg">
            <div className="flex items-center gap-4 mb-6">
               <div className="w-12 h-12 bg-slate-900 rounded-xl flex items-center justify-center group-hover:bg-cyan-900/30 transition-colors border border-slate-700">
                 <TrendingUp className="text-cyan-400" size={24} />
               </div>
               <h3 className="text-2xl font-bold text-white">{tx({ ru: 'Для Владельцев Каналов', en: 'For Channel Owners' })}</h3>
            </div>

            <ul className="space-y-4">
               <li className="flex items-start">
                  <Zap className="text-yellow-400 mt-1 mr-3 flex-shrink-0" size={18} />
                  <p className="text-slate-300">
                    <strong className="text-white">{tx({ ru: 'Автоматизация:', en: 'Automation:' })}</strong>{' '}
                    {tx({
                      ru: 'Бот сам опубликует пост и удалит его в нужное время. Вы только подтверждаете заявку.',
                      en: 'The bot publishes and removes posts on schedule. You only approve the deal.',
                    })}
                  </p>
               </li>
               <li className="flex items-start">
                  <ShieldCheck className="text-green-400 mt-1 mr-3 flex-shrink-0" size={18} />
                  <p className="text-slate-300">
                    <strong className="text-white">{tx({ ru: 'Гарантия выплат:', en: 'Guaranteed payouts:' })}</strong>{' '}
                    {tx({
                      ru: 'Никаких "кидков". Оплата поступает на баланс сразу после выполнения условий сделки.',
                      en: 'No payment scams. Funds arrive right after deal conditions are completed.',
                    })}
                  </p>
               </li>
               <li className="flex items-start">
                  <Target className="text-purple-400 mt-1 mr-3 flex-shrink-0" size={18} />
                  <p className="text-slate-300">
                    <strong className="text-white">{tx({ ru: 'Календарь слотов:', en: 'Slot calendar:' })}</strong>{' '}
                    {tx({
                      ru: 'Удобное управление расписанием и свободными местами.',
                      en: 'Convenient scheduling and slot availability management.',
                    })}
                  </p>
               </li>
            </ul>
          </div>
        </div>
      </div>
    </section>
  );
};

export default FeaturesSection;
