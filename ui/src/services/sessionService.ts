import { apiClient } from './api';
export interface Session {
  id: string;
  userId: string;
  sessionToken: string;
  status: 'Active' | 'Inactive' | 'Expired';
  createdAt: string;
  expiresAt: string;
  lastActiveAt: string;
  ipAddress?: string;
  userAgent?: string;
  deviceType?: string;
  deviceName?: string;
  location?: string;
  terminatedAt?: string;
  terminationReason?: string;
}

export class SessionService {
  /**
   * Get all active sessions for the current user
   * @returns Promise<Session[]> List of sessions
   */
  static async getSessions(): Promise<Session[]> {
    try {
      const data = await apiClient.get<Session[]>('/api/sessions');
      return data;
    } catch (error) {
      console.error('Failed to fetch sessions:', error);
      throw error;
    }
  }

  /**
   * Get device type display name
   */
  static getDeviceTypeDisplay(deviceType?: string): string {
    switch (deviceType) {
      case 'Mobile':
        return 'Mobile Device';
      case 'Tablet':
        return 'Tablet';
      case 'Desktop':
        return 'Desktop';
      default:
        return 'Unknown Device';
    }
  }

  /**
   * Get browser name from user agent
   */
  static getBrowserName(userAgent?: string): string {
    if (!userAgent) return 'Unknown Browser';
    
    const lowerUA = userAgent.toLowerCase();
    if (lowerUA.includes('chrome')) return 'Chrome';
    if (lowerUA.includes('firefox')) return 'Firefox';
    if (lowerUA.includes('safari')) return 'Safari';
    if (lowerUA.includes('edge')) return 'Edge';
    if (lowerUA.includes('opera')) return 'Opera';
    
    return 'Unknown Browser';
  }

  /**
   * Format last active time
   */
  static formatLastActive(lastActiveAt: string): string {
    const lastActive = new Date(lastActiveAt);
    const now = new Date();
    const diffMs = now.getTime() - lastActive.getTime();
    const diffMins = Math.floor(diffMs / (1000 * 60));
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 1) return 'Active now';
    if (diffMins < 60) return `${diffMins} minutes ago`;
    if (diffHours < 24) return `${diffHours} hours ago`;
    if (diffDays < 7) return `${diffDays} days ago`;
    
    return lastActive.toLocaleDateString();
  }

  /**
   * Delete a session by ID
   * @param id Session ID to delete
   */
  static async deleteSession(id: string): Promise<void> {
    try {
      await apiClient.delete(`/api/sessions/${id}`);
    } catch (error) {
      console.error('Failed to delete session:', error);
      throw error;
    }
  }

  /**
   * Get current session token from cookies
   */
  static getCurrentSessionToken(): string | null {
    // Try different possible cookie names for session token
    const cookies = document.cookie.split(';');
    const possibleNames = [
      'Pipelines.Session',
      '.AspNetCore.Cookies', 
      'ASP.NET_SessionId',
      'ASPXAUTH'
    ];
    
    for (let cookie of cookies) {
      const [name, value] = cookie.trim().split('=');
      
      // Check for exact match or starts with pattern
      if (possibleNames.includes(name) || name.startsWith('Pipelines.Session-')) {
        return decodeURIComponent(value);
      }
    }
    return null;
  }

  /**
   * Check if session is current session (by comparing session token)
   */
  static isCurrentSession(session: Session): boolean {
    const currentToken = this.getCurrentSessionToken();
    
    if (!currentToken) {
      // If no token found, let the parent component handle which one is current
      return false;
    }
    
    // Compare session tokens
    return session.sessionToken === currentToken;
  }
}
