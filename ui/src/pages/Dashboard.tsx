import React, { useState, useEffect } from 'react';
import {
  ChartBarIcon,
  CheckCircleIcon,
  XCircleIcon,
  RocketLaunchIcon,
  ClockIcon,
} from '@heroicons/react/24/outline';
import BuildCard from '../components/BuildCard/BuildCard';
import BuildStatus from '../components/BuildStatus/BuildStatus';
import type { Stats, Build } from '../types';

interface StatCard {
  title: string;
  value: number;
  icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
  color: string;
  bgColor: string;
}

// Mock data
const mockData = {
  stats: {
    totalBuilds: 156,
    successfulBuilds: 142,
    failedBuilds: 12,
    runningBuilds: 2,
    pendingBuilds: 5,
    successRate: 91.0,
  } as Stats,
  recentBuilds: [
    {
      number: 156,
      status: 'success' as const,
      started: '2024-01-15T10:30:00Z',
      finished: '2024-01-15T10:45:00Z',
      author: 'zhang.wei',
      message: 'Fix user login validation logic',
      branch: 'main',
      commit: 'a1b2c3d4e5f6g7h8i9j0',
      repository: { owner: 'myorg', name: 'backend-api' },
      steps: [],
    },
    {
      number: 155,
      status: 'running' as const,
      started: '2024-01-15T10:15:00Z',
      finished: null,
      author: 'li.ming',
      message: 'Add new payment functionality',
      branch: 'feature/payment',
      commit: 'b2c3d4e5f6g7h8i9j0k1',
      repository: { owner: 'myorg', name: 'frontend-app' },
      steps: [],
    },
    {
      number: 154,
      status: 'success' as const,
      started: '2024-01-15T09:45:00Z',
      finished: '2024-01-15T09:52:00Z',
      author: 'wang.fang',
      message: 'Update dependency versions',
      branch: 'develop',
      commit: 'c3d4e5f6g7h8i9j0k1l2',
      repository: { owner: 'myorg', name: 'mobile-app' },
      steps: [],
    },
    {
      number: 153,
      status: 'failure' as const,
      started: '2024-01-15T09:20:00Z',
      finished: null,
      author: 'chen.qiang',
      message: 'Refactor database query optimization',
      branch: 'main',
      commit: 'd4e5f6g7h8i9j0k1l2m3',
      repository: { owner: 'myorg', name: 'database-service' },
      steps: [],
    },
    {
      number: 152,
      status: 'success' as const,
      started: '2024-01-15T08:16:00Z',
      finished: '2024-01-15T08:28:00Z',
      author: 'liu.jie',
      message: 'Increase unit test coverage',
      branch: 'test/coverage',
      commit: 'e5f6g7h8i9j0k1l2m3n4',
      repository: { owner: 'myorg', name: 'utils-library' },
      steps: [],
    },
  ] as Build[],
};

const Dashboard: React.FC = () => {
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState<Stats>(mockData.stats);
  const [recentBuilds, setRecentBuilds] = useState<Build[]>([]);

  useEffect(() => {
    // TODO: Replace with real API call
    setLoading(true);
    setTimeout(() => {
      setStats(mockData.stats);
      setRecentBuilds(mockData.recentBuilds);
      setLoading(false);
    }, 1000);
  }, []);

  const statsCards: StatCard[] = [
    {
      title: 'Total Builds',
      value: stats.totalBuilds,
      icon: ChartBarIcon,
      color: 'text-primary-600',
      bgColor: 'bg-primary-100',
    },
    {
      title: 'Successful',
      value: stats.successfulBuilds,
      icon: CheckCircleIcon,
      color: 'text-success-600',
      bgColor: 'bg-success-100',
    },
    {
      title: 'Failed',
      value: stats.failedBuilds,
      icon: XCircleIcon,
      color: 'text-error-600',
      bgColor: 'bg-error-100',
    },
    {
      title: 'Running',
      value: stats.runningBuilds,
      icon: RocketLaunchIcon,
      color: 'text-primary-600',
      bgColor: 'bg-primary-100',
    },
    {
      title: 'Pending',
      value: stats.pendingBuilds,
      icon: ClockIcon,
      color: 'text-accent-600',
      bgColor: 'bg-accent-100',
    },
  ];

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
      </div>
    );
  }

  return (
    <div className="fade-in">
      {/* Page Title */}
      <div className="section-header">
        <h1 className="text-xl font-medium text-gray-900">Dashboard</h1>
        <p className="text-sm text-gray-600">Overview and metrics</p>
      </div>

      {/* Statistics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-6 mb-12">
        {statsCards.map((stat, index) => (
          <div key={index} className="card-minimal text-center">
            <div className="text-2xl font-semibold text-gray-900 mb-1">
              {stat.value}
            </div>
            <div className="text-sm text-gray-600">{stat.title}</div>
          </div>
        ))}
      </div>

      {/* Success Rate Metrics */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-12">
        <div className="card">
          <h3 className="text-sm font-medium text-gray-900 mb-6">Build Success Rate</h3>
          <div className="space-y-3">
            <div className="flex justify-between text-sm">
              <span>Last 30 days</span>
              <span className="font-medium">
                {((stats.successfulBuilds / (stats.successfulBuilds + stats.failedBuilds)) * 100).toFixed(1)}%
              </span>
            </div>
            <div className="w-full bg-gray-200 rounded-full h-2">
              <div 
                className="bg-green-600 h-2 rounded-full" 
                style={{ 
                  width: `${(stats.successfulBuilds / (stats.successfulBuilds + stats.failedBuilds)) * 100}%` 
                }}
              />
            </div>
          </div>
        </div>

        <div className="card">
          <h3 className="text-sm font-medium text-gray-900 mb-6">Build Status</h3>
          <div className="space-y-3">
            <div className="flex justify-between">
              <div className="flex items-center">
                <BuildStatus status="success" showText={false} />
                <span className="ml-2 text-sm text-gray-600">Success</span>
              </div>
              <span className="text-sm font-medium">{stats.successfulBuilds}</span>
            </div>
            <div className="flex justify-between">
              <div className="flex items-center">
                <BuildStatus status="failure" showText={false} />
                <span className="ml-2 text-sm text-gray-600">Failed</span>
              </div>
              <span className="text-sm font-medium">{stats.failedBuilds}</span>
            </div>
            <div className="flex justify-between">
              <div className="flex items-center">
                <BuildStatus status="running" showText={false} />
                <span className="ml-2 text-sm text-gray-600">Running</span>
              </div>
              <span className="text-sm font-medium">{stats.runningBuilds}</span>
            </div>
            <div className="flex justify-between">
              <div className="flex items-center">
                <BuildStatus status="pending" showText={false} />
                <span className="ml-2 text-sm text-gray-600">Pending</span>
              </div>
              <span className="text-sm font-medium">{stats.pendingBuilds}</span>
            </div>
          </div>
        </div>
      </div>

      {/* Recent Builds */}
      <div>
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-sm font-medium text-gray-900">Recent Builds</h2>
        </div>
        
        <div className="grid grid-cols-1 lg:grid-cols-2 xl:grid-cols-3 gap-6">
          {recentBuilds.map((build) => (
            <BuildCard key={`${build.repository.owner}-${build.repository.name}-${build.number}`} build={build} />
          ))}
        </div>
      </div>
    </div>
  );
};

export default Dashboard; 