import { apiClient } from './api';
import type { Repository } from '../types';

/**
 * Repository service class
 * Handles all repository-related API operations
 */
export class RepositoryService {
  /**
   * Fetch all repositories for the current user
   * @returns Promise<Repository[]> List of repositories
   */
  static async getRepositories(): Promise<Repository[]> {
    try {
      const data = await apiClient.get<Repository[]>('/api/repositories');
      return data;
    } catch (error) {
      // Re-throw error for caller to handle
      throw error;
    }
  }
}
