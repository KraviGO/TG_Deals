import React from 'react';
import { useI18n } from '../../i18n/I18nProvider';

const Footer: React.FC = () => {
  const { tx } = useI18n();
  return (
    <footer className="border-t border-slate-800 py-12 bg-slate-900">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
         <div className="flex flex-col md:flex-row justify-between items-center gap-6 mb-8">
            <div className="flex items-center gap-2">
               <div className="w-8 h-8 bg-cyan-500 rounded flex items-center justify-center font-bold text-slate-900">TG</div>
               <span className="font-bold text-xl tracking-tight text-white">TG Deal</span>
            </div>
            <div className="flex gap-8 text-sm text-slate-400">
               <a href="#" className="hover:text-white transition-colors">{tx({ ru: 'О нас', en: 'About' })}</a>
               <a href="#" className="hover:text-white transition-colors">{tx({ ru: 'Контакты', en: 'Contacts' })}</a>
               <a href="#" className="hover:text-white transition-colors">{tx({ ru: 'Политика', en: 'Policy' })}</a>
               <a href="#" className="hover:text-white transition-colors">{tx({ ru: 'Оферта', en: 'Terms' })}</a>
            </div>
         </div>
         <div className="border-t border-slate-800 pt-8 text-center md:text-left text-slate-500 text-sm">
            {tx({ ru: '© 2024 TG Deal. Все права защищены.', en: '© 2024 TG Deal. All rights reserved.' })}
         </div>
      </div>
    </footer>
  );
};

export default Footer;
