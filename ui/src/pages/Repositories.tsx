import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  MagnifyingGlassIcon,
  PlusIcon,
  FunnelIcon,
} from '@heroicons/react/24/outline';
import RepositoryCard from '../components/RepositoryCard/RepositoryCard';
import type { Repository } from '../types';

// Mock data
const mockRepositories: Repository[] = [
  {
    owner: 'myorg',
    name: 'backend-api',
    description: 'Backend API service providing core business logic and data interfaces',
    private: false,
    active: true,
    defaultBranch: 'main',
    branchCount: 12,
    lastActivity: '2024-01-15T10:30:00Z',
    cloneUrl: 'https://github.com/myorg/backend-api.git',
    branches: ['main', 'develop', 'feature/auth', 'feature/payments'],
    builds: [
      {
        number: 156,
        status: 'success',
        started: '2024-01-15T10:30:00Z',
        finished: '2024-01-15T10:45:00Z',
        author: 'zhang.wei',
        message: 'Fix user login validation logic',
        branch: 'main',
        commit: 'a1b2c3d4e5f6g7h8i9j0',
        repository: { owner: 'myorg', name: 'backend-api' },
        steps: [],
      },
    ],
  },
  {
    owner: 'myorg',
    name: 'frontend-app',
    description: 'Frontend application built with React for user interface',
    private: false,
    active: true,
    defaultBranch: 'main',
    branchCount: 8,
    lastActivity: '2024-01-15T09:15:00Z',
    cloneUrl: 'https://github.com/myorg/frontend-app.git',
    branches: ['main', 'develop', 'feature/dashboard'],
    builds: [
      {
        number: 89,
        status: 'running',
        started: '2024-01-15T09:15:00Z',
        finished: null,
        author: 'li.ming',
        message: 'Update UI components',
        branch: 'develop',
        commit: 'b2c3d4e5f6g7h8i9j0k1',
        repository: { owner: 'myorg', name: 'frontend-app' },
        steps: [],
      },
    ],
  },
  {
    owner: 'myorg',
    name: 'mobile-app',
    description: 'Mobile application supporting iOS and Android platforms',
    private: true,
    active: true,
    defaultBranch: 'main',
    branchCount: 15,
    lastActivity: '2024-01-14T16:22:00Z',
    cloneUrl: 'https://github.com/myorg/mobile-app.git',
    branches: ['main', 'develop', 'ios-fixes', 'android-updates'],
    builds: [],
  },
  {
    owner: 'myorg',
    name: 'database-service',
    description: 'Database service layer handling data persistence and query optimization',
    private: true,
    active: false,
    defaultBranch: 'main',
    branchCount: 6,
    lastActivity: '2024-01-10T11:00:00Z',
    cloneUrl: 'https://github.com/myorg/database-service.git',
    branches: ['main', 'feature/migrations'],
    builds: [],
  },
  {
    owner: 'myorg',
    name: 'utils-library',
    description: 'Common utility library with helper functions and components',
    private: false,
    active: true,
    defaultBranch: 'main',
    branchCount: 4,
    lastActivity: '2024-01-12T14:30:00Z',
    cloneUrl: 'https://github.com/myorg/utils-library.git',
    branches: ['main', 'develop'],
    builds: [],
  },
  {
    owner: 'myorg',
    name: 'documentation',
    description: 'Project documentation including API docs and user guides',
    private: false,
    active: true,
    defaultBranch: 'main',
    branchCount: 3,
    lastActivity: '2024-01-08T09:45:00Z',
    cloneUrl: 'https://github.com/myorg/documentation.git',
    branches: ['main'],
    builds: [],
  },
];

