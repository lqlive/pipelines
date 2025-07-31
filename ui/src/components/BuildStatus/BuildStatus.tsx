import React from 'react';
import {
  CheckCircleIcon,
  XCircleIcon,
  ExclamationCircleIcon,
  ExclamationTriangleIcon,
  ClockIcon,
  PlayIcon,
  StopIcon,
  XMarkIcon,
} from '@heroicons/react/24/outline';
import type { BuildStatusProps, BuildStatus } from '../../types';

interface StatusConfig {
  icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
  color: string;
  bg: string;
  text: string;
}

const statusConfig: Record<BuildStatus, StatusConfig> = {
  success: {
    icon: CheckCircleIcon,
    color: 'text-green-600',
    bg: 'bg-green-100',
    text: 'Success',
  },
  failure: {
    icon: XCircleIcon,
    color: 'text-red-600',
    bg: 'bg-red-100',
    text: 'Failed',
  },
  error: {
    icon: ExclamationCircleIcon,
    color: 'text-red-600',
    bg: 'bg-red-100',
    text: 'Error',
  },
  warning: {
    icon: ExclamationTriangleIcon,
    color: 'text-yellow-600',
    bg: 'bg-yellow-100',
    text: 'Warning',
  },
  pending: {
    icon: ClockIcon,
    color: 'text-gray-500',
    bg: 'bg-gray-100',
    text: 'Pending',
  },
  running: {
    icon: PlayIcon,
    color: 'text-blue-600',
    bg: 'bg-blue-100',
    text: 'Running',
  },
  cancelled: {
    icon: StopIcon,
    color: 'text-gray-600',
    bg: 'bg-gray-100',
    text: 'Cancelled',
  },
  killed: {
    icon: XMarkIcon,
    color: 'text-red-600',
    bg: 'bg-red-100',
    text: 'Killed',
  },
};

const BuildStatus: React.FC<BuildStatusProps> = ({ 
  status, 
  showText = true, 
  size = 'sm' 
}) => {
  const config = statusConfig[status];
  const Icon = config.icon;

  const sizeClasses = {
    xs: 'h-3 w-3',
    sm: 'h-4 w-4',
    md: 'h-5 w-5',
    lg: 'h-6 w-6',
  };

  return (
    <div className="flex items-center">
      <Icon className={`${sizeClasses[size]} ${config.color}`} />
      {showText && (
        <span className={`ml-2 text-sm font-medium ${config.color}`}>
          {config.text}
        </span>
      )}
    </div>
  );
};

export default BuildStatus; 