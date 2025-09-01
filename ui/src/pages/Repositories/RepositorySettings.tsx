import React, { useState, useEffect } from 'react';
import { useParams, Link, Routes, Route, NavLink, Navigate } from 'react-router-dom';
import {
  Cog6ToothIcon,
  RocketLaunchIcon,
  ShieldCheckIcon,
  GlobeAltIcon,
  KeyIcon,
  TrashIcon,
  ArrowLeftIcon,
} from '@heroicons/react/24/outline';
import type { Repository } from '../../types';
import { General, Builds, Webhooks, Access, Environment } from './Settings';

interface RouteParams extends Record<string, string | undefined> {
  owner: string;
  name: string;
}

interface RepositorySettings {
  general: {
    description: string;
    website: string;
    topics: string[];
    visibility: 'public' | 'private';
    defaultBranch: string;
  };
  builds: {
    enabled: boolean;
    autoCancel: boolean;
    timeout: number;
    concurrent: number;
    protected: boolean;
  };
  webhooks: {
    url: string;
    events: string[];
    secret: string;
    enabled: boolean;
  };
  access: {
    adminUsers: string[];
    writeUsers: string[];
    readUsers: string[];
  };
  environment: {
    variables: Array<{ key: string; value: string; secret: boolean }>;
  };
}

interface Tab {
  path: string;
  name: string;
  icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
}

