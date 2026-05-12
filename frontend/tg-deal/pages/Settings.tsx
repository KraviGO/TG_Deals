import React from 'react';
import { User } from '../types';
import Card from '../components/ui/Card';
import Button from '../components/ui/Button';
import { useI18n } from '../i18n/I18nProvider';

interface SettingsProps {
  user: User;
}

const Settings: React.FC<SettingsProps> = ({ user }) => {
  const { tx } = useI18n();

  return (
    <div className="space-y-6 max-w-4xl">
      <div>
         <h2 className="text-2xl font-bold">{tx({ ru: 'Профиль', en: 'Profile' })}</h2>
         <p className="text-slate-400 text-sm mt-1">
           {tx({
             ru: 'Обновите фото и персональные данные.',
             en: 'Update your photo and personal details.',
           })}
         </p>
      </div>

      <Card>
         <div className="flex flex-col md:flex-row items-start md:items-center gap-6 pb-6 border-b border-slate-700 mb-6">
            <img src={user.avatar} alt="Фото профиля" className="w-24 h-24 rounded-full border-4 border-slate-700 object-cover" />
            <div>
               <h3 className="text-lg font-bold text-white mb-1">{tx({ ru: 'Ваше фото', en: 'Your Photo' })}</h3>
               <p className="text-sm text-slate-400 mb-4">
                 {tx({ ru: 'Это фото будет отображаться в вашем профиле.', en: 'This will be displayed on your profile.' })}
               </p>
               <Button variant="secondary" className="bg-slate-700 border border-slate-600">
                 {tx({ ru: 'Изменить фото', en: 'Change Photo' })}
               </Button>
            </div>
         </div>

         <div className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
               <div>
                  <label className="text-sm font-medium text-slate-300 mb-2 block">{tx({ ru: 'Полное имя', en: 'Full Name' })}</label>
                  <input 
                    type="text" 
                    defaultValue={user.name}
                    className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:border-cyan-500"
                  />
               </div>
               <div>
                  <label className="text-sm font-medium text-slate-300 mb-2 block">{tx({ ru: 'Контакт', en: 'Contact' })}</label>
                  <input 
                    type="text" 
                    defaultValue="@alex_doe_tg"
                    className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:border-cyan-500"
                  />
               </div>
            </div>

            <div>
               <label className="text-sm font-medium text-slate-300 mb-2 block">{tx({ ru: 'Электронная почта', en: 'Email Address' })}</label>
               <input 
                 type="email" 
                 defaultValue={user.email || 'alex.doe@example.com'}
                 className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:border-cyan-500"
               />
            </div>

            <div>
               <label className="text-sm font-medium text-slate-300 mb-2 block">{tx({ ru: 'О себе', en: 'Public Bio' })}</label>
               <textarea 
                 rows={4}
                 defaultValue={tx({
                   ru: 'Я digital-маркетолог, специализируюсь на продвижении Telegram-каналов.',
                   en: "I'm a digital marketer specializing in Telegram channel growth and promotion. Let's connect!",
                 })}
                 className="w-full bg-slate-900 border border-slate-700 rounded-lg px-4 py-2.5 text-white focus:outline-none focus:border-cyan-500 resize-none"
               />
            </div>
         </div>

         <div className="flex justify-end gap-3 mt-8 pt-6 border-t border-slate-700">
            <Button variant="ghost">{tx({ ru: 'Отмена', en: 'Cancel' })}</Button>
            <Button>{tx({ ru: 'Сохранить', en: 'Save Changes' })}</Button>
         </div>
      </Card>
    </div>
  );
};

export default Settings;
