import { apiClient } from './client';
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  UserResponse,
  UserProfileResponse,
  UpdateProfileRequest,
  UpdateTimezoneRequest,
  UpdateEmailPreferencesRequest,
} from './types';

// Auth endpoints
export const authApi = {
  login: async (request: LoginRequest): Promise<AuthResponse> => {
    const response = await apiClient.post<AuthResponse>('/auth/login', request);
    return response.data;
  },

  register: async (request: RegisterRequest): Promise<AuthResponse> => {
    const response = await apiClient.post<AuthResponse>('/auth/register', request);
    return response.data;
  },

  getCurrentUser: async (): Promise<UserResponse> => {
    const response = await apiClient.get<UserResponse>('/auth/me');
    return response.data;
  },
};

// User settings endpoints
export const userApi = {
  getProfile: async (): Promise<UserProfileResponse> => {
    const response = await apiClient.get<UserProfileResponse>('/users/me/profile');
    return response.data;
  },

  updateProfile: async (request: UpdateProfileRequest): Promise<void> => {
    await apiClient.put('/users/me/profile', request);
  },

  updateTimezone: async (request: UpdateTimezoneRequest): Promise<void> => {
    await apiClient.put('/users/me/timezone', request);
  },

  updateEmailPreferences: async (request: UpdateEmailPreferencesRequest): Promise<void> => {
    await apiClient.put('/users/me/email-preferences', request);
  },

  getPublicProfile: async (slug: string): Promise<UserResponse> => {
    const response = await apiClient.get<UserResponse>(`/users/${slug}`);
    return response.data;
  },
};
