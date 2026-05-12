import React, { useEffect, useMemo, useState } from 'react';
import { Plus, Search } from 'lucide-react';
import Button from '../components/ui/Button';
import Card from '../components/ui/Card';
import { PublisherChannel, User, Role } from '../types';
import {
  getMyChannels,
  createChannel,
  confirmVerification,
  updateChannel,
  setChannelIntakeMode,
} from '../api/publishers';
import { ApiError } from '../api/client';
import { TELEGRAM_BOT_USERNAME } from '../api/config';
import { useI18n } from '../i18n/I18nProvider';

interface MyChannelsProps {
  user: User;
  token: string;
}

const topicLabel = (topic: string) => {
  switch (topic) {
    case 'General':
      return 'Общее';
    case 'IT & Technology':
      return 'Айти и технологии';
    case 'Business & Finance':
      return 'Бизнес и финансы';
    case 'Crypto':
      return 'Крипто';
    default:
      return topic;
  }
};

const intakeModeLabel = (mode: string) => {
  switch (mode) {
    case 'ActiveAuto':
      return 'Автоматический';
    case 'ActiveManual':
      return 'Ручной';
    case 'Paused':
      return 'Пауза';
    default:
      return mode;
  }
};

const ownershipStatusLabel = (status: string) => {
  switch (status) {
    case 'Verified':
      return 'Подтвержден';
    case 'Pending':
      return 'Ожидает подтверждения';
    case 'Banned':
      return 'Заблокирован';
    default:
      return status;
  }
};

