import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import {
  CodeBracketIcon,
  PlayIcon,
  Cog6ToothIcon,
  DocumentDuplicateIcon,
  ChevronDownIcon,
} from '@heroicons/react/24/outline';
import moment from 'moment';
import BuildCard from '../../components/BuildCard/BuildCard';
import BuildStatus from '../../components/BuildStatus/BuildStatus';
import type { Repository as RepositoryType, Build } from '../../types';

interface RouteParams extends Record<string, string | undefined> {
  owner: string;
  name: string;
}

// Mock data
const getMockRepository = (owner: string, name: string): RepositoryType => ({
  owner,
  name,
  description: 'Backend API service providing core business logic and data interfaces',
  private: false,
  active: true,
  defaultBranch: 'main',
  branchCount: 12,
  lastActivity: '2024-01-15T10:30:00Z',
  cloneUrl: `https://github.com/${owner}/${name}.git`,
  branches: ['main', 'develop', 'feature/auth', 'feature/payment', 'hotfix/security'],
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
      repository: { owner, name },
      steps: [],
    },
    {
      number: 155,
      status: 'failure',
      started: '2024-01-15T09:40:00Z',
      finished: '2024-01-15T09:58:00Z',
      author: 'li.ming',
      message: 'Optimize database query performance',
      branch: 'main',
      commit: 'b2c3d4e5f6g7h8i9j0k1',
      repository: { owner, name },
      steps: [],
    },
    {
      number: 154,
      status: 'success',
      started: '2024-01-15T08:20:00Z',
      finished: '2024-01-15T08:35:00Z',
      author: 'wang.fang',
      message: 'Add new API endpoints',
      branch: 'feature/payment',
      commit: 'c3d4e5f6g7h8i9j0k1l2',
      repository: { owner, name },
      steps: [],
    },
    {
      number: 153,
      status: 'success',
      started: '2024-01-14T16:20:00Z',
      finished: '2024-01-14T16:35:00Z',
      author: 'chen.qiang',
      message: 'Update documentation and README',
      branch: 'main',
      commit: 'd4e5f6g7h8i9j0k1l2m3',
      repository: { owner, name },
      steps: [],
    },
    {
      number: 152,
      status: 'running',
      started: '2024-01-14T15:45:00Z',
      finished: null,
      author: 'liu.jie',
      message: 'Refactor authentication module',
      branch: 'develop',
      commit: 'e5f6g7h8i9j0k1l2m3n4',
      repository: { owner, name },
      steps: [],
    },
  ],
});

