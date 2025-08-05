import { apiClient, OAUTH_BASE_URL } from './api';

// Login request interface
export interface LoginRequest {
  email: string;
  password: string;
}

// Login response interface
export interface LoginResponse {
  id: string;
  name: string;
  email: string;
  avatar?: string;
  provider: string;
  // No accessToken needed when using cookies
}

// User service class
export class UserService {
  /**
   * User login
   * @param loginData Login data
   * @returns Login response
   */
  static async login(loginData: LoginRequest): Promise<void> {
    try {
      // Login API returns empty response, just check for success
      await apiClient.post<void>('/api/users/login', loginData);
    } catch (error) {
      // Re-throw error for caller to handle
      throw error;
    }
  }

  /**
   * User registration
   * @param registerData Registration data
   * @returns Registration response
   */
  static async register(registerData: {
    email: string;
    password: string;
    name: string;
  }): Promise<void> {
    try {
      // Register API returns empty response, just check for success
      await apiClient.post<void>('/api/users/register', registerData);
    } catch (error) {
      throw error;
    }
  }

  /**
   * Get current user information
   * @returns User information
   */
  static async getCurrentUser(): Promise<LoginResponse> {
    try {
      const response = await apiClient.get<LoginResponse>('/api/users/me');
      return response;
    } catch (error) {
      throw error;
    }
  }

  /**
   * User logout
   */
  static async logout(): Promise<void> {
    try {
      await apiClient.post<void>('/api/users/logout');
    } catch (error) {
      // Don't prevent local logout if API call fails
      console.warn('Logout API call failed:', error);
    }
  }

  /**
   * Initiate OAuth provider login
   * @param providerId OAuth provider ID (e.g., 'microsoft', 'google', 'github')
   * @param redirectUri URL to redirect to after successful authentication
   */
  static loginWithProvider(providerId: string, redirectUri?: string): void {
    const finalRedirectUri = redirectUri || window.location.origin + '/';
    const challengeUrl = `${OAUTH_BASE_URL}/api/users/login/challenge?provider=${providerId}&redirectUri=${encodeURIComponent(finalRedirectUri)}`;
    
    // Redirect to OAuth provider challenge endpoint
    window.location.href = challengeUrl;
  }
}