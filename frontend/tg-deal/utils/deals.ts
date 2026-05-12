export const dealStatusLabel = (status: string) => {
  switch (status) {
    case 'PendingPublisherDecision':
      return 'Ожидает решения владельца';
    case 'Accepted':
      return 'Принята';
    case 'FundingReserved':
      return 'Деньги зарезервированы';
    case 'ReadyToPublish':
      return 'Готова к публикации';
    case 'PublishedPendingConfirm':
      return 'Опубликовано, ждет подтверждения';
    case 'Completed':
      return 'Завершена';
    case 'Rejected':
      return 'Отклонена';
    case 'CanceledByAdvertiser':
      return 'Отменена';
    case 'Disputed':
      return 'Спор';
    case 'Resolved':
      return 'Решена';
    default:
      return status;
  }
};

export const dealFundingLabel = (fundingStatus: string) => {
  switch (fundingStatus) {
    case 'None':
      return 'Нет резерва';
    case 'Reserved':
      return 'В резерве';
    case 'Released':
      return 'Возвращено';
    case 'Captured':
      return 'Списано';
    default:
      return fundingStatus;
  }
};

export const dealStatusBadgeClass = (status: string) => {
  switch (status) {
    case 'Completed':
      return 'bg-green-500/10 text-green-400 border-green-500/20';
    case 'FundingReserved':
    case 'ReadyToPublish':
    case 'Accepted':
      return 'bg-cyan-500/10 text-cyan-400 border-cyan-500/20';
    case 'PendingPublisherDecision':
    case 'PublishedPendingConfirm':
      return 'bg-yellow-500/10 text-yellow-400 border-yellow-500/20';
    case 'Disputed':
      return 'bg-orange-500/10 text-orange-400 border-orange-500/20';
    case 'Resolved':
      return 'bg-blue-500/10 text-blue-400 border-blue-500/20';
    case 'Rejected':
    case 'CanceledByAdvertiser':
      return 'bg-red-500/10 text-red-400 border-red-500/20';
    default:
      return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
  }
};

export const fundingStatusBadgeClass = (fundingStatus: string) => {
  switch (fundingStatus) {
    case 'Captured':
      return 'bg-green-500/10 text-green-400 border-green-500/20';
    case 'Reserved':
      return 'bg-cyan-500/10 text-cyan-400 border-cyan-500/20';
    case 'Released':
      return 'bg-slate-500/10 text-slate-300 border-slate-500/20';
    default:
      return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
  }
};

export const canCancelDeal = (status: string) =>
  !['Completed', 'Rejected', 'CanceledByAdvertiser', 'Resolved'].includes(status);

export const canOpenDispute = (status: string, fundingStatus: string) =>
  !['Disputed', 'Resolved', 'Rejected', 'CanceledByAdvertiser'].includes(status) &&
  (fundingStatus === 'Reserved' || fundingStatus === 'Captured');