const RepositorySettings: React.FC = () => {
  const { owner, name } = useParams<RouteParams>();
  const [loading, setLoading] = useState(true);
  const [repository, setRepository] = useState<Repository | null>(null);
  const [settings, setSettings] = useState<RepositorySettings>({
    general: {
      description: 'Backend API service providing core business logic and data interfaces',
      website: 'https://example.com',
      topics: ['api', 'backend', 'nodejs'],
      visibility: 'public',
      defaultBranch: 'main',
    },
    builds: {
      enabled: true,
      autoCancel: true,
      timeout: 60,
      concurrent: 3,
      protected: true,
    },
    webhooks: {
      url: 'https://api.example.com/webhooks/builds',
      events: ['push', 'pull_request', 'tag'],
      secret: '',
      enabled: true,
    },
    access: {
      adminUsers: ['admin@example.com'],
      writeUsers: ['dev@example.com'],
      readUsers: ['readonly@example.com'],
    },
    environment: {
      variables: [
        { key: 'NODE_ENV', value: 'production', secret: false },
        { key: 'API_KEY', value: '***', secret: true },
        { key: 'DATABASE_URL', value: '***', secret: true },
      ],
    },
  });

  const tabs: Tab[] = [
    { path: `/repositories/${owner}/${name}/settings`, name: 'General', icon: Cog6ToothIcon },
    { path: `/repositories/${owner}/${name}/settings/builds`, name: 'Builds', icon: RocketLaunchIcon },
    { path: `/repositories/${owner}/${name}/settings/webhooks`, name: 'Webhooks', icon: GlobeAltIcon },
    { path: `/repositories/${owner}/${name}/settings/access`, name: 'Access', icon: ShieldCheckIcon },
    { path: `/repositories/${owner}/${name}/settings/environment`, name: 'Environment', icon: KeyIcon },
  ];

  useEffect(() => {
    if (!owner || !name) return;
    
    // Mock API call
    setLoading(true);
    setTimeout(() => {
      setRepository({
        owner,
        name,
        description: settings.general.description,
        private: settings.general.visibility === 'private',
        active: settings.builds.enabled,
        defaultBranch: settings.general.defaultBranch,
        branchCount: 12,
        lastActivity: '2024-01-15T10:30:00Z',
        cloneUrl: `https://github.com/${owner}/${name}.git`,
        branches: ['main', 'develop', 'feature/auth'],
        builds: [],
      });
      setLoading(false);
    }, 1000);
  }, [owner, name, settings.general.description, settings.general.visibility, settings.builds.enabled, settings.general.defaultBranch]);

  const handleGeneralChange = (key: keyof RepositorySettings['general'], value: any) => {
    setSettings(prev => ({
      ...prev,
      general: {
        ...prev.general,
        [key]: value
      }
    }));
  };

  const handleBuildsChange = (key: keyof RepositorySettings['builds'], value: any) => {
    setSettings(prev => ({
      ...prev,
      builds: {
        ...prev.builds,
        [key]: value
      }
    }));
  };

  const handleWebhooksChange = (key: keyof RepositorySettings['webhooks'], value: any) => {
    setSettings(prev => ({
      ...prev,
      webhooks: {
        ...prev.webhooks,
        [key]: value
      }
    }));
  };

  const handleAccessChange = (key: keyof RepositorySettings['access'], value: any) => {
    setSettings(prev => ({
      ...prev,
      access: {
        ...prev.access,
        [key]: value
      }
    }));
  };

  const handleEnvironmentChange = (key: keyof RepositorySettings['environment'], value: any) => {
    setSettings(prev => ({
      ...prev,
      environment: {
        ...prev.environment,
        [key]: value
      }
    }));
  };

  const saveSettings = () => {
    console.log('Save repository settings:', settings);
    // TODO: Implement actual save logic
  };

  const deleteRepository = () => {
    if (confirm(`Are you sure you want to delete ${owner}/${name}? This action cannot be undone.`)) {
      console.log('Delete repository:', owner, name);
      // TODO: Implement delete logic
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
      </div>
    );
  }

  if (!repository) {
    return (
      <div className="text-center py-12">
        <h3 className="text-lg font-medium text-gray-900 mb-2">Repository not found</h3>
        <p className="text-gray-600">Please check if the repository name is correct</p>
      </div>
    );
  }

  return (
    <div className="fade-in">
      {/* Page Header */}
      <div className="section-header">
        <div className="flex items-center justify-between">
          <div>
            <div className="flex items-center mb-2">
              <Link 
                to={`/repositories/${owner}/${name}`}
                className="flex items-center text-gray-500 hover:text-gray-900 mr-3"
              >
                <ArrowLeftIcon className="h-4 w-4 mr-1" />
                Back
              </Link>
              <h1 className="text-xl font-medium text-gray-900">
                {owner}/{name} Settings
              </h1>
            </div>
            <p className="text-sm text-gray-600">Manage repository configuration and settings</p>
          </div>
        </div>
      </div>

      <div className="flex flex-col lg:flex-row gap-6">
        {/* Sidebar Navigation */}
        <div className="lg:w-64">
          <div className="card">
            <nav className="space-y-1">
              {tabs.map((tab) => {
                const Icon = tab.icon;
                return (
                  <NavLink
                    key={tab.path}
                    to={tab.path}
                    end={tab.path.endsWith('/settings')}
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
              <Route index element={
                <General
                  settings={settings.general}
                  onSettingChange={handleGeneralChange}
                />
              } />
              <Route path="builds" element={
                <Builds
                  settings={settings.builds}
                  onSettingChange={handleBuildsChange}
                />
              } />
              <Route path="webhooks" element={
                <Webhooks
                  settings={settings.webhooks}
                  onSettingChange={handleWebhooksChange}
                />
              } />
              <Route path="access" element={
                <Access
                  settings={settings.access}
                  onSettingChange={handleAccessChange}
                />
              } />
              <Route path="environment" element={
                <Environment
                  settings={settings.environment}
                  onSettingChange={handleEnvironmentChange}
                />
              } />
            </Routes>

            {/* Save Button */}
            <div className="mt-8 pt-6 border-t border-gray-200">
              <div className="flex justify-between">
                <button 
                  onClick={deleteRepository}
                  className="btn-danger"
                >
                  <TrashIcon className="h-4 w-4 mr-2" />
                  Delete Repository
                </button>
                
                <div className="flex space-x-3">
                  <button className="btn-secondary">
                    Reset
                  </button>
                  <button onClick={saveSettings} className="btn-primary">
                    Save Changes
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default RepositorySettings; 