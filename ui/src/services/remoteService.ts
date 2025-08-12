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
   * @returns Promise with array of GitHub repositories
   */
  static async getGitHubRepositories(): Promise<RemoteRepository[]> {
    try {
      const response = await apiClient.get<GitHubRepositoriesResponse>('/api/remotes/github/repositories');
      return response.items;
    } catch (error) {
      console.error('Failed to get GitHub repositories:', error);
      throw error;
    }
  }

  /**
   * Enable a repository for CI/CD
   * @param repoId Repository ID to enable
   * @returns Promise with operation result
   */
  static async enableRepository(repoId: string): Promise<void> {
    try {
      await apiClient.post(`/api/repositories/${repoId}/enable`);
    } catch (error) {
      console.error('Failed to enable repository:', error);
      throw error;
    }
  }

  /**
   * Enable multiple repositories for CI/CD
   * @param repoIds Array of repository IDs to enable
   * @returns Promise with operation results
   */
  static async enableMultipleRepositories(repoIds: string[]): Promise<void> {
    try {
      const promises = repoIds.map(repoId => 
        apiClient.post(`/api/repositories/${repoId}/enable`)
      );
      await Promise.all(promises);
    } catch (error) {
      console.error('Failed to enable repositories:', error);
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
}