const Repository: React.FC = () => {
  const { owner, name } = useParams<RouteParams>();
  const [loading, setLoading] = useState(true);
  const [repository, setRepository] = useState<RepositoryType | null>(null);
  const [filteredBuilds, setFilteredBuilds] = useState<Build[]>([]);
  const [selectedBranch, setSelectedBranch] = useState('all');
  const [selectedStatus, setSelectedStatus] = useState('all');

  useEffect(() => {
    if (!owner || !name) return;
    
    // Mock API call
    setLoading(true);
    setTimeout(() => {
      const mockRepo = getMockRepository(owner, name);
      setRepository(mockRepo);
      setLoading(false);
    }, 1000);
  }, [owner, name]);

  // Filter builds
  useEffect(() => {
    if (!repository) return;

    let filtered = repository.builds;

    if (selectedBranch !== 'all') {
      filtered = filtered.filter(build => build.branch === selectedBranch);
    }

    if (selectedStatus !== 'all') {
      filtered = filtered.filter(build => build.status === selectedStatus);
    }

    setFilteredBuilds(filtered);
  }, [repository, selectedBranch, selectedStatus]);

  const triggerBuild = () => {
    // Mock trigger new build
    console.log('Trigger new build');
  };

  const copyCloneUrl = () => {
    if (repository) {
      navigator.clipboard.writeText(repository.cloneUrl);
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
      {/* Page Title */}
      <div className="section-header">
        <div className="flex items-center justify-between">
          <div>
            <div className="flex items-center mb-2">
              <h1 className="text-xl font-medium text-gray-900">{repository.name}</h1>
              {repository.private && (
                <span className="status-badge bg-gray-100 text-gray-700 ml-3">Private</span>
              )}
              {!repository.active && (
                <span className="status-badge bg-yellow-100 text-yellow-800 ml-2">Inactive</span>
              )}
            </div>
            <p className="text-sm text-gray-600">{repository.description}</p>
          </div>
          <div className="flex items-center space-x-3">
            <button onClick={triggerBuild} className="btn-primary">
              <PlayIcon className="h-4 w-4 mr-2" />
              Run Build
            </button>
            <Link 
              to={`/repositories/${owner}/${name}/settings`}
              className="btn-secondary"
            >
              <Cog6ToothIcon className="h-4 w-4 mr-2" />
              Settings
            </Link>
          </div>
        </div>
      </div>

      {/* Repository Info */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        <div className="lg:col-span-2">
          <div className="card">
            <h3 className="text-sm font-medium text-gray-900 mb-4">Repository Info</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <div className="text-sm text-gray-500 mb-1">Default branch</div>
                <div className="flex items-center">
                  <CodeBracketIcon className="h-4 w-4 text-gray-400 mr-2" />
                  <span className="font-medium">{repository.defaultBranch}</span>
                </div>
              </div>
              <div>
                <div className="text-sm text-gray-500 mb-1">Branch count</div>
                <div className="font-medium">{repository.branchCount} branches</div>
              </div>
              <div>
                <div className="text-sm text-gray-500 mb-1">Last activity</div>
                <div className="font-medium">{moment(repository.lastActivity).fromNow()}</div>
              </div>
              <div>
                <div className="text-sm text-gray-500 mb-1">Status</div>
                <div className="font-medium">
                  {repository.active ? 'Active' : 'Inactive'}
                </div>
              </div>
            </div>
            <div className="mt-4">
              <div className="text-sm text-gray-500 mb-1">Clone URL</div>
              <div className="flex items-center">
                <input
                  type="text"
                  value={repository.cloneUrl}
                  readOnly
                  className="minimal-input flex-1 mr-2 text-sm font-mono"
                />
                <button onClick={copyCloneUrl} className="btn-secondary text-sm">
                  Copy
                </button>
              </div>
            </div>
          </div>
        </div>

        <div className="card">
          <h3 className="text-sm font-medium text-gray-900 mb-4">Latest Build</h3>
          {repository.builds.length > 0 ? (
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <BuildStatus status={repository.builds[0].status} />
                <span className="text-sm text-gray-500">#{repository.builds[0].number}</span>
              </div>
              <div className="text-sm text-gray-600">
                {repository.builds[0].message}
              </div>
              <div className="text-xs text-gray-500">
                {moment(repository.builds[0].started).fromNow()}
              </div>
            </div>
          ) : (
            <div className="text-center py-6 text-gray-500">
              <div>No build records</div>
              <button onClick={triggerBuild} className="btn-primary mt-3 text-sm">
                Run first build
              </button>
            </div>
          )}
        </div>
      </div>

      {/* Build History */}
      <div>
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-sm font-medium text-gray-900">Build History</h2>
          <div className="flex items-center space-x-4">
            {/* Branch Filter */}
            <div className="relative">
              <select
                value={selectedBranch}
                onChange={(e) => setSelectedBranch(e.target.value)}
                className="appearance-none bg-white border border-gray-300 rounded-md px-3 py-2 pr-8 text-sm focus:outline-none focus:ring-1 focus:ring-primary-500 focus:border-primary-500"
              >
                <option value="all">All branches</option>
                {repository.branches.map(branch => (
                  <option key={branch} value={branch}>{branch}</option>
                ))}
              </select>
              <ChevronDownIcon className="absolute right-2 top-2.5 h-4 w-4 text-gray-400 pointer-events-none" />
            </div>

            {/* Status Filter */}
            <div className="relative">
              <select
                value={selectedStatus}
                onChange={(e) => setSelectedStatus(e.target.value)}
                className="appearance-none bg-white border border-gray-300 rounded-md px-3 py-2 pr-8 text-sm focus:outline-none focus:ring-1 focus:ring-primary-500 focus:border-primary-500"
              >
                <option value="all">All status</option>
                <option value="success">Success</option>
                <option value="failure">Failed</option>
                <option value="running">Running</option>
                <option value="pending">Pending</option>
              </select>
              <ChevronDownIcon className="absolute right-2 top-2.5 h-4 w-4 text-gray-400 pointer-events-none" />
            </div>
          </div>
        </div>

        {filteredBuilds.length === 0 ? (
          <div className="text-center py-12">
            <div className="text-gray-400 mb-4">
              <PlayIcon className="h-12 w-12 mx-auto" />
            </div>
            <h3 className="text-lg font-medium text-gray-900 mb-2">No build records found</h3>
            <p className="text-gray-600">
              {selectedBranch !== 'all' || selectedStatus !== 'all'
                ? 'Try adjusting your filter conditions'
                : 'Start your first build'
              }
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
            {filteredBuilds.map((build) => (
              <BuildCard
                key={build.number}
                build={build}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Repository; 