const Repositories: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [repositories, setRepositories] = useState<Repository[]>([]);
  const [filteredRepositories, setFilteredRepositories] = useState<Repository[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterActive, setFilterActive] = useState<'all' | 'active' | 'inactive'>('all');
  const [filterPrivate, setFilterPrivate] = useState<'all' | 'public' | 'private'>('all');

  useEffect(() => {
    // Mock API call
    setTimeout(() => {
      setRepositories(mockRepositories);
      setLoading(false);
    }, 1000);
  }, []);

  // Filter repositories
  useEffect(() => {
    let filtered = repositories;

    // Search filter
    if (searchTerm) {
      filtered = filtered.filter(repo => 
        repo.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        repo.description.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Active status filter
    if (filterActive !== 'all') {
      filtered = filtered.filter(repo => 
        filterActive === 'active' ? repo.active : !repo.active
      );
    }

    // Private status filter
    if (filterPrivate !== 'all') {
      filtered = filtered.filter(repo => 
        filterPrivate === 'private' ? repo.private : !repo.private
      );
    }

    setFilteredRepositories(filtered);
  }, [repositories, searchTerm, filterActive, filterPrivate]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
      </div>
    );
  }

  const activeCount = repositories.filter(repo => repo.active).length;
  const privateCount = repositories.filter(repo => repo.private).length;

  return (
    <div className="fade-in">
      {/* Page Title */}
      <div className="section-header">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-xl font-medium text-gray-900">Repositories</h1>
            <p className="text-sm text-gray-600">Manage your project repositories</p>
          </div>
                      <Link to="/repositories/new" className="btn-primary">
              <PlusIcon className="h-4 w-4 mr-2" />
              Import Repository
            </Link>
        </div>
      </div>

      {/* Search and Filter */}
      <div className="mb-6">
        {/* Search Box */}
        <div className="relative mb-6">
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MagnifyingGlassIcon className="h-4 w-4 text-gray-400" />
          </div>
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="minimal-input pl-10"
            placeholder="Search repositories..."
          />
        </div>

        {/* Filters */}
        <div className="flex items-center space-x-3">
          <div className="flex items-center">
            <FunnelIcon className="h-4 w-4 text-gray-400 mr-2" />
            <span className="text-xs text-gray-500 uppercase tracking-wide">Filter</span>
          </div>
          
          <select
            value={filterActive}
            onChange={(e) => setFilterActive(e.target.value as 'all' | 'active' | 'inactive')}
            className="text-xs border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-gray-900 focus:border-transparent"
          >
            <option value="all">All status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
          </select>

          <select
            value={filterPrivate}
            onChange={(e) => setFilterPrivate(e.target.value as 'all' | 'public' | 'private')}
            className="text-xs border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-gray-900 focus:border-transparent"
          >
            <option value="all">All visibility</option>
            <option value="public">Public</option>
            <option value="private">Private</option>
          </select>
        </div>
      </div>

      {/* Statistics */}
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center space-x-6 text-sm text-gray-500">
          <span>{repositories.length} repositories</span>
          <span>{activeCount} active</span>
          <span>{privateCount} private</span>
        </div>
      </div>

      {/* Repository List */}
      <div>
        {filteredRepositories.length === 0 ? (
          <div className="text-center py-12">
            <div className="text-gray-400 mb-4">
              <FunnelIcon className="h-12 w-12 mx-auto" />
            </div>
            <h3 className="text-lg font-medium text-gray-900 mb-2">No repositories found</h3>
            <p className="text-gray-600 mb-4">
              {searchTerm || filterActive !== 'all' || filterPrivate !== 'all'
                ? 'Try adjusting your search terms or filters'
                : 'Get started by adding your first repository'
              }
            </p>
            {!(searchTerm || filterActive !== 'all' || filterPrivate !== 'all') && (
              <Link to="/repositories/new" className="btn-primary">
                <PlusIcon className="h-5 w-5 mr-2" />
                Add Repository
              </Link>
            )}
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
            {filteredRepositories.map((repository) => (
              <RepositoryCard
                key={`${repository.owner}/${repository.name}`}
                repository={repository}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Repositories; 