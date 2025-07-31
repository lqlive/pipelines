import React, { useState } from 'react';
import {
  Cog6ToothIcon,
  UserIcon,
  BellIcon,
  ShieldCheckIcon,
  ServerIcon,
} from '@heroicons/react/24/outline';
import type { Settings as SettingsType } from '../types';

type TabId = 'general' | 'notifications' | 'security' | 'server';

interface Tab {
  id: TabId;
  name: string;
  icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
}

const Settings: React.FC = () => {
  const [activeTab, setActiveTab] = useState<TabId>('general');
  const [settings, setSettings] = useState<SettingsType>({
    general: {
      language: 'en-US',
      timezone: 'UTC',
      theme: 'light',
    },
    notifications: {
      emailBuilds: true,
      emailFailures: true,
      emailSuccess: false,
      browserNotifications: true,
    },
    security: {
      twoFactorEnabled: false,
      sessionTimeout: '24',
    },
    server: {
      serverUrl: 'http://localhost:8080',
      maxBuilds: '10',
      buildTimeout: '60',
    }
  });

  const tabs: Tab[] = [
    { id: 'general', name: 'General', icon: Cog6ToothIcon },
    { id: 'notifications', name: 'Notifications', icon: BellIcon },
    { id: 'security', name: 'Security', icon: ShieldCheckIcon },
    { id: 'server', name: 'Server', icon: ServerIcon },
  ];

  const handleSettingChange = <T extends keyof SettingsType>(
    category: T,
    key: keyof SettingsType[T],
    value: SettingsType[T][typeof key]
  ) => {
    setSettings(prev => ({
      ...prev,
      [category]: {
        ...prev[category],
        [key]: value
      }
    }));
  };

  const saveSettings = () => {
    console.log('Save settings:', settings);
    // TODO: Implement actual save logic
  };

  return (
    <div className="fade-in">
      {/* Page Title */}
      <div className="section-header">
        <h1 className="text-xl font-medium text-gray-900">Settings</h1>
        <p className="text-sm text-gray-600">Manage your preferences and configuration</p>
      </div>

      <div className="flex flex-col lg:flex-row gap-6">
        {/* Sidebar Navigation */}
        <div className="lg:w-64">
          <div className="card">
            <nav className="space-y-1">
              {tabs.map((tab) => {
                const Icon = tab.icon;
                return (
                  <button
                    key={tab.id}
                    onClick={() => setActiveTab(tab.id)}
                    className={`w-full flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors ${
                      activeTab === tab.id
                        ? 'bg-gray-100 text-gray-900'
                        : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                    }`}
                  >
                    <Icon className="h-4 w-4 mr-3" />
                    {tab.name}
                  </button>
                );
              })}
            </nav>
          </div>
        </div>

        {/* Main Content */}
        <div className="flex-1">
          <div className="card">
            {/* General Settings */}
            {activeTab === 'general' && (
              <div>
                <h2 className="text-xl font-semibold text-gray-900 mb-6">General Settings</h2>
                <div className="space-y-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Language
                    </label>
                    <select
                      value={settings.general.language}
                      onChange={(e) => handleSettingChange('general', 'language', e.target.value)}
                      className="minimal-input max-w-xs"
                    >
                      <option value="en-US">English (US)</option>
                      <option value="zh-CN">中文 (简体)</option>
                      <option value="zh-TW">中文 (繁體)</option>
                      <option value="ja-JP">日本語</option>
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Timezone
                    </label>
                    <select
                      value={settings.general.timezone}
                      onChange={(e) => handleSettingChange('general', 'timezone', e.target.value)}
                      className="minimal-input max-w-xs"
                    >
                      <option value="UTC">UTC</option>
                      <option value="America/New_York">America/New York</option>
                      <option value="Europe/London">Europe/London</option>
                      <option value="Asia/Shanghai">Asia/Shanghai</option>
                      <option value="Asia/Tokyo">Asia/Tokyo</option>
                    </select>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Theme
                    </label>
                    <div className="space-y-2">
                      <label className="flex items-center">
                        <input
                          type="radio"
                          name="theme"
                          value="light"
                          checked={settings.general.theme === 'light'}
                          onChange={(e) => handleSettingChange('general', 'theme', e.target.value as 'light' | 'dark' | 'system')}
                          className="mr-2"
                        />
                        Light theme
                      </label>
                      <label className="flex items-center">
                        <input
                          type="radio"
                          name="theme"
                          value="dark"
                          checked={settings.general.theme === 'dark'}
                          onChange={(e) => handleSettingChange('general', 'theme', e.target.value as 'light' | 'dark' | 'system')}
                          className="mr-2"
                        />
                        Dark theme
                      </label>
                      <label className="flex items-center">
                        <input
                          type="radio"
                          name="theme"
                          value="system"
                          checked={settings.general.theme === 'system'}
                          onChange={(e) => handleSettingChange('general', 'theme', e.target.value as 'light' | 'dark' | 'system')}
                          className="mr-2"
                        />
                        Follow system
                      </label>
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Notification Settings */}
            {activeTab === 'notifications' && (
              <div>
                <h2 className="text-xl font-semibold text-gray-900 mb-6">Notification Settings</h2>
                <div className="space-y-6">
                  <div>
                    <h3 className="text-lg font-medium text-gray-900 mb-4">Email Notifications</h3>
                    <div className="space-y-3">
                      <label className="flex items-center">
                        <input
                          type="checkbox"
                          checked={settings.notifications.emailBuilds}
                          onChange={(e) => handleSettingChange('notifications', 'emailBuilds', e.target.checked)}
                          className="mr-3"
                        />
                        <div>
                          <div className="text-sm font-medium text-gray-900">All builds</div>
                          <div className="text-sm text-gray-500">Receive email notifications for all builds</div>
                        </div>
                      </label>
                      <label className="flex items-center">
                        <input
                          type="checkbox"
                          checked={settings.notifications.emailFailures}
                          onChange={(e) => handleSettingChange('notifications', 'emailFailures', e.target.checked)}
                          className="mr-3"
                        />
                        <div>
                          <div className="text-sm font-medium text-gray-900">Build failures</div>
                          <div className="text-sm text-gray-500">Send email only when builds fail</div>
                        </div>
                      </label>
                      <label className="flex items-center">
                        <input
                          type="checkbox"
                          checked={settings.notifications.emailSuccess}
                          onChange={(e) => handleSettingChange('notifications', 'emailSuccess', e.target.checked)}
                          className="mr-3"
                        />
                        <div>
                          <div className="text-sm font-medium text-gray-900">Build success</div>
                          <div className="text-sm text-gray-500">Send email only when builds succeed</div>
                        </div>
                      </label>
                    </div>
                  </div>

                  <div>
                    <h3 className="text-lg font-medium text-gray-900 mb-4">Browser Notifications</h3>
                    <label className="flex items-center">
                      <input
                        type="checkbox"
                        checked={settings.notifications.browserNotifications}
                        onChange={(e) => handleSettingChange('notifications', 'browserNotifications', e.target.checked)}
                        className="mr-3"
                      />
                      <div>
                        <div className="text-sm font-medium text-gray-900">Enable browser notifications</div>
                        <div className="text-sm text-gray-500">Show build status notifications in browser</div>
                      </div>
                    </label>
                  </div>
                </div>
              </div>
            )}

            {/* Security Settings */}
            {activeTab === 'security' && (
              <div>
                <h2 className="text-xl font-semibold text-gray-900 mb-6">Security Settings</h2>
                <div className="space-y-6">
                  <div>
                    <h3 className="text-lg font-medium text-gray-900 mb-4">Account Security</h3>
                    <div className="space-y-4">
                      <label className="flex items-center justify-between">
                        <div>
                          <div className="text-sm font-medium text-gray-900">Two-factor authentication</div>
                          <div className="text-sm text-gray-500">Add an extra layer of security to your account</div>
                        </div>
                        <input
                          type="checkbox"
                          checked={settings.security.twoFactorEnabled}
                          onChange={(e) => handleSettingChange('security', 'twoFactorEnabled', e.target.checked)}
                          className="ml-3"
                        />
                      </label>
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Session timeout (hours)
                    </label>
                    <select
                      value={settings.security.sessionTimeout}
                      onChange={(e) => handleSettingChange('security', 'sessionTimeout', e.target.value)}
                      className="minimal-input max-w-xs"
                    >
                      <option value="1">1 hour</option>
                      <option value="8">8 hours</option>
                      <option value="24">24 hours</option>
                      <option value="168">7 days</option>
                      <option value="720">30 days</option>
                    </select>
                  </div>

                  <div className="pt-4 border-t border-gray-200">
                    <button className="btn-danger">
                      Log out all other sessions
                    </button>
                    <p className="mt-2 text-sm text-gray-500">
                      This will force logout all your sessions on other devices
                    </p>
                  </div>
                </div>
              </div>
            )}

            {/* Server Settings */}
            {activeTab === 'server' && (
              <div>
                <h2 className="text-xl font-semibold text-gray-900 mb-6">Server Settings</h2>
                <div className="space-y-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Server URL
                    </label>
                    <input
                      type="url"
                      value={settings.server.serverUrl}
                      onChange={(e) => handleSettingChange('server', 'serverUrl', e.target.value)}
                      className="minimal-input max-w-md"
                      placeholder="http://localhost:8080"
                    />
                    <p className="mt-1 text-sm text-gray-500">
                      Pipelines server URL address
                    </p>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Max concurrent builds
                    </label>
                    <input
                      type="number"
                      value={settings.server.maxBuilds}
                      onChange={(e) => handleSettingChange('server', 'maxBuilds', e.target.value)}
                      className="minimal-input max-w-xs"
                      min="1"
                      max="50"
                    />
                    <p className="mt-1 text-sm text-gray-500">
                      Maximum number of concurrent builds
                    </p>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Build timeout in minutes
                    </label>
                    <input
                      type="number"
                      value={settings.server.buildTimeout}
                      onChange={(e) => handleSettingChange('server', 'buildTimeout', e.target.value)}
                      className="minimal-input max-w-xs"
                      min="1"
                      max="600"
                    />
                    <p className="mt-1 text-sm text-gray-500">
                      Maximum execution time for build tasks
                    </p>
                  </div>

                  <div className="pt-4 border-t border-gray-200">
                    <button className="btn-secondary mr-3">
                      Test Connection
                    </button>
                    <button className="btn-secondary">
                      Reset to defaults
                    </button>
                  </div>
                </div>
              </div>
            )}

            {/* Save Button */}
            <div className="mt-8 pt-6 border-t border-gray-200">
              <div className="flex justify-end space-x-3">
                <button className="btn-secondary">
                  Reset
                </button>
                <button onClick={saveSettings} className="btn-primary">
                  Save Settings
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Settings; 