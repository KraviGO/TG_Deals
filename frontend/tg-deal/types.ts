export enum Role {
  Advertiser = 'Advertiser',
  Publisher = 'Publisher',
  Admin = 'Admin',
  Guest = 'Guest'
}

export type DealStatus =
  | 'Created'
  | 'PendingPublisherDecision'
  | 'Accepted'
  | 'FundingReserved'
  | 'ReadyToPublish'
  | 'PublishedPendingConfirm'
  | 'Completed'
  | 'Rejected'
  | 'CanceledByAdvertiser'
  | 'Disputed'
  | 'Resolved';

export type FundingStatus = 'None' | 'Reserved' | 'Released' | 'Captured';

export type IntakeMode = 'ActiveAuto' | 'ActiveManual' | 'Paused';

export type OwnershipStatus =
  | 'PendingVerification'
  | 'Verified'
  | 'Rejected'
  | 'Banned';

export interface User {
  id: string;
  email: string;
  role: Role;
  name?: string;
  avatar?: string;
  balance?: number;
}

export interface Channel {
  id: string;
  name: string;
  description: string;
  subscribers: number;
  pricePerPost: number;
  category: string;
  image: string;
  verified: boolean;
  avgViews: number;
  status?: 'Active' | 'Paused' | 'Under Review';
  err?: number; // Процент вовлеченности канала.
}

export interface Deal {
  id: string;
  channelName: string;
  channelImage: string;
  status: 'In Progress' | 'Pending Approval' | 'Completed' | 'Rejected' | 'Posted';
  startDate?: string;
  endDate: string;
  price: number;
}

export interface Transaction {
  id: string;
  date: string;
  type: 'Deposit' | 'Payment' | 'Withdrawal';
  amount: number;
  details: string;
  status: 'Completed' | 'Pending';
}

export interface Wallet {
  currency: string;
  available: number;
  reserved: number;
  total: number;
}

export interface Application {
  id: string;
  advertiserName: string;
  advertiserImage: string;
  postDate: string;
  price: number;
  status: 'Pending' | 'Accepted' | 'Declined';
  creativePreview: string;
}

export interface StatMetric {
  label: string;
  value: string | number;
  change?: string;
  isPositive?: boolean;
}

export interface CatalogChannel {
  channelId: string;
  telegramChannelId: string;
  title: string;
  topic: string;
  language: string;
  pricePerPostRub: number;
  intakeMode: IntakeMode | string;
  ownershipStatus: OwnershipStatus | string;
}

export interface PublisherChannel {
  channelId: string;
  telegramChannelId: string;
  title: string;
  topic: string;
  language: string;
  pricePerPostRub: number;
  intakeMode: IntakeMode | string;
  ownershipStatus: OwnershipStatus | string;
}

export interface DealSummary {
  dealId: string;
  channelId: string;
  status: DealStatus | string;
  desiredPublishAtUtc: string;
  fundingStatus: FundingStatus | string;
  reservationId?: string;
  amount: number;
  currency: string;
  postText: string;
  postUrl?: string;
  publishedAtUtc?: string;
  createdAt: string;
}

export interface PublisherDeal {
  dealId: string;
  channelId: string;
  advertiserUserId: string;
  status: DealStatus | string;
  fundingStatus: FundingStatus | string;
  reservationId?: string;
  amount: number;
  currency: string;
  postUrl?: string;
  publishedAtUtc?: string;
  desiredPublishAtUtc: string;
  createdAt: string;
}

export interface AdminDealDispute {
  disputeId: string;
  dealId: string;
  disputeStatus: string;
  reason: string;
  openedByUserId: string;
  openedByRole: string;
  disputeCreatedAt: string;
  resolvedByUserId?: string;
  resolutionAction?: string;
  resolutionNote?: string;
  resolvedAt?: string;
  channelId: string;
  advertiserUserId: string;
  publisherUserId: string;
  dealStatus: string;
  fundingStatus: string;
  amount: number;
  currency: string;
  postUrl?: string;
  publishedAtUtc?: string;
}

export interface AppNotification {
  id: string;
  title: string;
  body: string;
  level: 'info' | 'success' | 'warning';
  occurredAt: string;
  route: string;
}

export interface PublisherWalletSummary {
  currency: string;
  available: number;
  paidOut: number;
  totalAccrued: number;
}

export interface PublisherLedgerEntry {
  entryId: string;
  dealId: string;
  grossAmount: number;
  platformFeeAmount: number;
  publisherAmount: number;
  currency: string;
  status: string;
  createdAt: string;
  availableAt?: string;
}
