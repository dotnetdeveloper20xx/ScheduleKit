import axios, { AxiosError, type AxiosInstance } from 'axios';
import type { ApiError } from './types';

// Create axios instance with default config
export const apiClient: AxiosInstance = axios.create({
  baseURL: '/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000,
});

// Get token from localStorage (must be imported dynamically to avoid circular dependency)
function getAuthToken(): string | null {
  return localStorage.getItem('schedulekit_token');
}

// Request interceptor for auth
apiClient.interceptors.request.use(
  (config) => {
    const token = getAuthToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ApiError>) => {
    // Handle specific error codes
    if (error.response?.status === 401) {
      // TODO: Handle unauthorized - redirect to login
      console.error('Unauthorized - redirect to login');
    }

    if (error.response?.status === 403) {
      // TODO: Handle forbidden
      console.error('Forbidden - insufficient permissions');
    }

    return Promise.reject(error);
  }
);

// Helper function to extract error message
export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const apiError = error.response?.data as ApiError | undefined;
    if (apiError?.message) {
      return apiError.message;
    }
    if (apiError?.details && apiError.details.length > 0) {
      return apiError.details.map((d) => d.message).join(', ');
    }
    return error.message;
  }
  if (error instanceof Error) {
    return error.message;
  }
  return 'An unexpected error occurred';
}
