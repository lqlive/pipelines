import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  FolderIcon,
  LockClosedIcon,
  MagnifyingGlassIcon,
  CheckCircleIcon,
  PlusIcon,
  ArrowLeftIcon,
  ExclamationCircleIcon,
  CloudIcon,
  ArrowPathIcon,
} from '@heroicons/react/24/outline';
import { RemoteService, RemoteRepository } from '../services/remoteService';

interface GitProvider {
  id: string;
  name: string;
  icon: string;
  connected: boolean;
  description: string;
  repositoryCount?: number;
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
  const [authenticationRequired, setAuthenticationRequired] = useState(false);
  const [error, setError] = useState<string | null>(null);

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



  useEffect(() => {
    // Load repositories when component mounts or provider changes
    loadRepositories();
  }, [selectedProvider]);

  useEffect(() => {
    // Filter repositories based on search term
    const filtered = repositories.filter(repo =>
      repo.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (repo.description && repo.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (repo.language && repo.language.toLowerCase().includes(searchTerm.toLowerCase()))
    );
    setFilteredRepositories(filtered);
  }, [repositories, searchTerm]);

  const loadRepositories = async () => {
    setLoading(true);
    setError(null);
    setAuthenticationRequired(false);
    
    try {
      // Call real API to fetch repositories from GitHub
      const repositories = await RemoteService.getGitHubRepositories();
      setRepositories(repositories);
    } catch (error: any) {
      console.error('Failed to load repositories:', error);
      
      // Check for 401 status in multiple ways to ensure we catch it
      if (error.response?.status === 401 || error.status === 401) {
        setAuthenticationRequired(true);
        setError('Authentication required to access GitHub repositories');
      } else {
        setError('Failed to load repositories. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  const syncRepositories = async () => {
    setSyncLoading(true);
    try {
      // Sync repositories from GitHub API
      const repositories = await RemoteService.syncGitHubRepositories();
      setRepositories(repositories);
    } catch (error) {
      console.error('Failed to sync repositories:', error);
      setError('Failed to sync repositories. Please try again.');
    } finally {
      setSyncLoading(false);
    }
  };

  const handleAuthentication = () => {
    // Redirect to GitHub authentication using service helper
    RemoteService.loginWithGitHub(window.location.href);
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
      // Call API to enable repository using remote service
      await RemoteService.enableGitHubRepository(repoId);
      
      setRepositories(prev => 
        prev.map(repo => 
          repo.id === repoId ? { ...repo, enabled: true } : repo
        )
      );
      
      const repo = repositories.find(r => r.id === repoId);
      if (repo) {
        // Extract owner and repo name from URL if fullName is not available
        const fullName = repo.fullName || repo.url.split('/').slice(-2).join('/');
        const [owner, repoName] = fullName.split('/');
        navigate(`/repositories/${owner}/${repoName}`);
      }
    } catch (error) {
      console.error('Failed to enable repository:', error);
      setError('Failed to enable repository. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const enableSelectedRepositories = async () => {
    if (selectedRepos.size === 0) return;
    
    // Filter out already enabled repositories
    const reposToEnable = Array.from(selectedRepos).filter(repoId => {
      const repo = repositories.find(r => r.id === repoId);
      return repo && !repo.enabled;
    });
    
    if (reposToEnable.length === 0) {
      setError('Selected repositories are already enabled.');
      return;
    }
    
    setLoading(true);
    try {
      // Call API to enable multiple repositories using remote service
      await RemoteService.enableGitHubRepositories(reposToEnable);
      
      setRepositories(prev => 
        prev.map(repo => 
          reposToEnable.includes(repo.id) ? { ...repo, enabled: true } : repo
        )
      );
      
      setSelectedRepos(new Set());
      navigate('/repositories');
    } catch (error) {
      console.error('Failed to enable repositories:', error);
      setError('Failed to enable repositories. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const selectedProvider_obj = gitProviders.find(p => p.id === selectedProvider);
  const enabledCount = repositories.filter(repo => repo.enabled).length;
  const availableCount = repositories.filter(repo => !repo.enabled).length;
  
  // Count selected repositories that are not yet enabled
  const selectedAvailableCount = Array.from(selectedRepos).filter(repoId => {
    const repo = repositories.find(r => r.id === repoId);
    return repo && !repo.enabled;
  }).length;

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
                {selectedAvailableCount > 0 && (
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
                    Enable {selectedAvailableCount} Repository{selectedAvailableCount > 1 ? 'ies' : 'y'}
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

              {/* Error Message */}
              {error && !authenticationRequired && (
                <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md">
                  <div className="flex items-center">
                    <ExclamationCircleIcon className="h-5 w-5 text-red-400 mr-2" />
                    <p className="text-sm text-red-800">{error}</p>
                  </div>
                </div>
              )}

              {/* Repository List */}
              {loading ? (
                <div className="flex items-center justify-center h-32">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
                </div>
              ) : authenticationRequired ? (
                <div className="text-center py-8">
                  <CloudIcon className="h-12 w-12 text-gray-400 mx-auto mb-3" />
                  <h3 className="text-lg font-medium text-gray-900 mb-2">Authentication Required</h3>
                  <p className="text-gray-600 mb-4">
                    Please authenticate with GitHub to view your repositories
                  </p>
                  <button 
                    onClick={handleAuthentication}
                    className="btn-primary"
                  >
                    Authenticate with GitHub
                  </button>
                </div>
              ) : filteredRepositories.length === 0 ? (
                <div className="text-center py-8">
                  <FolderIcon className="h-12 w-12 text-gray-400 mx-auto mb-3" />
                  <h3 className="text-lg font-medium text-gray-900 mb-2">No repositories found</h3>
                  <p className="text-gray-600">
                    {searchTerm 
                      ? 'Try adjusting your search terms'
                      : error
                        ? 'Unable to load repositories'
                        : 'No repositories available for import'
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
                              {repo.stars !== undefined && <span>⭐ {repo.stars}</span>}
                              {repo.forks !== undefined && <span>🍴 {repo.forks}</span>}
                              {repo.lastUpdated && <span>Updated {new Date(repo.lastUpdated).toLocaleDateString()}</span>}
                              {repo.cloneUrl && (
                                <a 
                                  href={repo.cloneUrl} 
                                  target="_blank" 
                                  rel="noopener noreferrer"
                                  className="text-blue-600 hover:text-blue-800"
                                >
                                  View on GitHub
                                </a>
                              )}
                            </div>
                          </div>
                        </div>
                        
                        <div className="ml-4">
                          {repo.enabled ? (
                            <button
                              onClick={() => {
                                const fullName = repo.fullName || repo.cloneUrl?.split('/').slice(-2).join('/') || '';
                                const [owner, repoName] = fullName.split('/');
                                if (owner && repoName) {
                                  navigate(`/repositories/${owner}/${repoName}`);
                                }
                              }}
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