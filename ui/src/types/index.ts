// Build related types
export type BuildStatus = 'success' | 'failure' | 'error' | 'warning' | 'pending' | 'running' | 'cancelled' | 'killed';

export interface Build {
  number: number;
  status: BuildStatus;
  started: string;
  finished: string | null;
  author: string;
  message: string;
  branch: string;
  commit: string;
  repository: {
    owner: string;
    name: string;
  };
  steps: BuildStep[];
}

export interface BuildStep {
  name: string;
  status: BuildStatus;
  started: string;
  finished: string | null;
  logs: string[];
}

// Repository related types
export interface Repository {
  owner: string;
  name: string;
  description: string;
  private: boolean;
  active: boolean;
  defaultBranch: string;
  branchCount: number;
  lastActivity: string;
  cloneUrl: string;
  branches: string[];
  builds: Build[];
}

// Statistics types
export interface Stats {
  totalBuilds: number;
  successfulBuilds: number;
  failedBuilds: number;
  runningBuilds: number;
  pendingBuilds: number;
  successRate: number;
}

// Settings types
export interface GeneralSettings {
  language: string;
  timezone: string;
  theme: 'light' | 'dark' | 'system';
}

export interface NotificationSettings {
  emailBuilds: boolean;
  emailFailures: boolean;
  emailSuccess: boolean;
  browserNotifications: boolean;
}

export interface SecuritySettings {
  twoFactorEnabled: boolean;
  sessionTimeout: string;
}

export interface ServerSettings {
  serverUrl: string;
  maxBuilds: string;
  buildTimeout: string;
}

export interface Settings {
  general: GeneralSettings;
  notifications: NotificationSettings;
  security: SecuritySettings;
  server: ServerSettings;
}

// Component props types
export interface BuildStatusProps {
  status: BuildStatus;
  showText?: boolean;
  size?: 'xs' | 'sm' | 'md' | 'lg';
}

export interface BuildCardProps {
  build: Build;
  onClick?: () => void;
}

export interface RepositoryCardProps {
  repository: Repository;
  onClick?: () => void;
}

// Navigation types
export interface NavigationItem {
  name: string;
  href: string;
  icon: React.ComponentType<React.SVGProps<SVGSVGElement>>;
}

// API response types
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

// Error types
export interface ApiError {
  message: string;
  code?: string;
  details?: Record<string, any>;
} 