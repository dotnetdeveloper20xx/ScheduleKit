import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { useOAuthCallback } from '@/api/hooks/useOAuth';
import { getErrorMessage } from '@/api/client';

export function OAuthCallbackPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const { setAuthData } = useAuth();
  const oAuthCallback = useOAuthCallback();
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const code = searchParams.get('code');
    const state = searchParams.get('state');

    // Retrieve stored OAuth data
    const storedState = sessionStorage.getItem('oauth_state');
    const provider = sessionStorage.getItem('oauth_provider');
    const redirectUri = sessionStorage.getItem('oauth_redirect_uri');

    // Clear stored data
    sessionStorage.removeItem('oauth_state');
    sessionStorage.removeItem('oauth_provider');
    sessionStorage.removeItem('oauth_redirect_uri');

    if (!code || !state) {
      setError('Missing authorization code or state parameter.');
      return;
    }

    if (!storedState || state !== storedState) {
      setError('Invalid state parameter. This may be a CSRF attack.');
      return;
    }

    if (!provider || !redirectUri) {
      setError('Missing OAuth session data. Please try again.');
      return;
    }

    // Exchange code for tokens
    oAuthCallback.mutate(
      {
        provider,
        code,
        state,
        redirectUri,
      },
      {
        onSuccess: (data) => {
          // Store auth data
          setAuthData(data);

          // Navigate to dashboard
          navigate('/');
        },
        onError: (err) => {
          setError(getErrorMessage(err));
        },
      }
    );
  }, [searchParams, navigate, oAuthCallback, setAuthData]);

  if (error) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
        <div className="max-w-md w-full">
          <div className="bg-white rounded-lg shadow-lg p-8 text-center">
            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-red-100 mb-4">
              <svg
                className="h-6 w-6 text-red-600"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z"
                />
              </svg>
            </div>
            <h2 className="text-xl font-semibold text-gray-900 mb-2">
              Authentication Failed
            </h2>
            <p className="text-gray-500 mb-6">{error}</p>
            <button
              onClick={() => navigate('/login')}
              className="text-blue-600 hover:text-blue-500 font-medium"
            >
              Return to Login
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4">
      <div className="max-w-md w-full">
        <div className="bg-white rounded-lg shadow-lg p-8 text-center">
          <div className="animate-spin mx-auto h-12 w-12 border-4 border-blue-600 border-t-transparent rounded-full mb-4" />
          <h2 className="text-xl font-semibold text-gray-900 mb-2">
            Completing Sign In
          </h2>
          <p className="text-gray-500">Please wait...</p>
        </div>
      </div>
    </div>
  );
}
