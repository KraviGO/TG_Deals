
import React from 'react';
import { User, Role } from '../types';
import AdvertiserDashboard from '../components/dashboard/AdvertiserDashboard';
import OwnerDashboard from '../components/dashboard/OwnerDashboard';
import AdminDashboard from '../components/dashboard/AdminDashboard';

interface DashboardProps {
  user: User;
  token: string;
}

const Dashboard: React.FC<DashboardProps> = ({ user, token }) => {
  if (user.role === Role.Advertiser) return <AdvertiserDashboard user={user} token={token} />;
  if (user.role === Role.Admin) return <AdminDashboard />;
  return <OwnerDashboard token={token} />;
};

export default Dashboard;
