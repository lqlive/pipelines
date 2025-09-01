import React from 'react';
import {
  UserIcon,
  CogIcon,
  ShieldCheckIcon,
  BellIcon,
} from '@heroicons/react/24/outline';
import { Routes, Route, NavLink } from 'react-router-dom';
import ProfileTab from './Profile';
import AccountTab from './AccountSettings';
import SecurityTab from './SecuritySettings';
import NotificationTab from './NotificationSettings';
import { useNotification } from '../../contexts/NotificationContext';

interface NavTab {
  name: string;
  path: string;
  icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
}

const UserProfile: React.FC = () => {
  const { showNotification } = useNotification();

  const navTabs: NavTab[] = [
    { name: 'Profile', path: '/profile', icon: UserIcon },
    { name: 'Account', path: '/profile/account', icon: CogIcon },
    { name: 'Security', path: '/profile/security', icon: ShieldCheckIcon },
    { name: 'Notifications', path: '/profile/notifications', icon: BellIcon },
  ];



  // Helper component to wrap tabs with headers
  const TabWithHeader: React.FC<{ 
    title: string; 
    description: string; 
    children: React.ReactNode 
  }> = ({ title, description, children }) => (
    <div>
      <div className="mb-6">
        <h2 className="text-xl font-semibold text-gray-900">{title}</h2>
        <p className="text-sm text-gray-600 mt-1">{description}</p>
      </div>
      {children}
    </div>
  );

  // Tab wrapper components
  const ProfileTabWithHeader = () => (
    <TabWithHeader 
      title="Profile Information" 
      description="Manage your personal information and profile picture"
    >
      <ProfileTab onNotification={showNotification} />
    </TabWithHeader>
  );

  const AccountTabWithHeader = () => (
    <TabWithHeader 
      title="Account Settings" 
      description="Manage your account information and preferences"
    >
      <AccountTab onNotification={showNotification} />
    </TabWithHeader>
  );

  const SecurityTabWithHeader = () => (
    <TabWithHeader 
      title="Security Settings" 
      description="Manage your account security and active sessions"
    >
      <SecurityTab onNotification={showNotification} />
    </TabWithHeader>
  );

  const NotificationTabWithHeader = () => (
    <TabWithHeader 
      title="Notification Settings" 
      description="Choose how you want to be notified about activity"
    >
      <NotificationTab onNotification={showNotification} />
    </TabWithHeader>
  );

  return (
    <div className="fade-in">
      {/* Page Header */}
      <div className="section-header">
        <h1 className="text-xl font-medium text-gray-900">Profile Settings</h1>
        <p className="text-sm text-gray-600">Manage your account information and preferences</p>
      </div>

      <div className="flex flex-col lg:flex-row gap-6">
        {/* Sidebar Navigation */}
        <div className="lg:w-64">
          <div className="card">
            <nav className="space-y-1">
              {navTabs.map((tab) => {
                const Icon = tab.icon;
                return (
                                    <NavLink
                    key={tab.path}
                    to={tab.path}
                    end={tab.path === '/profile'} // 只有 profile 需要 end 匹配
                    className={({ isActive }) =>
                      `w-full flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors ${
                        isActive
                        ? 'bg-gray-100 text-gray-900'
                        : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                      }`
                    }
                  >
                    <Icon className="h-4 w-4 mr-3" />
                    {tab.name}
                  </NavLink>
                );
              })}
            </nav>
          </div>
        </div>

        {/* Main Content */}
        <div className="flex-1">
          <div className="card">
            <Routes>
              <Route index element={<ProfileTabWithHeader />} />
              <Route path="account" element={<AccountTabWithHeader />} />
              <Route path="security" element={<SecurityTabWithHeader />} />
              <Route path="notifications" element={<NotificationTabWithHeader />} />
            </Routes>
          </div>
        </div>
      </div>


    </div>
  );
};

export default UserProfile;