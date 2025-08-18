import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  MagnifyingGlassIcon,
  PlusIcon,
  FunnelIcon,
  ExclamationTriangleIcon,
} from '@heroicons/react/24/outline';
import RepositoryCard from '../components/RepositoryCard/RepositoryCard';
import type { Repository } from '../types';
import { RepositoryService } from '../services/repositoryService';

const Repositories: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [repositories, setRepositories] = useState<Repository[]>([]);
  const [filteredRepositories, setFilteredRepositories] = useState<Repository[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [filterActive, setFilterActive] = useState<'all' | 'active' | 'inactive'>('all');
  const [filterPrivate, setFilterPrivate] = useState<'all' | 'public' | 'private'>('all');

  // Fetch repositories from API using repository service
  const fetchRepositories = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const data = await RepositoryService.getRepositories();
      setRepositories(data);
    } catch (err: any) {
      console.error('Error fetching repositories:', err);
      setError(err.message || 'Failed to load repositories');
    } finally {
      setLoading(false);
    }
  };

  // Load repositories when component mounts
  useEffect(() => {
    fetchRepositories();
  }, []);

  // Apply filters to repositories when repositories or filter criteria change
  useEffect(() => {
    let filtered = repositories;

    // Apply search filter by name or description
    if (searchTerm) {
      filtered = filtered.filter(repo => 
        repo.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        repo.description.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Apply active status filter
    if (filterActive !== 'all') {
      filtered = filtered.filter(repo => 
        filterActive === 'active' ? repo.active : !repo.active
      );
    }

    // Apply visibility filter (public/private)
    if (filterPrivate !== 'all') {
      filtered = filtered.filter(repo => 
        filterPrivate === 'private' ? repo.private : !repo.private
      );
    }

    setFilteredRepositories(filtered);
  }, [repositories, searchTerm, filterActive, filterPrivate]);

  // Show loading state
  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
        <span className="ml-3 text-gray-600">Loading repositories...</span>
      </div>
    );
  }

  // Show error state
  if (error) {
    return (
      <div className="flex flex-col items-center justify-center h-64">
        <ExclamationTriangleIcon className="h-12 w-12 text-red-500 mb-4" />
        <h3 className="text-lg font-medium text-gray-900 mb-2">Error loading repositories</h3>
        <p className="text-gray-600 mb-4">{error}</p>
        <button
          onClick={fetchRepositories}
          className="btn-primary"
        >
          Try Again
        </button>
      </div>
    );
  }

  // Calculate repository statistics
  const activeCount = repositories.filter(repo => repo.active).length;
  const privateCount = repositories.filter(repo => repo.private).length;

  return (
    <div className="fade-in">
      {/* Page Header */}
      <div className="section-header">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-xl font-medium text-gray-900">Repositories</h1>
            <p className="text-sm text-gray-600">Manage your project repositories</p>
          </div>
          <div className="flex items-center space-x-3">
            <button
              onClick={fetchRepositories}
              className="btn-secondary"
              disabled={loading}
            >
              Refresh
            </button>
            <Link to="/repositories/new" className="btn-primary">
              <PlusIcon className="h-4 w-4 mr-2" />
              Import Repository
            </Link>
          </div>
        </div>
      </div>

      {/* Search and Filter Controls */}
      <div className="mb-6">
        {/* Search Input */}
        <div className="relative mb-6">
          <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
            <MagnifyingGlassIcon className="h-4 w-4 text-gray-400" />
          </div>
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="minimal-input pl-10"
            placeholder="Search repositories by name or description..."
          />
        </div>

        {/* Filter Controls */}
        <div className="flex items-center space-x-3">
          <div className="flex items-center">
            <FunnelIcon className="h-4 w-4 text-gray-400 mr-2" />
            <span className="text-xs text-gray-500 uppercase tracking-wide">Filter</span>
          </div>
          
          {/* Status Filter */}
          <select
            value={filterActive}
            onChange={(e) => setFilterActive(e.target.value as 'all' | 'active' | 'inactive')}
            className="text-xs border border-gray-300 rounded-md px-2 py-1 focus:outline-none focus:ring-2 focus:ring-gray-900 focus:border-transparent"
          >
            <option value="all">All status</option>
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
          </select>

          {/* Visibility Filter */}
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

      {/* Repository Statistics */}
      <div className="flex items-center justify-between mb-8">
        <div className="flex items-center space-x-6 text-sm text-gray-500">
          <span>{repositories.length} repositories</span>
          <span>{activeCount} active</span>
          <span>{privateCount} private</span>
        </div>
      </div>

      {/* Repository Grid */}
      <div>
        {filteredRepositories.length === 0 ? (
          // Empty state - no repositories match current filters
          <div className="text-center py-12">
            <div className="text-gray-400 mb-4">
              <FunnelIcon className="h-12 w-12 mx-auto" />
            </div>
            <h3 className="text-lg font-medium text-gray-900 mb-2">No repositories found</h3>
            <p className="text-gray-600 mb-4">
              {searchTerm || filterActive !== 'all' || filterPrivate !== 'all'
                ? 'Try adjusting your search terms or filters'
                : 'Get started by importing your first repository'
              }
            </p>
          </div>
        ) : (
          // Repository grid - display filtered repositories
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