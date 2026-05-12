import { Channel, Deal, Role, User, Transaction, Application } from './types';

export const MOCK_USER: User = {
  id: 'u1',
  name: 'Mikael',
  email: 'mikael@example.com',
  avatar: 'https://picsum.photos/id/64/100/100',
  balance: 1234.56,
  role: Role.Advertiser, 
};

export const CHANNELS: Channel[] = [
  {
    id: 'c1',
    name: 'Tech Innovators',
    description: 'The latest news and insights in the world of technology and startups.',
    subscribers: 125000,
    pricePerPost: 250,
    category: 'IT & Technology',
    image: 'https://picsum.photos/id/0/200/200',
    verified: true,
    avgViews: 15200,
    status: 'Active',
    err: 12.5
  },
  {
    id: 'c2',
    name: 'Crypto Pulse',
    description: 'Your daily source for cryptocurrency news, analysis, and market trends.',
    subscribers: 88000,
    pricePerPost: 180,
    category: 'Business & Finance',
    image: 'https://picsum.photos/id/1/200/200',
    verified: true,
    avgViews: 9800,
    status: 'Paused',
    err: 8.2
  },
  {
    id: 'c3',
    name: 'Global News Desk',
    description: 'Unbiased breaking news and in-depth analysis from around the globe.',
    subscribers: 1200000,
    pricePerPost: 2200,
    category: 'News & Media',
    image: 'https://picsum.photos/id/2/200/200',
    verified: true,
    avgViews: 450000,
    status: 'Active',
    err: 15.0
  },
  {
    id: 'c4',
    name: 'Startup Growth Hub',
    description: 'Actionable tips on marketing, sales, and scaling your startup.',
    subscribers: 155000,
    pricePerPost: 350,
    category: 'Business & Finance',
    image: 'https://picsum.photos/id/3/200/200',
    verified: false,
    avgViews: 22000,
    status: 'Active',
    err: 10.1
  },
  {
    id: 'c5',
    name: 'Design Finds',
    description: 'Daily inspiration for UI/UX designers and creative professionals.',
    subscribers: 510000,
    pricePerPost: 800,
    category: 'Design & Art',
    image: 'https://picsum.photos/id/4/200/200',
    verified: true,
    avgViews: 95000,
    status: 'Active',
    err: 18.5
  },
  {
    id: 'c6',
    name: 'Gamer\'s Hub',
    description: 'Reviews, gameplay clips, and news from the gaming industry.',
    subscribers: 480000,
    pricePerPost: 650,
    category: 'Entertainment',
    image: 'https://picsum.photos/id/5/200/200',
    verified: true,
    avgViews: 88000,
    status: 'Under Review',
    err: 0
  },
];

export const ACTIVE_DEALS: Deal[] = [
  {
    id: 'd1',
    channelName: 'Tech Innovations',
    channelImage: 'https://picsum.photos/id/0/200/200',
    status: 'In Progress',
    startDate: '2024-10-01',
    endDate: '25 Dec, 2024',
    price: 250,
  },
  {
    id: 'd2',
    channelName: 'Crypto Pulse',
    channelImage: 'https://picsum.photos/id/1/200/200',
    status: 'Pending Approval',
    startDate: '2024-10-05',
    endDate: '18 Jan, 2025',
    price: 180,
  },
  {
    id: 'd3',
    channelName: 'Gamer\'s Hub',
    channelImage: 'https://picsum.photos/id/5/200/200',
    status: 'Completed',
    startDate: '2023-09-15',
    endDate: '05 Jan, 2025',
    price: 650,
  },
  {
    id: 'd4',
    channelName: 'Design Finds',
    channelImage: 'https://picsum.photos/id/4/200/200',
    status: 'Rejected',
    startDate: '2023-08-10',
    endDate: '12 Aug, 2023',
    price: 800,
  },
];

export const MOCK_TRANSACTIONS: Transaction[] = [
  { id: 't1', date: 'Oct 26, 2023', type: 'Deposit', amount: 100.00, details: 'Deposit via Credit Card', status: 'Completed' },
  { id: 't2', date: 'Oct 25, 2023', type: 'Payment', amount: -45.50, details: 'Payment for \'Summer Sale\' Campaign', status: 'Completed' },
  { id: 't3', date: 'Oct 24, 2023', type: 'Payment', amount: -75.00, details: 'Payment for \'New Arrivals\' Campaign', status: 'Completed' },
  { id: 't4', date: 'Oct 22, 2023', type: 'Deposit', amount: 250.00, details: 'Deposit via PayPal', status: 'Completed' },
  { id: 't5', date: 'Oct 20, 2023', type: 'Payment', amount: -112.25, details: 'Payment for \'Q4 Promo\' Campaign', status: 'Completed' },
  { id: 't6', date: 'Oct 19, 2023', type: 'Withdrawal', amount: -500.00, details: 'Withdrawal to Bank Account', status: 'Completed' },
];

export const MOCK_APPLICATIONS: Application[] = [
  { id: 'a1', advertiserName: 'AdCorp Inc.', advertiserImage: 'https://picsum.photos/id/60/100/100', postDate: 'Oct 26, 2024 - 18:00', price: 250.00, status: 'Pending', creativePreview: 'https://picsum.photos/id/100/50/50' },
  { id: 'a2', advertiserName: 'Gamer\'s Hub', advertiserImage: 'https://picsum.photos/id/61/100/100', postDate: 'Oct 27, 2024 - 12:00', price: 180.00, status: 'Pending', creativePreview: 'https://picsum.photos/id/101/50/50' },
  { id: 'a3', advertiserName: 'Fashionista Boutique', advertiserImage: 'https://picsum.photos/id/62/100/100', postDate: 'Oct 28, 2024 - 20:00', price: 300.00, status: 'Pending', creativePreview: 'https://picsum.photos/id/102/50/50' },
];
