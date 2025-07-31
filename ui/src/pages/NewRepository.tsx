import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  FolderIcon,
  GlobeAltIcon,
  LockClosedIcon,
  MagnifyingGlassIcon,
  CheckCircleIcon,
  PlusIcon,
  ArrowLeftIcon,
  ExclamationCircleIcon,
  CloudIcon,
  ArrowPathIcon,
} from '@heroicons/react/24/outline';

interface GitProvider {
  id: string;
  name: string;
  icon: string;
  connected: boolean;
  description: string;
  repositoryCount?: number;
}

interface RemoteRepository {
  id: string;
  name: string;
  fullName: string;
  description: string;
  private: boolean;
  language: string | null;
  stars: number;
  forks: number;
  lastUpdated: string;
  defaultBranch: string;
  cloneUrl: string;
  enabled: boolean;
  provider: string;
}

const NewRepository: React.FC = () => {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [syncLoading, setSyncLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedProvider, setSelectedProvider] = useState<string>('github');
  const [repositories, setRepositories] = useState<RemoteRepository[]>([]);
  const [filteredRepositories, setFilteredRepositories] = useState<RemoteRepository[]>([]);
  const [selectedRepos, setSelectedRepos] = useState<Set<string>>(new Set());

  const gitProviders: GitProvider[] = [
    {
      id: 'github',
      name: 'GitHub',
      icon: '🐙',
      connected: true,
      description: 'Import repositories from GitHub',
      repositoryCount: 247,
    },
    {
      id: 'gitlab',
      name: 'GitLab',
      icon: '🦊',
      connected: false,
      description: 'Import repositories from GitLab',
    },
    {
      id: 'bitbucket',
      name: 'Bitbucket',
      icon: '🪣',
      connected: false,
      description: 'Import repositories from Bitbucket',
    },
    {
      id: 'gitea',
      name: 'Gitea',
      icon: '🍃',
      connected: false,
      description: 'Import repositories from Gitea',
    },
  ];

  // Mock repository data
  const mockRepositories: RemoteRepository[] = [
    {
      id: '1',
      name: 'backend-api',
      fullName: 'myorg/backend-api',
      description: 'Backend API service providing core business logic',
      private: false,
      language: 'TypeScript',
      stars: 42,
      forks: 12,
      lastUpdated: '2024-01-15T10:30:00Z',
      defaultBranch: 'main',
      cloneUrl: 'https://github.com/myorg/backend-api.git',
      enabled: true,
      provider: 'github',
    },
    {
      id: '2',
      name: 'frontend-app',
      fullName: 'myorg/frontend-app',
      description: 'React frontend application with modern UI',
      private: false,
      language: 'JavaScript',
      stars: 38,
      forks: 8,
      lastUpdated: '2024-01-14T15:20:00Z',
      defaultBranch: 'main',
      cloneUrl: 'https://github.com/myorg/frontend-app.git',
      enabled: false,
      provider: 'github',
    },
    {
      id: '3',
      name: 'mobile-app',
      fullName: 'myorg/mobile-app',
      description: 'React Native mobile application',
      private: true,
      language: 'TypeScript',
      stars: 15,
      forks: 3,
      lastUpdated: '2024-01-13T09:45:00Z',
      defaultBranch: 'develop',
      cloneUrl: 'https://github.com/myorg/mobile-app.git',
      enabled: false,
      provider: 'github',
    },
    {
      id: '4',
      name: 'data-processing',
      fullName: 'myorg/data-processing',
      description: 'Python data processing and analytics pipeline',
      private: false,
      language: 'Python',
      stars: 67,
      forks: 23,
      lastUpdated: '2024-01-12T14:10:00Z',
      defaultBranch: 'main',
      cloneUrl: 'https://github.com/myorg/data-processing.git',
      enabled: false,
      provider: 'github',
    },
    {
      id: '5',
      name: 'devops-tools',
      fullName: 'myorg/devops-tools',
      description: 'Collection of DevOps utilities and scripts',
      private: true,
      language: 'Shell',
      stars: 28,
      forks: 7,
      lastUpdated: '2024-01-11T16:30:00Z',
      defaultBranch: 'main',
      cloneUrl: 'https://github.com/myorg/devops-tools.git',
      enabled: false,
      provider: 'github',
    },
    {
      id: '6',
      name: 'microservice-auth',
      fullName: 'myorg/microservice-auth',
      description: 'Authentication microservice with JWT support',
      private: false,
      language: 'Go',
      stars: 91,
      forks: 34,
      lastUpdated: '2024-01-10T11:20:00Z',
      defaultBranch: 'main',
      cloneUrl: 'https://github.com/myorg/microservice-auth.git',
      enabled: false,
      provider: 'github',
    },
  ];

  useEffect(() => {
    // Load repositories when component mounts or provider changes
    loadRepositories();
  }, [selectedProvider]);

  useEffect(() => {
    // Filter repositories based on search term
    const filtered = repositories.filter(repo =>
      repo.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      repo.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (repo.language && repo.language.toLowerCase().includes(searchTerm.toLowerCase()))
    );
    setFilteredRepositories(filtered);
  }, [repositories, searchTerm]);

  const loadRepositories = async () => {
    setLoading(true);
    try {
      // Mock API call to fetch repositories from selected provider
      await new Promise(resolve => setTimeout(resolve, 1000));
      setRepositories(mockRepositories.filter(repo => repo.provider === selectedProvider));
    } catch (error) {
      console.error('Failed to load repositories:', error);
    } finally {
      setLoading(false);
    }
  };

  const syncRepositories = async () => {
    setSyncLoading(true);
    try {
      // Mock API call to sync repositories from provider
      await new Promise(resolve => setTimeout(resolve, 2000));
      await loadRepositories();
    } catch (error) {
      console.error('Failed to sync repositories:', error);
    } finally {
      setSyncLoading(false);
    }
  };

  const toggleRepositorySelection = (repoId: string) => {
    const newSelected = new Set(selectedRepos);
    if (newSelected.has(repoId)) {
      newSelected.delete(repoId);
    } else {
      newSelected.add(repoId);
    }
    setSelectedRepos(newSelected);
  };

  const enableRepository = async (repoId: string) => {
    setLoading(true);
    try {
      // Mock API call to enable repository
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      setRepositories(prev => 
        prev.map(repo => 
          repo.id === repoId ? { ...repo, enabled: true } : repo
        )
      );
      
      const repo = repositories.find(r => r.id === repoId);
      if (repo) {
        navigate(`/repositories/${repo.fullName.split('/')[0]}/${repo.fullName.split('/')[1]}`);
      }
    } catch (error) {
      console.error('Failed to enable repository:', error);
    } finally {
      setLoading(false);
    }
  };

  const enableSelectedRepositories = async () => {
    if (selectedRepos.size === 0) return;
    
    setLoading(true);
    try {
      // Mock API call to enable multiple repositories
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      setRepositories(prev => 
        prev.map(repo => 
          selectedRepos.has(repo.id) ? { ...repo, enabled: true } : repo
        )
      );
      
      setSelectedRepos(new Set());
      navigate('/repositories');
    } catch (error) {
      console.error('Failed to enable repositories:', error);
    } finally {
      setLoading(false);
    }
  };

  const selectedProvider_obj = gitProviders.find(p => p.id === selectedProvider);
  const enabledCount = repositories.filter(repo => repo.enabled).length;
  const availableCount = repositories.filter(repo => !repo.enabled).length;

  return (
    <div className="fade-in">
      {/* Page Header */}
      <div className="section-header">
        <div className="flex items-center justify-between">
          <div>
            <div className="flex items-center mb-2">
              <button 
                onClick={() => navigate('/repositories')}
                className="flex items-center text-gray-500 hover:text-gray-900 mr-3"
              >
                <ArrowLeftIcon className="h-4 w-4 mr-1" />
                Back
              </button>
              <h1 className="text-xl font-medium text-gray-900">
                Import Repositories
              </h1>
            </div>
            <p className="text-sm text-gray-600">
              Import repositories from your Git providers to enable CI/CD
            </p>
          </div>
          
          <button
            onClick={syncRepositories}
            disabled={syncLoading || !selectedProvider_obj?.connected}
            className="btn-secondary disabled:opacity-50"
          >
            {syncLoading ? (
              <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-gray-600 mr-2"></div>
            ) : (
                             <ArrowPathIcon className="h-4 w-4 mr-2" />
            )}
            Sync Repositories
          </button>
        </div>
      </div>

      <div className="max-w-6xl">
        <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
          {/* Provider Selection */}
          <div className="lg:col-span-1">
            <div className="card">
              <h2 className="text-lg font-semibold text-gray-900 mb-4">Git Providers</h2>
              
              <div className="space-y-3">
                {gitProviders.map((provider) => (
                  <button
                    key={provider.id}
                    onClick={() => setSelectedProvider(provider.id)}
                    disabled={!provider.connected}
                    className={`w-full text-left p-3 border rounded-md transition-colors ${
                      selectedProvider === provider.id
                        ? 'border-blue-500 bg-blue-50'
                        : provider.connected
                        ? 'border-gray-200 hover:bg-gray-50'
                        : 'border-gray-100 bg-gray-50 cursor-not-allowed'
                    }`}
                  >
                    <div className="flex items-center justify-between">
                      <div className="flex items-center">
                        <span className="text-lg mr-2">{provider.icon}</span>
                        <div>
                          <div className="font-medium">{provider.name}</div>
                          {provider.connected && provider.repositoryCount && (
                            <div className="text-xs text-gray-500">
                              {provider.repositoryCount} repositories
                            </div>
                          )}
                        </div>
                      </div>
                      {provider.connected ? (
                        <CheckCircleIcon className="h-4 w-4 text-green-500" />
                      ) : (
                        <CloudIcon className="h-4 w-4 text-gray-400" />
                      )}
                    </div>
                  </button>
                ))}
              </div>
              
              {!selectedProvider_obj?.connected && (
                <div className="mt-4 p-3 bg-yellow-50 border border-yellow-200 rounded-md">
                  <p className="text-sm text-yellow-800">
                    Connect to {selectedProvider_obj?.name} to import repositories.
                  </p>
                  <button className="mt-2 btn-primary text-sm">
                    Connect {selectedProvider_obj?.name}
                  </button>
                </div>
              )}
            </div>
          </div>

          {/* Repository List */}
          <div className="lg:col-span-3">
            <div className="card">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold text-gray-900">
                  {selectedProvider_obj?.name} Repositories
                </h2>
                {selectedRepos.size > 0 && (
                  <button
                    onClick={enableSelectedRepositories}
                    disabled={loading}
                    className="btn-primary"
                  >
                    {loading ? (
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                    ) : (
                      <PlusIcon className="h-4 w-4 mr-2" />
                    )}
                    Enable {selectedRepos.size} Repository{selectedRepos.size > 1 ? 'ies' : 'y'}
                  </button>
                )}
              </div>

              {/* Stats */}
              <div className="flex items-center space-x-6 mb-4 text-sm text-gray-600">
                <span>{repositories.length} total</span>
                <span>{enabledCount} enabled</span>
                <span>{availableCount} available</span>
              </div>

              {/* Search */}
              <div className="relative mb-4">
                <MagnifyingGlassIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
                <input
                  type="text"
                  placeholder="Search repositories..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="minimal-input pl-10"
                />
              </div>

              {/* Repository List */}
              {loading ? (
                <div className="flex items-center justify-center h-32">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
                </div>
              ) : filteredRepositories.length === 0 ? (
                <div className="text-center py-8">
                  <FolderIcon className="h-12 w-12 text-gray-400 mx-auto mb-3" />
                  <h3 className="text-lg font-medium text-gray-900 mb-2">No repositories found</h3>
                  <p className="text-gray-600">
                    {searchTerm 
                      ? 'Try adjusting your search terms'
                      : selectedProvider_obj?.connected 
                        ? 'No repositories available for import'
                        : `Connect to ${selectedProvider_obj?.name} to view repositories`
                    }
                  </p>
                </div>
              ) : (
                <div className="space-y-3">
                  {filteredRepositories.map((repo) => (
                    <div
                      key={repo.id}
                      className={`p-4 border rounded-md transition-colors ${
                        repo.enabled 
                          ? 'border-green-200 bg-green-50'
                          : selectedRepos.has(repo.id)
                          ? 'border-blue-200 bg-blue-50'
                          : 'border-gray-200 hover:bg-gray-50'
                      }`}
                    >
                      <div className="flex items-center justify-between">
                        <div className="flex items-start flex-1">
                          {!repo.enabled && (
                            <input
                              type="checkbox"
                              checked={selectedRepos.has(repo.id)}
                              onChange={() => toggleRepositorySelection(repo.id)}
                              className="mt-1 mr-3"
                            />
                          )}
                          
                          <div className="flex-1">
                            <div className="flex items-center">
                              <h3 className="font-medium text-gray-900">{repo.name}</h3>
                              {repo.private && (
                                <LockClosedIcon className="h-4 w-4 text-gray-400 ml-2" />
                              )}
                              {repo.enabled && (
                                <span className="ml-2 inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-green-100 text-green-800">
                                  <CheckCircleIcon className="h-3 w-3 mr-1" />
                                  Enabled
                                </span>
                              )}
                            </div>
                            
                            <p className="text-sm text-gray-600 mt-1">
                              {repo.description || 'No description available'}
                            </p>
                            
                            <div className="flex items-center mt-2 text-xs text-gray-500 space-x-4">
                              {repo.language && (
                                <span className="flex items-center">
                                  <span className="w-2 h-2 rounded-full bg-blue-500 mr-1"></span>
                                  {repo.language}
                                </span>
                              )}
                              <span>⭐ {repo.stars}</span>
                              <span>🍴 {repo.forks}</span>
                              <span>Updated {new Date(repo.lastUpdated).toLocaleDateString()}</span>
                            </div>
                          </div>
                        </div>
                        
                        <div className="ml-4">
                          {repo.enabled ? (
                            <button
                              onClick={() => navigate(`/repositories/${repo.fullName.split('/')[0]}/${repo.fullName.split('/')[1]}`)}
                              className="btn-secondary text-sm"
                            >
                              View Repository
                            </button>
                          ) : (
                            <button
                              onClick={() => enableRepository(repo.id)}
                              disabled={loading}
                              className="btn-primary text-sm"
                            >
                              {loading ? (
                                <div className="animate-spin rounded-full h-3 w-3 border-b-2 border-white"></div>
                              ) : (
                                'Enable'
                              )}
                            </button>
                          )}
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}

            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default NewRepository; 