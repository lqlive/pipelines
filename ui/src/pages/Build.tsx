import React, { useState, useEffect } from 'react';
import { useParams } from 'react-router-dom';
import {
  CodeBracketIcon,
  StopIcon,
  ArrowPathIcon,
  ClockIcon,
} from '@heroicons/react/24/outline';
import moment from 'moment';
import BuildStatus from '../components/BuildStatus/BuildStatus';
import type { Build as BuildType } from '../types';

interface RouteParams extends Record<string, string | undefined> {
  owner: string;
  name: string;
  buildNumber: string;
}

// Mock data
const getMockBuild = (owner: string, name: string, buildNumber: string): BuildType => ({
  number: parseInt(buildNumber),
  status: 'success',
  started: '2024-01-15T10:30:00Z',
  finished: '2024-01-15T10:45:00Z',
  author: 'zhang.wei',
  message: 'Fix user login validation logic',
  branch: 'main',
  commit: 'a1b2c3d4e5f6g7h8i9j0',
  repository: { owner, name },
  steps: [
    {
      name: 'Clone',
      status: 'success',
      started: '2024-01-15T10:30:00Z',
      finished: '2024-01-15T10:31:00Z',
      logs: [
        '+ git clone https://github.com/myorg/backend-api.git .',
        'Cloning into \'.\'...',
        'remote: Enumerating objects: 1234, done.',
        'remote: Counting objects: 100% (1234/1234), done.',
        'remote: Compressing objects: 100% (567/567), done.',
        'remote: Total 1234 (delta 890), reused 1123 (delta 779)',
        'Receiving objects: 100% (1234/1234), 2.34 MiB | 5.67 MiB/s, done.',
        'Resolving deltas: 100% (890/890), done.',
        '',
        '+ git checkout a1b2c3d4e5f6g7h8i9j0',
        'Note: switching to \'a1b2c3d4e5f6g7h8i9j0\'.',
        'HEAD is now at a1b2c3d Fix user login validation logic'
      ]
    },
    {
      name: 'Install Dependencies',
      status: 'success',
      started: '2024-01-15T10:31:00Z',
      finished: '2024-01-15T10:35:00Z',
      logs: [
        '+ npm ci',
        'npm WARN using --force',
        'npm WARN using --force',
        'added 1234 packages in 3.2s',
        '',
        '56 packages are looking for funding',
        '  run `npm fund` for details'
      ]
    },
    {
      name: 'Run Tests',
      status: 'success',
      started: '2024-01-15T10:35:00Z',
      finished: '2024-01-15T10:42:00Z',
      logs: [
        '+ npm test',
        '',
        '> backend-api@1.0.0 test',
        '> jest --coverage',
        '',
        ' PASS  src/auth/auth.test.js',
        ' PASS  src/users/users.test.js',
        ' PASS  src/payments/payments.test.js',
        '',
        'Test Suites: 15 passed, 15 total',
        'Tests:       89 passed, 89 total',
        'Snapshots:   0 total',
        'Time:        6.789 s',
        'Ran all test suites.',
        '',
        'Coverage: 92.5%'
      ]
    },
    {
      name: 'Build',
      status: 'success',
      started: '2024-01-15T10:42:00Z',
      finished: '2024-01-15T10:45:00Z',
      logs: [
        '+ npm run build',
        '',
        '> backend-api@1.0.0 build',
        '> webpack --mode=production',
        '',
        'asset main.js 1.23 MiB [emitted] [minimized] (name: main)',
        'webpack 5.74.0 compiled successfully in 2834 ms'
      ]
    }
  ]
});

const formatDuration = (started: string, finished: string | null): string => {
  if (!finished) {
    return moment(started).fromNow();
  }
  
  const duration = moment(finished).diff(moment(started));
  const minutes = Math.floor(duration / 60000);
  const seconds = Math.floor((duration % 60000) / 1000);
  
  if (minutes > 0) {
    return `${minutes}m ${seconds}s`;
  }
  return `${seconds}s`;
};

