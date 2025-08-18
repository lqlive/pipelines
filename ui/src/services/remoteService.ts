import { apiClient, OAUTH_BASE_URL } from './api';

export interface RemoteRepository {
  id: string;
  name: string;
  fullName?: string;
  url: string;
  description: string | null;
  private?: boolean;
  language?: string | null;
  stars?: number;
  forks?: number;
  lastUpdated?: string;
  defaultBranch?: string;
  cloneUrl?: string;
  enabled?: boolean;
  provider?: string;
}

export interface GitHubRepositoriesResponse {
  count: number;
  items: RemoteRepository[];
  enabledItems: RemoteRepository[];
}

export interface GitHubAuthChallenge {
  challengeUrl: string;
  state?: string;
}

/**
 * Service for managing remote Git providers (GitHub, GitLab, etc.)
 */
export class RemoteService {
  /**
   * Redirect to GitHub authorization challenge (same style as userService.loginWithProvider)
   * @param redirectUri Optional redirect target after authentication; defaults to current location
   */
  static loginWithGitHub(redirectUri?: string): void {
    const finalRedirectUri = redirectUri || window.location.href;
    const challengeUrl = `${OAUTH_BASE_URL}/api/remotes/github/authorization/challenge?redirectUri=${encodeURIComponent(finalRedirectUri)}`;
    window.location.href = challengeUrl;
  }

  /**
   * Get repositories from GitHub
   * @returns Promise with array of GitHub repositories (including enabled status)
   */
  static async getGitHubRepositories(): Promise<RemoteRepository[]> {
    try {
      const response = await apiClient.get<GitHubRepositoriesResponse>('/api/remotes/github/repositories');
      
      // Create a Set of enabled repository IDs for fast lookup
      const enabledRepoIds = new Set(response.enabledItems.map(repo => repo.id));
      
      // Mark repositories as enabled if they exist in enabledItems
      const allRepositories = response.items.map(repo => ({
        ...repo,
        enabled: enabledRepoIds.has(repo.id)
      }));
      
      return allRepositories;
    } catch (error) {
      console.error('Failed to get GitHub repositories:', error);
      throw error;
    }
  }

  /**
   * Sync repositories from GitHub (refresh the list)
   * @returns Promise with updated array of repositories
   */
  static async syncGitHubRepositories(): Promise<RemoteRepository[]> {
    try {
      // For now, just refresh the list by calling getGitHubRepositories
      // In the future, this might call a specific sync endpoint
      return await this.getGitHubRepositories();
    } catch (error) {
      console.error('Failed to sync GitHub repositories:', error);
      throw error;
    }
  }

  /**
   * Check if user is authenticated with GitHub
   * @returns Promise with authentication status
   */
  static async checkGitHubAuthStatus(): Promise<{ isAuthenticated: boolean }> {
    try {
      const response = await apiClient.get<{ isAuthenticated: boolean }>('/api/remotes/github/status');
      return response;
    } catch (error) {
      // If we get a 401, user is not authenticated
      if ((error as any)?.response?.status === 401) {
        return { isAuthenticated: false };
      }
      console.error('Failed to check GitHub auth status:', error);
      throw error;
    }
  }

  /**
   * Enable GitHub repositories by IDs
   * @param ids Array of repository IDs to enable
   * @returns Promise<void>
   */
  static async enableGitHubRepositories(ids: string[]): Promise<void> {
    try {
      await apiClient.post<void>('/api/remotes/github/repositories/enable', { ids });
    } catch (error) {
      throw error;
    }
  }

  /**
   * Enable a single GitHub repository
   * @param repoId Repository ID to enable
   * @returns Promise<void>
   */
  static async enableGitHubRepository(repoId: string): Promise<void> {
    try {
      await this.enableGitHubRepositories([repoId]);
    } catch (error) {
      throw error;
    }
  }
}
