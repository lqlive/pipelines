import React from 'react';
import { Link } from 'react-router-dom';
import {
  CodeBracketIcon,
  ClockIcon,
} from '@heroicons/react/24/outline';
import moment from 'moment';
import BuildStatus from '../BuildStatus/BuildStatus';
import type { BuildCardProps } from '../../types';

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

const BuildCard: React.FC<BuildCardProps> = ({ build, onClick }) => {
  const buildPath = `/repositories/${build.repository.owner}/${build.repository.name}/builds/${build.number}`;
  
  const cardContent = (
    <div className="card cursor-pointer">
      <div className="flex items-start justify-between mb-3">
        <div className="flex items-center">
          <span className="text-sm font-medium text-gray-900">#{build.number}</span>
          <div className="ml-3">
            <BuildStatus status={build.status} />
          </div>
        </div>
        <div className="flex items-center text-xs text-gray-500">
          <ClockIcon className="h-3 w-3 mr-1" />
          {formatDuration(build.started, build.finished)}
        </div>
      </div>
      
      <div className="mb-3">
        <p className="text-sm text-gray-900 font-medium mb-1">
          {build.message || 'No commit message'}
        </p>
        <div className="flex items-center text-xs text-gray-500 space-x-4">
          <span>{build.author}</span>
          <div className="flex items-center">
            <CodeBracketIcon className="h-3 w-3 mr-1" />
            <span>{build.branch}</span>
          </div>
        </div>
      </div>
      
      <div className="text-xs text-gray-400">
        {moment(build.started).format('MMM D, YYYY HH:mm')}
      </div>
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
    <Link to={buildPath}>
      {cardContent}
    </Link>
  );
};

export default BuildCard; 