import { useQuery, useMutation } from '@tanstack/react-query';
import { apiClient } from '../client';
import type {
  OAuthProvidersResponse,
  OAuthAuthorizeResponse,
  OAuthCallbackRequest,
  AuthResponse,
} from '../types';

export function useOAuthProviders() {
  return useQuery({
    queryKey: ['oauth', 'providers'],
    queryFn: async () => {
      const response = await apiClient.get<OAuthProvidersResponse>(
        '/api/v1/auth/oauth/providers'
      );
      return response.data;
    },
    staleTime: 5 * 60 * 1000, // Cache for 5 minutes
  });
}

export function useOAuthAuthorize() {
  return useMutation({
    mutationFn: async ({
      provider,
      redirectUri,
    }: {
      provider: string;
      redirectUri: string;
    }) => {
      const response = await apiClient.get<OAuthAuthorizeResponse>(
        `/api/v1/auth/oauth/${provider}/authorize`,
        { params: { redirectUri } }
      );
      return response.data;
    },
  });
}

export function useOAuthCallback() {
  return useMutation({
    mutationFn: async (request: OAuthCallbackRequest) => {
      const response = await apiClient.post<AuthResponse>(
        '/api/v1/auth/oauth/callback',
        request
      );
      return response.data;
    },
  });
}
