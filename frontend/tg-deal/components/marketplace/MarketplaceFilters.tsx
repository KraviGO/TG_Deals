
import React from 'react';
import { Search } from 'lucide-react';
import Button from '../ui/Button';
import { useI18n } from '../../i18n/I18nProvider';

interface MarketplaceFiltersProps {
  searchTerm: string;
  setSearchTerm: (term: string) => void;
  topic: string;
  setTopic: (value: string) => void;
  language: string;
  setLanguage: (value: string) => void;
  intakeMode: string;
  setIntakeMode: (value: string) => void;
  minPrice: string;
  setMinPrice: (value: string) => void;
  maxPrice: string;
  setMaxPrice: (value: string) => void;
  onApply: () => void;
  onClear: () => void;
}

const MarketplaceFilters: React.FC<MarketplaceFiltersProps> = ({
  searchTerm,
  setSearchTerm,
  topic,
  setTopic,
  language,
  setLanguage,
  intakeMode,
  setIntakeMode,
  minPrice,
  setMinPrice,
  maxPrice,
  setMaxPrice,
  onApply,
  onClear,
}) => {
  const { tx } = useI18n();
  return (
    <div className="w-64 flex-shrink-0 hidden lg:block">
      <h2 className="text-lg font-bold mb-4">{tx({ ru: 'Найдите подходящий канал', en: 'Find the perfect channel' })}</h2>
      <p className="text-sm text-slate-400 mb-6">{tx({ ru: 'Фильтруйте и ищите каналы', en: 'Filter and search for channels' })}</p>

      <div className="space-y-6">
        <div className="relative">
           <Search className="absolute left-3 top-2.5 text-slate-500" size={18} />
           <input 
             type="text" 
             placeholder={tx({ ru: 'Поиск по названию канала...', en: 'Search by channel name...' })} 
             className="w-full bg-slate-800 border border-slate-700 rounded-lg pl-10 pr-4 py-2 text-sm text-white focus:outline-none focus:border-cyan-500 transition-colors"
             value={searchTerm}
             onChange={(e) => setSearchTerm(e.target.value)}
           />
        </div>

        <div>
           <label className="text-sm font-medium text-slate-300 mb-2 block">{tx({ ru: 'Тематика', en: 'Topic' })}</label>
           <select
             className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-white focus:outline-none"
             value={topic}
             onChange={(e) => setTopic(e.target.value)}
           >
              <option value="">{tx({ ru: 'Все тематики', en: 'All Topics' })}</option>
              <option value="IT & Technology">Айти и технологии</option>
              <option value="Business & Finance">Бизнес и финансы</option>
              <option value="Crypto">Крипто</option>
              <option value="General">Общее</option>
           </select>
        </div>

        <div>
          <label className="text-sm font-medium text-slate-300 mb-2 block">{tx({ ru: 'Язык', en: 'Language' })}</label>
          <select
            className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-white focus:outline-none"
            value={language}
            onChange={(e) => setLanguage(e.target.value)}
          >
            <option value="">{tx({ ru: 'Любой язык', en: 'Any language' })}</option>
            <option value="ru">Русский</option>
            <option value="en">Английский</option>
          </select>
        </div>

        <div>
          <label className="text-sm font-medium text-slate-300 mb-2 block">{tx({ ru: 'Режим заявок', en: 'Intake mode' })}</label>
          <select
            className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-white focus:outline-none"
            value={intakeMode}
            onChange={(e) => setIntakeMode(e.target.value)}
          >
            <option value="">{tx({ ru: 'Любой режим', en: 'Any mode' })}</option>
            <option value="ActiveAuto">{tx({ ru: 'Авто', en: 'Auto' })}</option>
            <option value="ActiveManual">{tx({ ru: 'Ручной', en: 'Manual' })}</option>
            <option value="Paused">{tx({ ru: 'Пауза', en: 'Paused' })}</option>
          </select>
        </div>

        <div className="grid grid-cols-2 gap-2">
          <input
            type="number"
            min={0}
            placeholder={tx({ ru: 'Мин ₽', en: 'Min ₽' })}
            value={minPrice}
            onChange={(e) => setMinPrice(e.target.value)}
            className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-white focus:outline-none"
          />
          <input
            type="number"
            min={0}
            placeholder={tx({ ru: 'Макс ₽', en: 'Max ₽' })}
            value={maxPrice}
            onChange={(e) => setMaxPrice(e.target.value)}
            className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-white focus:outline-none"
          />
        </div>

        <Button fullWidth onClick={onApply}>{tx({ ru: 'Применить фильтры', en: 'Apply Filters' })}</Button>
        <Button variant="ghost" fullWidth onClick={onClear}>{tx({ ru: 'Сбросить фильтры', en: 'Clear Filters' })}</Button>
      </div>
    </div>
  );
};

export default MarketplaceFilters;
