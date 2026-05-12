import React from 'react';
import Card from '../ui/Card';
import { useI18n } from '../../i18n/I18nProvider';

const AdminDashboard: React.FC = () => {
  const { tx } = useI18n();
  return (
    <div className="space-y-6">
      <div>
        <h2 className="text-2xl font-bold">{tx({ ru: 'Центр управления администратора', en: 'Admin Control Center' })}</h2>
        <p className="text-slate-400 text-sm mt-1">
          {tx({
            ru: 'Используйте модерацию каналов и раздел споров для управления сделками.',
            en: 'Use marketplace moderation for channels and dispute resolve for deals.',
          })}
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card>
          <p className="text-slate-400 text-sm">{tx({ ru: 'Модерация каналов', en: 'Channel Moderation' })}</p>
          <h3 className="text-xl font-semibold text-white mt-2">{tx({ ru: 'Блокировка / Разблокировка', en: 'Ban / Unban' })}</h3>
          <p className="text-xs text-slate-500 mt-2">
            Раздел модерации каналов доступен через боковое меню.
          </p>
        </Card>

        <Card>
          <p className="text-slate-400 text-sm">{tx({ ru: 'Споры', en: 'Disputes' })}</p>
          <h3 className="text-xl font-semibold text-white mt-2">{tx({ ru: 'Полная очередь споров', en: 'Full Dispute Queue' })}</h3>
          <p className="text-xs text-slate-500 mt-2">
            {tx({ ru: 'Управляйте всеми спорами в ', en: 'Manage all disputes in ' })}
            <code>/deals</code> {tx({ ru: 'и выбирайте действие для решения спора.', en: 'and resolve actions.' })}
          </p>
        </Card>

        <Card>
          <p className="text-slate-400 text-sm">{tx({ ru: 'Права доступа', en: 'Permissions' })}</p>
          <h3 className="text-xl font-semibold text-white mt-2">{tx({ ru: 'Ролевой доступ', en: 'Role-Based Access' })}</h3>
          <p className="text-xs text-slate-500 mt-2">
            {tx({
              ru: 'Действия только для администратора доступны в разделах бокового меню.',
              en: 'Admin-only actions are available from the sidebar sections.',
            })}
          </p>
        </Card>
      </div>
    </div>
  );
};

export default AdminDashboard;
