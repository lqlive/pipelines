import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import {
  Cog6ToothIcon,
  RocketLaunchIcon,
  ShieldCheckIcon,
  GlobeAltIcon,
  KeyIcon,
  TrashIcon,
  ArrowLeftIcon,
} from '@heroicons/react/24/outline';
import type { Repository } from '../types';

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

type TabId = 'general' | 'builds' | 'webhooks' | 'access' | 'environment';

interface Tab {
  id: TabId;
  name: string;
  icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
}

const RepositorySettings: React.FC = () => {
  const { owner, name } = useParams<RouteParams>();
  const [activeTab, setActiveTab] = useState<TabId>('general');
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
    { id: 'general', name: 'General', icon: Cog6ToothIcon },
    { id: 'builds', name: 'Builds', icon: RocketLaunchIcon },
    { id: 'webhooks', name: 'Webhooks', icon: GlobeAltIcon },
    { id: 'access', name: 'Access', icon: ShieldCheckIcon },
    { id: 'environment', name: 'Environment', icon: KeyIcon },
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

  const handleSettingChange = <T extends keyof RepositorySettings>(
    category: T,
    key: keyof RepositorySettings[T],
    value: any
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
                      Repository Description
                    </label>
                    <textarea
                      value={settings.general.description}
                      onChange={(e) => handleSettingChange('general', 'description', e.target.value)}
                      className="minimal-input"
                      rows={3}
                      placeholder="A short description of your repository"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Website URL
                    </label>
                    <input
                      type="url"
                      value={settings.general.website}
                      onChange={(e) => handleSettingChange('general', 'website', e.target.value)}
                      className="minimal-input max-w-md"
                      placeholder="https://example.com"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Topics
                    </label>
                    <input
                      type="text"
                      value={settings.general.topics.join(', ')}
                      onChange={(e) => handleSettingChange('general', 'topics', e.target.value.split(', ').filter(t => t.trim()))}
                      className="minimal-input max-w-md"
                      placeholder="api, backend, nodejs"
                    />
                    <p className="mt-1 text-sm text-gray-500">Separate topics with commas</p>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Visibility
                    </label>
                    <div className="space-y-2">
                      <label className="flex items-center">
                        <input
                          type="radio"
                          name="visibility"
                          value="public"
                          checked={settings.general.visibility === 'public'}
                          onChange={(e) => handleSettingChange('general', 'visibility', e.target.value as 'public' | 'private')}
                          className="mr-2"
                        />
                        <div>
                          <div className="text-sm font-medium">Public</div>
                          <div className="text-sm text-gray-500">Anyone can see this repository</div>
                        </div>
                      </label>
                      <label className="flex items-center">
                        <input
                          type="radio"
                          name="visibility"
                          value="private"
                          checked={settings.general.visibility === 'private'}
                          onChange={(e) => handleSettingChange('general', 'visibility', e.target.value as 'public' | 'private')}
                          className="mr-2"
                        />
                        <div>
                          <div className="text-sm font-medium">Private</div>
                          <div className="text-sm text-gray-500">Only you and collaborators can see this repository</div>
                        </div>
                      </label>
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Default Branch
                    </label>
                    <select
                      value={settings.general.defaultBranch}
                      onChange={(e) => handleSettingChange('general', 'defaultBranch', e.target.value)}
                      className="minimal-input max-w-xs"
                    >
                      <option value="main">main</option>
                      <option value="master">master</option>
                      <option value="develop">develop</option>
                    </select>
                  </div>
                </div>
              </div>
            )}

            {/* Build Settings */}
            {activeTab === 'builds' && (
              <div>
                <h2 className="text-xl font-semibold text-gray-900 mb-6">Build Settings</h2>
                <div className="space-y-6">
                  <div>
                    <label className="flex items-center justify-between">
                      <div>
                        <div className="text-sm font-medium text-gray-900">Enable builds</div>
                        <div className="text-sm text-gray-500">Allow this repository to trigger builds</div>
                      </div>
                      <input
                        type="checkbox"
                        checked={settings.builds.enabled}
                        onChange={(e) => handleSettingChange('builds', 'enabled', e.target.checked)}
                        className="ml-3"
                      />
                    </label>
                  </div>

                  <div>
                    <label className="flex items-center justify-between">
                      <div>
                        <div className="text-sm font-medium text-gray-900">Auto-cancel redundant builds</div>
                        <div className="text-sm text-gray-500">Cancel previous builds when new commits are pushed</div>
                      </div>
                      <input
                        type="checkbox"
                        checked={settings.builds.autoCancel}
                        onChange={(e) => handleSettingChange('builds', 'autoCancel', e.target.checked)}
                        className="ml-3"
                      />
                    </label>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Build timeout (minutes)
                    </label>
                    <input
                      type="number"
                      value={settings.builds.timeout}
                      onChange={(e) => handleSettingChange('builds', 'timeout', parseInt(e.target.value))}
                      className="minimal-input max-w-xs"
                      min="1"
                      max="600"
                    />
                    <p className="mt-1 text-sm text-gray-500">Maximum time a build can run before being cancelled</p>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Concurrent builds
                    </label>
                    <input
                      type="number"
                      value={settings.builds.concurrent}
                      onChange={(e) => handleSettingChange('builds', 'concurrent', parseInt(e.target.value))}
                      className="minimal-input max-w-xs"
                      min="1"
                      max="10"
                    />
                    <p className="mt-1 text-sm text-gray-500">Maximum number of builds that can run simultaneously</p>
                  </div>

                  <div>
                    <label className="flex items-center justify-between">
                      <div>
                        <div className="text-sm font-medium text-gray-900">Protected builds</div>
                        <div className="text-sm text-gray-500">Require status checks to pass before merging</div>
                      </div>
                      <input
                        type="checkbox"
                        checked={settings.builds.protected}
                        onChange={(e) => handleSettingChange('builds', 'protected', e.target.checked)}
                        className="ml-3"
                      />
                    </label>
                  </div>
                </div>
              </div>
            )}

            {/* Webhook Settings */}
            {activeTab === 'webhooks' && (
              <div>
                <h2 className="text-xl font-semibold text-gray-900 mb-6">Webhook Settings</h2>
                <div className="space-y-6">
                  <div>
                    <label className="flex items-center justify-between mb-4">
                      <div>
                        <div className="text-sm font-medium text-gray-900">Enable webhooks</div>
                        <div className="text-sm text-gray-500">Send HTTP requests when events occur</div>
                      </div>
                      <input
                        type="checkbox"
                        checked={settings.webhooks.enabled}
                        onChange={(e) => handleSettingChange('webhooks', 'enabled', e.target.checked)}
                        className="ml-3"
                      />
                    </label>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Payload URL
                    </label>
                    <input
                      type="url"
                      value={settings.webhooks.url}
                      onChange={(e) => handleSettingChange('webhooks', 'url', e.target.value)}
                      className="minimal-input"
                      placeholder="https://api.example.com/webhooks"
                      disabled={!settings.webhooks.enabled}
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Secret Token
                    </label>
                    <input
                      type="password"
                      value={settings.webhooks.secret}
                      onChange={(e) => handleSettingChange('webhooks', 'secret', e.target.value)}
                      className="minimal-input max-w-md"
                      placeholder="Optional secret for payload validation"
                      disabled={!settings.webhooks.enabled}
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Events
                    </label>
                    <div className="space-y-2">
                      {['push', 'pull_request', 'tag', 'release'].map((event) => (
                        <label key={event} className="flex items-center">
                          <input
                            type="checkbox"
                            checked={settings.webhooks.events.includes(event)}
                            onChange={(e) => {
                              const events = e.target.checked
                                ? [...settings.webhooks.events, event]
                                : settings.webhooks.events.filter(e => e !== event);
                              handleSettingChange('webhooks', 'events', events);
                            }}
                            className="mr-2"
                            disabled={!settings.webhooks.enabled}
                          />
                          <span className="text-sm capitalize">{event.replace('_', ' ')}</span>
                        </label>
                      ))}
                    </div>
                  </div>
                </div>
              </div>
            )}

            {/* Access Settings */}
            {activeTab === 'access' && (
              <div>
                <h2 className="text-xl font-semibold text-gray-900 mb-6">Access Control</h2>
                <div className="space-y-6">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Admin Users
                    </label>
                    <textarea
                      value={settings.access.adminUsers.join('\n')}
                      onChange={(e) => handleSettingChange('access', 'adminUsers', e.target.value.split('\n').filter(u => u.trim()))}
                      className="minimal-input"
                      rows={3}
                      placeholder="admin@example.com&#10;admin2@example.com"
                    />
                    <p className="mt-1 text-sm text-gray-500">One email per line. Admin users can modify settings</p>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Write Users
                    </label>
                    <textarea
                      value={settings.access.writeUsers.join('\n')}
                      onChange={(e) => handleSettingChange('access', 'writeUsers', e.target.value.split('\n').filter(u => u.trim()))}
                      className="minimal-input"
                      rows={3}
                      placeholder="dev@example.com&#10;dev2@example.com"
                    />
                    <p className="mt-1 text-sm text-gray-500">Write users can trigger builds and view logs</p>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Read Users
                    </label>
                    <textarea
                      value={settings.access.readUsers.join('\n')}
                      onChange={(e) => handleSettingChange('access', 'readUsers', e.target.value.split('\n').filter(u => u.trim()))}
                      className="minimal-input"
                      rows={3}
                      placeholder="readonly@example.com&#10;guest@example.com"
                    />
                    <p className="mt-1 text-sm text-gray-500">Read users can only view build status and logs</p>
                  </div>
                </div>
              </div>
            )}

            {/* Environment Variables */}
            {activeTab === 'environment' && (
              <div>
                <h2 className="text-xl font-semibold text-gray-900 mb-6">Environment Variables</h2>
                <div className="space-y-4">
                  {settings.environment.variables.map((variable, index) => (
                    <div key={index} className="flex items-center space-x-3 p-3 border border-gray-200 rounded-md">
                      <input
                        type="text"
                        value={variable.key}
                        onChange={(e) => {
                          const newVars = [...settings.environment.variables];
                          newVars[index].key = e.target.value;
                          handleSettingChange('environment', 'variables', newVars);
                        }}
                        className="minimal-input flex-1"
                        placeholder="Variable name"
                      />
                      <input
                        type={variable.secret ? "password" : "text"}
                        value={variable.value}
                        onChange={(e) => {
                          const newVars = [...settings.environment.variables];
                          newVars[index].value = e.target.value;
                          handleSettingChange('environment', 'variables', newVars);
                        }}
                        className="minimal-input flex-1"
                        placeholder="Variable value"
                      />
                      <label className="flex items-center">
                        <input
                          type="checkbox"
                          checked={variable.secret}
                          onChange={(e) => {
                            const newVars = [...settings.environment.variables];
                            newVars[index].secret = e.target.checked;
                            handleSettingChange('environment', 'variables', newVars);
                          }}
                          className="mr-1"
                        />
                        <span className="text-xs">Secret</span>
                      </label>
                      <button
                        onClick={() => {
                          const newVars = settings.environment.variables.filter((_, i) => i !== index);
                          handleSettingChange('environment', 'variables', newVars);
                        }}
                        className="text-red-600 hover:text-red-800"
                      >
                        <TrashIcon className="h-4 w-4" />
                      </button>
                    </div>
                  ))}
                  
                  <button
                    onClick={() => {
                      const newVars = [...settings.environment.variables, { key: '', value: '', secret: false }];
                      handleSettingChange('environment', 'variables', newVars);
                    }}
                    className="btn-secondary"
                  >
                    Add Variable
                  </button>
                </div>
              </div>
            )}

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