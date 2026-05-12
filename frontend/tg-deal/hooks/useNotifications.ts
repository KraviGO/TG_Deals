import { useCallback, useEffect, useMemo, useState } from 'react';
import { listAdminDisputes, listMyDeals, listPublisherInbox } from '../api/deals';
import { AppNotification, Role, User } from '../types';
import { useI18n } from '../i18n/I18nProvider';

const POLL_MS = 30000;

const storageKey = (userId: string) => `tgdeal.notifications.read.${userId}`;

const parseStoredReadIds = (raw: string | null): Set<string> => {
  if (!raw) return new Set<string>();
  try {
    const parsed = JSON.parse(raw) as string[];
    return new Set(parsed);
  } catch {
    return new Set<string>();
  }
};

const saveReadIds = (userId: string, ids: Set<string>) => {
  const compact = Array.from(ids).slice(-500);
  localStorage.setItem(storageKey(userId), JSON.stringify(compact));
};

export const useNotifications = (user: User, token: string) => {
  const { tx } = useI18n();
  const [notifications, setNotifications] = useState<AppNotification[]>([]);
  const [readIds, setReadIds] = useState<Set<string>>(
    () => parseStoredReadIds(localStorage.getItem(storageKey(user.id)))
  );

  useEffect(() => {
    setReadIds(parseStoredReadIds(localStorage.getItem(storageKey(user.id))));
  }, [user.id]);

  const fetchNotifications = useCallback(async () => {
    if (!token) {
      setNotifications([]);
      return;
    }

    try {
      if (user.role === Role.Advertiser) {
        const deals = await listMyDeals(token);
        const mapped = deals.map<AppNotification>((deal) => ({
          id: `deal:${deal.dealId}:${deal.status}:${deal.fundingStatus}:${deal.publishedAtUtc ?? ''}`,
          title:
            deal.status === 'PublishedPendingConfirm'
              ? tx({ ru: 'Пост опубликован, нужно подтверждение', en: 'Post published, confirmation required' })
              : deal.status === 'Completed'
              ? tx({ ru: 'Сделка завершена', en: 'Deal completed' })
              : deal.status === 'Rejected'
              ? tx({ ru: 'Заявка отклонена', en: 'Deal rejected' })
              : deal.status === 'Disputed'
              ? tx({ ru: 'Открыт спор по сделке', en: 'Dispute opened for deal' })
              : deal.status === 'Resolved'
              ? tx({ ru: 'Спор решен', en: 'Dispute resolved' })
              : tx({ ru: 'Обновление по сделке', en: 'Deal update' }),
          body: `${deal.amount.toLocaleString('ru-RU')} ${deal.currency} • status ${deal.status}`,
          level:
            deal.status === 'Rejected' || deal.status === 'Disputed'
              ? 'warning'
              : deal.status === 'Completed'
              ? 'success'
              : 'info',
          occurredAt: deal.publishedAtUtc ?? deal.createdAt,
          route: '/deals',
        }));
        mapped.sort((a, b) => new Date(b.occurredAt).getTime() - new Date(a.occurredAt).getTime());
        setNotifications(mapped.slice(0, 100));
        return;
      }

      if (user.role === Role.Publisher) {
        const inbox = await listPublisherInbox(token);
        const mapped = inbox.map<AppNotification>((deal) => ({
          id: `publisher:${deal.dealId}:${deal.status}:${deal.fundingStatus}:${deal.publishedAtUtc ?? ''}`,
          title:
            deal.status === 'PendingPublisherDecision'
              ? tx({ ru: 'Новый заказ', en: 'New order' })
              : deal.status === 'FundingReserved'
              ? tx({ ru: 'Заказ оплачен и зарезервирован', en: 'Order paid and reserved' })
              : deal.status === 'PublishedPendingConfirm'
              ? tx({ ru: 'Ожидается подтверждение рекламодателя', en: 'Awaiting advertiser confirmation' })
              : deal.status === 'Completed'
              ? tx({ ru: 'Сделка завершена', en: 'Deal completed' })
              : tx({ ru: 'Обновление входящей заявки', en: 'Incoming deal update' }),
          body: `${deal.amount.toLocaleString('ru-RU')} ${deal.currency} • ${deal.status}`,
          level:
            deal.status === 'PendingPublisherDecision'
              ? 'warning'
              : deal.status === 'Completed'
              ? 'success'
              : 'info',
          occurredAt: deal.publishedAtUtc ?? deal.createdAt,
          route: '/applications',
        }));
        mapped.sort((a, b) => new Date(b.occurredAt).getTime() - new Date(a.occurredAt).getTime());
        setNotifications(mapped.slice(0, 100));
        return;
      }

      if (user.role === Role.Admin) {
        const disputes = await listAdminDisputes(token, 'all', 100);
        const mapped = disputes.map<AppNotification>((dispute) => ({
          id: `dispute:${dispute.disputeId}:${dispute.disputeStatus}`,
          title:
            dispute.disputeStatus === 'Open'
              ? tx({ ru: 'Новый спор', en: 'New dispute' })
              : tx({ ru: 'Спор решен', en: 'Dispute resolved' }),
          body: `Deal ${dispute.dealId} • ${dispute.reason}`,
          level: dispute.disputeStatus === 'Open' ? 'warning' : 'success',
          occurredAt: dispute.disputeStatus === 'Open'
            ? dispute.disputeCreatedAt
            : dispute.resolvedAt ?? dispute.disputeCreatedAt,
          route: '/deals',
        }));
        mapped.sort((a, b) => new Date(b.occurredAt).getTime() - new Date(a.occurredAt).getTime());
        setNotifications(mapped.slice(0, 100));
        return;
      }

      setNotifications([]);
    } catch {
      // Игнорируем временные ошибки: уведомления не должны ломать основной интерфейс.
    }
  }, [token, tx, user.role]);

  useEffect(() => {
    let cancelled = false;
    const run = async () => {
      if (cancelled) return;
      await fetchNotifications();
    };

    run();
    const timer = window.setInterval(run, POLL_MS);
    return () => {
      cancelled = true;
      window.clearInterval(timer);
    };
  }, [fetchNotifications]);

  const unreadCount = useMemo(
    () => notifications.filter((item) => !readIds.has(item.id)).length,
    [notifications, readIds]
  );

  const markAllAsRead = useCallback(() => {
    const next = new Set(readIds);
    notifications.forEach((item) => next.add(item.id));
    setReadIds(next);
    saveReadIds(user.id, next);
  }, [notifications, readIds, user.id]);

  return {
    notifications,
    unreadCount,
    markAllAsRead,
    refreshNotifications: fetchNotifications,
  };
};
