import { useEffect, useState } from 'react';
import { useNavigate, useParams, useSearchParams } from 'react-router-dom';
import { Button } from '@/components/ui/Button';

function ProviderLogo({ provider }: { provider: string }) {
  switch (provider) {
    case 'google':
      return (
        <svg className="h-12 w-12" viewBox="0 0 24 24">
          <path
            fill="#4285F4"
            d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"
          />
          <path
            fill="#34A853"
            d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
          />
          <path
            fill="#FBBC05"
            d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
          />
          <path
            fill="#EA4335"
            d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
          />
        </svg>
      );
    case 'microsoft':
      return (
        <svg className="h-12 w-12" viewBox="0 0 23 23">
          <path fill="#f25022" d="M0 0h11v11H0z" />
          <path fill="#00a4ef" d="M0 12h11v11H0z" />
          <path fill="#7fba00" d="M12 0h11v11H12z" />
          <path fill="#ffb900" d="M12 12h11v11H12z" />
        </svg>
      );
    case 'github':
      return (
        <svg className="h-12 w-12" fill="currentColor" viewBox="0 0 24 24">
          <path d="M12 0c-6.626 0-12 5.373-12 12 0 5.302 3.438 9.8 8.207 11.387.599.111.793-.261.793-.577v-2.234c-3.338.726-4.033-1.416-4.033-1.416-.546-1.387-1.333-1.756-1.333-1.756-1.089-.745.083-.729.083-.729 1.205.084 1.839 1.237 1.839 1.237 1.07 1.834 2.807 1.304 3.492.997.107-.775.418-1.305.762-1.604-2.665-.305-5.467-1.334-5.467-5.931 0-1.311.469-2.381 1.236-3.221-.124-.303-.535-1.524.117-3.176 0 0 1.008-.322 3.301 1.23.957-.266 1.983-.399 3.003-.404 1.02.005 2.047.138 3.006.404 2.291-1.552 3.297-1.23 3.297-1.23.653 1.653.242 2.874.118 3.176.77.84 1.235 1.911 1.235 3.221 0 4.609-2.807 5.624-5.479 5.921.43.372.823 1.102.823 2.222v3.293c0 .319.192.694.801.576 4.765-1.589 8.199-6.086 8.199-11.386 0-6.627-5.373-12-12-12z" />
        </svg>
      );
    default:
      return null;
  }
}

function getProviderName(provider: string): string {
  switch (provider) {
    case 'google':
      return 'Google';
    case 'microsoft':
      return 'Microsoft';
    case 'github':
      return 'GitHub';
    default:
      return provider;
  }
}

function getProviderColor(provider: string): string {
  switch (provider) {
    case 'google':
      return 'bg-white border border-gray-300';
    case 'microsoft':
      return 'bg-white border border-gray-300';
    case 'github':
      return 'bg-gray-900 text-white';
    default:
      return 'bg-white border border-gray-300';
  }
}

export function MockOAuthPage() {
  const navigate = useNavigate();
  const { provider } = useParams<{ provider: string }>();
  const [searchParams] = useSearchParams();
  const [isLoading, setIsLoading] = useState(false);

  const redirectUri = searchParams.get('redirect_uri') || '';
  const state = searchParams.get('state') || '';

  useEffect(() => {
    // Validate that we have the required parameters
    if (!redirectUri || !state) {
      navigate('/login');
    }
  }, [redirectUri, state, navigate]);

  const handleContinue = () => {
    setIsLoading(true);

    // Generate a mock authorization code
    const mockCode = `mock_${provider}_${Date.now()}`;

    // Simulate a brief delay
    setTimeout(() => {
      // Redirect to our callback page with the mock code
      navigate(`/oauth/callback?code=${mockCode}&state=${encodeURIComponent(state)}`);
    }, 500);
  };

  const handleCancel = () => {
    navigate('/login');
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-100 py-12 px-4">
      <div className="max-w-md w-full">
        <div className="bg-white rounded-lg shadow-lg p-8">
          {/* Provider Logo */}
          <div className="flex justify-center mb-6">
            <ProviderLogo provider={provider || ''} />
          </div>

          <h1 className="text-center text-2xl font-bold text-gray-900 mb-2">
            Sign in with {getProviderName(provider || '')}
          </h1>

          <p className="text-center text-sm text-gray-500 mb-6">
            This is a mock OAuth login page for demo purposes
          </p>

          {/* Mock User Info */}
          <div className="bg-gray-50 rounded-lg p-4 mb-6">
            <p className="text-sm font-medium text-gray-700 mb-2">
              Signing in as:
            </p>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-blue-100 flex items-center justify-center">
                <span className="text-blue-600 font-medium">D</span>
              </div>
              <div>
                <p className="font-medium text-gray-900">Demo User</p>
                <p className="text-sm text-gray-500">
                  demo.user@{provider === 'microsoft' ? 'outlook' : provider === 'github' ? 'github.example' : 'gmail'}.com
                </p>
              </div>
            </div>
          </div>

          {/* Permission Notice */}
          <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6">
            <p className="text-sm text-yellow-800">
              <strong>ScheduleKit</strong> will receive:
            </p>
            <ul className="mt-2 text-sm text-yellow-700 list-disc list-inside">
              <li>Your email address</li>
              <li>Your name</li>
            </ul>
          </div>

          {/* Actions */}
          <div className="space-y-3">
            <Button
              onClick={handleContinue}
              className="w-full"
              disabled={isLoading}
            >
              {isLoading ? 'Signing in...' : 'Continue'}
            </Button>

            <button
              onClick={handleCancel}
              className="w-full text-sm text-gray-500 hover:text-gray-700"
            >
              Cancel
            </button>
          </div>

          {/* Demo Notice */}
          <div className="mt-6 pt-4 border-t border-gray-200">
            <p className="text-xs text-gray-400 text-center">
              This mock OAuth page simulates the login flow.
              In production, you would be redirected to {getProviderName(provider || '')}'s actual login page.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