const MyChannels: React.FC<MyChannelsProps> = ({ user, token }) => {
  const { tx } = useI18n();
  const [channels, setChannels] = useState<PublisherChannel[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);

  const [searchTerm, setSearchTerm] = useState('');

  const [newTitle, setNewTitle] = useState('');
  const [newTgId, setNewTgId] = useState('');
  const [newTopic, setNewTopic] = useState('Общее');
  const [newLanguage, setNewLanguage] = useState('ru');
  const [newPrice, setNewPrice] = useState('1000');
  const [creating, setCreating] = useState(false);

  const [editingChannelId, setEditingChannelId] = useState<string | null>(null);
  const [editTitle, setEditTitle] = useState('');
  const [editTgId, setEditTgId] = useState('');
  const [editTopic, setEditTopic] = useState('Общее');
  const [editLanguage, setEditLanguage] = useState('ru');
  const [editPrice, setEditPrice] = useState('1000');
  const [savingEdit, setSavingEdit] = useState(false);

  const isPublisher = user.role === Role.Publisher;

  const load = async () => {
    if (!isPublisher) {
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const data = await getMyChannels(token);
      setChannels(data);
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось загрузить каналы', en: 'Failed to load channels' }));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    load();
  }, []);

  const filteredChannels = useMemo(() => {
    if (!searchTerm) return channels;
    const normalized = searchTerm.toLowerCase();
    return channels.filter(
      (channel) =>
        channel.title.toLowerCase().includes(normalized) ||
        channel.telegramChannelId.toLowerCase().includes(normalized) ||
        channel.topic.toLowerCase().includes(normalized)
    );
  }, [channels, searchTerm]);

  const handleCreate = async () => {
    if (!newTitle || !newTgId) {
      setError(tx({ ru: 'Укажите название и Telegram Channel ID', en: 'Provide title and Telegram Channel ID' }));
      return;
    }

    const parsedPrice = Number(newPrice);
    if (!Number.isFinite(parsedPrice) || parsedPrice <= 0) {
      setError(tx({ ru: 'Укажите корректную цену за пост (RUB)', en: 'Provide valid post price (RUB)' }));
      return;
    }

    setCreating(true);
    setError(null);
    setInfo(null);
    try {
      await createChannel(token, newTgId, newTitle, newTopic, newLanguage, parsedPrice);
      setNewTitle('');
      setNewTgId('');
      setNewTopic('Общее');
      setNewLanguage('ru');
      setNewPrice('1000');
      await load();
      setInfo(tx({ ru: 'Канал добавлен', en: 'Channel created' }));
    } catch (e: unknown) {
      if (e instanceof ApiError && e.message === 'DuplicateChannel') {
        setError(tx({ ru: 'Этот канал уже добавлен в ваш профиль.', en: 'This channel is already added to your profile.' }));
      }
      else if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось создать канал', en: 'Failed to create channel' }));
    } finally {
      setCreating(false);
    }
  };

  const beginEdit = (channel: PublisherChannel) => {
    setEditingChannelId(channel.channelId);
    setEditTitle(channel.title);
    setEditTgId(channel.telegramChannelId);
    setEditTopic(topicLabel(channel.topic));
    setEditLanguage(channel.language);
    setEditPrice(String(channel.pricePerPostRub));
  };

  const cancelEdit = () => {
    setEditingChannelId(null);
    setEditTitle('');
    setEditTgId('');
    setEditTopic('Общее');
    setEditLanguage('ru');
    setEditPrice('1000');
  };

  const saveEdit = async () => {
    if (!editingChannelId) return;

    const parsedPrice = Number(editPrice);
    if (!Number.isFinite(parsedPrice) || parsedPrice <= 0) {
      setError(tx({ ru: 'Укажите корректную цену за пост (RUB)', en: 'Provide valid post price (RUB)' }));
      return;
    }

    setSavingEdit(true);
    setError(null);
    setInfo(null);
    try {
      await updateChannel(token, editingChannelId, editTgId, editTitle, editTopic, editLanguage, parsedPrice);
      cancelEdit();
      await load();
      setInfo(tx({ ru: 'Канал обновлен', en: 'Channel updated' }));
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось обновить канал', en: 'Failed to update channel' }));
    } finally {
      setSavingEdit(false);
    }
  };

  const verifyChannel = async (channelId: string) => {
    setError(null);
    setInfo(null);
    try {
      await confirmVerification(token, channelId);
      await load();
      setInfo(tx({ ru: 'Канал подтвержден', en: 'Channel verified' }));
    } catch (e: unknown) {
      if (e instanceof ApiError && e.message === 'TelegramBotMissingChannelPermissions') {
        setError(tx({
          ru: 'Бот не добавлен админом канала или ему не выданы права на публикацию и удаление постов. Добавьте бота админом с нужными правами и попробуйте снова.',
          en: 'The bot is not a channel admin or does not have post/delete permissions. Add the bot as an admin with the required permissions and try again.',
        }));
      }
      else if (e instanceof ApiError && e.message === 'TelegramPublisherUnavailable') {
        setError(tx({
          ru: 'Telegram publisher сейчас недоступен. Проверьте, что сервис запущен, и попробуйте снова.',
          en: 'Telegram publisher is unavailable. Check that the service is running and try again.',
        }));
      }
      else if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось подтвердить канал', en: 'Failed to verify channel' }));
    }
  };

  const handleSetMode = async (
    channelId: string,
    mode: 'ActiveAuto' | 'ActiveManual' | 'Paused'
  ) => {
    setError(null);
    setInfo(null);
    try {
      await setChannelIntakeMode(token, channelId, mode);
      await load();
      setInfo(`${tx({ ru: 'Режим заявок обновлен', en: 'Intake mode updated' })}: ${intakeModeLabel(mode)}`);
    } catch (e: unknown) {
      if (e instanceof ApiError) setError(e.message);
      else if (e instanceof Error) setError(e.message);
      else setError(tx({ ru: 'Не удалось обновить режим заявок', en: 'Failed to update intake mode' }));
    }
  };

  if (!isPublisher) {
    return (
      <Card>
        <div className="text-slate-300">{tx({ ru: 'Доступно только владельцам каналов.', en: 'Available only for Publisher role.' })}</div>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold">{tx({ ru: 'Мои каналы', en: 'My Channels' })}</h2>
          <p className="text-slate-400 text-sm mt-1">{tx({ ru: 'Управляйте профилем, ценами и режимом заявок.', en: 'Manage profile, pricing and intake mode.' })}</p>
        </div>
        <Button icon={Plus} onClick={handleCreate} disabled={creating}>
          {creating ? tx({ ru: 'Создание...', en: 'Creating...' }) : tx({ ru: 'Добавить канал', en: 'Add New Channel' })}
        </Button>
      </div>

      {info && (
        <div className="text-sm text-green-300 bg-green-900/30 border border-green-800 rounded-lg px-3 py-2">
          {info}
        </div>
      )}
      {error && (
        <div className="text-sm text-red-400 bg-red-950 border border-red-800 rounded-lg px-3 py-2">
          {error}
        </div>
      )}

      <Card>
        <div className="space-y-3">
          <h3 className="text-lg font-semibold text-white">Как подтвердить канал</h3>
          <div className="text-sm text-slate-300 space-y-2">
            <p>
              Добавьте бота <span className="font-semibold text-cyan-300">{TELEGRAM_BOT_USERNAME}</span> администратором
              вашего Telegram-канала.
            </p>
            <p>
              В правах администратора включите публикацию сообщений и удаление сообщений. После этого добавьте канал здесь
              и нажмите <span className="font-semibold text-white">Подтвердить</span> в таблице каналов.
            </p>
            <p className="text-slate-400">
              В поле идентификатора канала укажите публичный username без символа @ или внутренний числовой id канала.
            </p>
          </div>
        </div>
      </Card>

      <Card>
        <div className="grid grid-cols-1 md:grid-cols-6 gap-3">
          <input
            value={newTgId}
            onChange={(e) => setNewTgId(e.target.value)}
            className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white"
            placeholder={tx({ ru: 'Telegram ID (без @)', en: 'Telegram ID (without @)' })}
          />
          <input
            value={newTitle}
            onChange={(e) => setNewTitle(e.target.value)}
            className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white"
            placeholder={tx({ ru: 'Название', en: 'Title' })}
          />
          <input
            value={newTopic}
            onChange={(e) => setNewTopic(e.target.value)}
            className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white"
            placeholder={tx({ ru: 'Тематика', en: 'Topic' })}
          />
          <select
            value={newLanguage}
            onChange={(e) => setNewLanguage(e.target.value)}
            className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white"
          >
            <option value="ru">Русский</option>
            <option value="en">Английский</option>
          </select>
          <input
            type="number"
            min={1}
            value={newPrice}
            onChange={(e) => setNewPrice(e.target.value)}
            className="bg-slate-900 border border-slate-700 rounded-lg px-3 py-2 text-white"
            placeholder={tx({ ru: 'Цена RUB', en: 'Price RUB' })}
          />
          <div className="text-xs text-slate-400 flex items-center">{tx({ ru: 'Базовая цена за один пост', en: 'Base price for one post' })}</div>
        </div>
      </Card>

      <div className="relative flex-1 max-w-lg">
        <Search className="absolute left-3 top-2.5 text-slate-500" size={18} />
        <input
          type="text"
          placeholder={tx({ ru: 'Поиск по названию, тематике или Telegram ID...', en: 'Search by title, topic or Telegram ID...' })}
          className="w-full bg-slate-800 border border-slate-700 rounded-lg pl-10 pr-4 py-2 text-sm text-white"
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
      </div>

      <Card noPadding>
        <div className="overflow-x-auto">
          <table className="w-full text-left">
            <thead>
              <tr className="bg-slate-800/50 text-xs font-semibold text-slate-400 uppercase">
                <th className="px-6 py-4">Канал</th>
                    <th className="px-6 py-4">Идентификатор Telegram</th>
                <th className="px-6 py-4">Тематика</th>
                <th className="px-6 py-4">Язык</th>
                <th className="px-6 py-4">Цена за пост</th>
                <th className="px-6 py-4">Режим заявок</th>
                <th className="px-6 py-4">Статус</th>
                <th className="px-6 py-4 text-center">Действия</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-700 text-sm">
              {loading ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={8}>
                    Загрузка...
                  </td>
                </tr>
              ) : filteredChannels.length === 0 ? (
                <tr>
                  <td className="px-6 py-4 text-slate-400" colSpan={8}>
                    Каналов не найдено
                  </td>
                </tr>
              ) : (
                filteredChannels.map((channel) => {
                  const isEditing = editingChannelId === channel.channelId;
                  const isBanned = channel.ownershipStatus === 'Banned';
                  return (
                    <tr key={channel.channelId} className="hover:bg-slate-750/50">
                      <td className="px-6 py-4 font-medium text-white">
                        {isEditing ? (
                          <input
                            value={editTitle}
                            onChange={(e) => setEditTitle(e.target.value)}
                            className="bg-slate-900 border border-slate-700 rounded px-2 py-1 text-white"
                          />
                        ) : (
                          channel.title
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-300">
                        {isEditing ? (
                          <input
                            value={editTgId}
                            onChange={(e) => setEditTgId(e.target.value)}
                            className="bg-slate-900 border border-slate-700 rounded px-2 py-1 text-white"
                          />
                        ) : (
                          `@${channel.telegramChannelId}`
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-300">
                        {isEditing ? (
                          <input
                            value={editTopic}
                            onChange={(e) => setEditTopic(e.target.value)}
                            className="bg-slate-900 border border-slate-700 rounded px-2 py-1 text-white"
                          />
                        ) : (
                          topicLabel(channel.topic)
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-300">
                        {isEditing ? (
                          <select
                            value={editLanguage}
                            onChange={(e) => setEditLanguage(e.target.value)}
                            className="bg-slate-900 border border-slate-700 rounded px-2 py-1 text-white"
                          >
                            <option value="ru">Русский</option>
                            <option value="en">Английский</option>
                          </select>
                        ) : (
                          channel.language === 'ru' ? 'Русский' : channel.language === 'en' ? 'Английский' : channel.language
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-300">
                        {isEditing ? (
                          <input
                            type="number"
                            min={1}
                            value={editPrice}
                            onChange={(e) => setEditPrice(e.target.value)}
                            className="bg-slate-900 border border-slate-700 rounded px-2 py-1 text-white"
                          />
                        ) : (
                          `${channel.pricePerPostRub.toLocaleString('ru-RU')} ₽`
                        )}
                      </td>
                      <td className="px-6 py-4 text-slate-300">{intakeModeLabel(channel.intakeMode)}</td>
                      <td className="px-6 py-4 text-slate-300">{ownershipStatusLabel(channel.ownershipStatus)}</td>
                      <td className="px-6 py-4 text-center">
                        <div className="flex items-center justify-center gap-2 flex-wrap">
                          {isEditing ? (
                            <>
                              <Button size="sm" onClick={saveEdit} disabled={savingEdit}>
                                {savingEdit ? 'Сохранение...' : 'Сохранить'}
                              </Button>
                              <Button size="sm" variant="secondary" onClick={cancelEdit}>
                                Отмена
                              </Button>
                            </>
                          ) : (
                            <>
                              {isBanned ? (
                                <span className="text-xs text-red-300 bg-red-900/30 border border-red-800 px-2 py-1 rounded">
                                  Заблокирован навсегда
                                </span>
                              ) : (
                                <>
                                  <Button size="sm" variant="secondary" onClick={() => beginEdit(channel)}>
                                    Редактировать
                                  </Button>
                                  <select
                                    value={channel.intakeMode}
                                    onChange={(e) =>
                                      handleSetMode(
                                        channel.channelId,
                                        e.target.value as 'ActiveAuto' | 'ActiveManual' | 'Paused'
                                      )
                                    }
                                    className="bg-slate-900 border border-slate-700 rounded px-2 py-1 text-white text-xs"
                                  >
                                    <option value="ActiveAuto">Автоматический</option>
                                    <option value="ActiveManual">Ручной</option>
                                    <option value="Paused">Пауза</option>
                                  </select>
                                  {channel.ownershipStatus !== 'Verified' && (
                                    <Button size="sm" variant="secondary" onClick={() => verifyChannel(channel.channelId)}>
                                      Подтвердить
                                    </Button>
                                  )}
                                </>
                              )}
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
};

export default MyChannels;