const Build: React.FC = () => {
  const { owner, name, buildNumber } = useParams<RouteParams>();
  const [loading, setLoading] = useState(true);
  const [build, setBuild] = useState<BuildType | null>(null);
  const [selectedStep, setSelectedStep] = useState(0);

  useEffect(() => {
    if (!owner || !name || !buildNumber) return;
    
    // Mock API call
    setLoading(true);
    setTimeout(() => {
      const mockBuild = getMockBuild(owner, name, buildNumber);
      setBuild(mockBuild);
      setLoading(false);
    }, 1000);
  }, [owner, name, buildNumber]);

  const cancelBuild = () => {
    console.log('Cancel build');
  };

  const restartBuild = () => {
    console.log('Restart build');
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900"></div>
      </div>
    );
  }

  if (!build) {
    return (
      <div className="text-center py-12">
        <h3 className="text-lg font-medium text-gray-900 mb-2">Build not found</h3>
        <p className="text-gray-600">Please check if the build number is correct</p>
      </div>
    );
  }

  const selectedStepData = build.steps[selectedStep];

  return (
    <div className="fade-in">
      {/* Page Header */}
      <div className="section-header">
        <div className="flex items-center justify-between">
          <div>
            <div className="flex items-center mb-2">
              <h1 className="text-xl font-medium text-gray-900">Build #{build.number}</h1>
              <div className="ml-4">
                <BuildStatus status={build.status} />
              </div>
            </div>
            <div className="flex items-center text-sm text-gray-600 space-x-6">
              <div className="flex items-center">
                <CodeBracketIcon className="h-4 w-4 mr-1" />
                <span>{build.branch}</span>
              </div>
              <span>{build.author}</span>
              <div className="flex items-center">
                <ClockIcon className="h-4 w-4 mr-1" />
                <span>{formatDuration(build.started, build.finished)}</span>
              </div>
            </div>
          </div>
          <div className="flex items-center space-x-3">
            {build.status === 'running' && (
              <button onClick={cancelBuild} className="btn-danger">
                <StopIcon className="h-4 w-4 mr-2" />
                Cancel Build
              </button>
            )}
            <button onClick={restartBuild} className="btn-secondary">
              <ArrowPathIcon className="h-4 w-4 mr-2" />
              Restart Build
            </button>
          </div>
        </div>
      </div>

      {/* Build Info */}
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6 mb-8">
        <div className="card lg:col-span-1">
          <h3 className="text-sm font-medium text-gray-900 mb-4">Build Information</h3>
          <div className="space-y-3">
            <div>
              <div className="text-sm text-gray-500">Message</div>
              <div className="font-medium">{build.message}</div>
            </div>
            <div>
              <div className="text-sm text-gray-500">Commit</div>
              <div className="font-mono text-sm">{build.commit.substring(0, 8)}</div>
            </div>
            <div>
              <div className="text-sm text-gray-500">Started</div>
              <div className="text-sm">{moment(build.started).format('MMM D, YYYY HH:mm:ss')}</div>
            </div>
            {build.finished && (
              <div>
                <div className="text-sm text-gray-500">Finished</div>
                <div className="text-sm">{moment(build.finished).format('MMM D, YYYY HH:mm:ss')}</div>
              </div>
            )}
          </div>
        </div>

        <div className="card lg:col-span-3">
          <h3 className="text-sm font-medium text-gray-900 mb-4">Build Steps</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            {build.steps.map((step, index) => (
              <button
                key={index}
                onClick={() => setSelectedStep(index)}
                className={`text-left p-4 rounded-lg border transition-colors ${
                  selectedStep === index
                    ? 'bg-gray-50 border-gray-300'
                    : 'hover:bg-gray-50 border-gray-200'
                }`}
              >
                <div className="flex items-center justify-between mb-2">
                  <span className="text-sm font-medium">{step.name}</span>
                  <BuildStatus status={step.status} showText={false} size="xs" />
                </div>
                <div className="text-xs text-gray-500">
                  {formatDuration(step.started, step.finished)}
                </div>
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Build Logs */}
      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Steps List */}
        <div className="card lg:col-span-1">
          <h3 className="text-sm font-medium text-gray-900 mb-4">Build Steps</h3>
          <div className="space-y-2">
            {build.steps.map((step, index) => (
              <button
                key={index}
                onClick={() => setSelectedStep(index)}
                className={`w-full text-left p-3 rounded-md transition-colors ${
                  selectedStep === index
                    ? 'bg-primary-100 border border-primary-200'
                    : 'hover:bg-gray-50'
                }`}
              >
                <div className="flex items-center justify-between mb-1">
                  <span className="text-sm font-medium">{step.name}</span>
                  <BuildStatus status={step.status} showText={false} size="xs" />
                </div>
                <div className="text-xs text-gray-500">
                  {formatDuration(step.started, step.finished)}
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* Log Display */}
        <div className="card lg:col-span-3">
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-sm font-medium text-gray-900">
              {selectedStepData.name} - Logs
            </h3>
            <div className="flex items-center text-xs text-gray-500">
              <ClockIcon className="h-3 w-3 mr-1" />
              {formatDuration(selectedStepData.started, selectedStepData.finished)}
            </div>
          </div>
          
          <div className="bg-gray-900 rounded-lg p-4 font-mono text-sm text-white overflow-auto max-h-96">
            {selectedStepData.logs.map((line, index) => (
              <div key={index} className="mb-1">
                <span className="text-gray-400 mr-4 select-none">
                  {(index + 1).toString().padStart(3, ' ')}
                </span>
                {line}
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default Build; 