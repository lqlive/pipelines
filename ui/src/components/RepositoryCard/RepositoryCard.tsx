import React from 'react';
import { Link } from 'react-router-dom';
import {
  CodeBracketIcon,
  ClockIcon,
} from '@heroicons/react/24/outline';
import moment from 'moment';
import BuildStatus from '../BuildStatus/BuildStatus';
import type { RepositoryCardProps } from '../../types';

const RepositoryCard: React.FC<RepositoryCardProps> = ({ repository, onClick }) => {
  const repoPath = `/repositories/${repository.owner}/${repository.name}`;
  const latestBuild = repository.builds?.[0];
  
  const cardContent = (
    <div className="card cursor-pointer">
      <div className="flex items-start justify-between mb-3">
        <div className="flex-1">
          <div className="flex items-center mb-2">
            <h3 className="text-sm font-medium text-gray-900">{repository.name}</h3>
            {repository.private && (
              <span className="status-badge bg-gray-100 text-gray-700 ml-2">Private</span>
            )}
            {!repository.active && (
              <span className="status-badge bg-yellow-100 text-yellow-800 ml-2">Inactive</span>
            )}
          </div>
          
          <p className="text-sm text-gray-600 mb-3">{repository.description}</p>
          
          <div className="flex items-center text-xs text-gray-500 space-x-4">
            <div className="flex items-center">
              <CodeBracketIcon className="h-3 w-3 mr-1" />
              <span>{repository.branchCount} branches</span>
            </div>
            {repository.active ? (
              <span className="text-green-600">Active</span>
            ) : (
              <span className="text-yellow-600">Inactive</span>
            )}
          </div>
        </div>
      </div>

      {latestBuild && (
        <div className="pt-3 border-t border-gray-100">
          <div className="flex items-center justify-between">
            <div className="flex items-center">
              <BuildStatus status={latestBuild.status} showText={false} size="xs" />
              <span className="ml-2 text-xs text-gray-600">
                #{latestBuild.number}
              </span>
            </div>
            <div className="flex items-center text-xs text-gray-500">
              <ClockIcon className="h-3 w-3 mr-1" />
              {moment(latestBuild.started).fromNow()}
            </div>
          </div>
        </div>
      )}
    </div>
  );

  if (onClick) {
    return (
      <div onClick={onClick}>
        {cardContent}
      </div>
    );
  }

  return (
    <Link to={repoPath}>
      {cardContent}
    </Link>
  );
};

export default RepositoryCard; 