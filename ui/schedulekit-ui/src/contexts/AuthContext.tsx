import {
  createContext,
  useContext,
  useState,
  useEffect,
  useCallback,
  type ReactNode,
} from 'react';
import { authApi } from '@/api/auth';
import type { UserResponse, LoginRequest, RegisterRequest } from '@/api/types';

interface AuthContextType {
  user: UserResponse | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => void;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

const TOKEN_KEY = 'schedulekit_token';
const USER_KEY = 'schedulekit_user';

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserResponse | null>(() => {
    const savedUser = localStorage.getItem(USER_KEY);
    return savedUser ? JSON.parse(savedUser) : null;
  });
  const [isLoading, setIsLoading] = useState(true);

  const isAuthenticated = !!user;

  // Get token from localStorage
  const getToken = useCallback(() => {
    return localStorage.getItem(TOKEN_KEY);
  }, []);

  // Save auth state
  const saveAuth = useCallback((token: string, userData: UserResponse) => {
    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(USER_KEY, JSON.stringify(userData));
    setUser(userData);
  }, []);

  // Clear auth state
  const clearAuth = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setUser(null);
  }, []);

  // Login
  const login = useCallback(
    async (request: LoginRequest) => {
      const response = await authApi.login(request);
      saveAuth(response.accessToken, response.user);
    },
    [saveAuth]
  );

  // Register
  const register = useCallback(
    async (request: RegisterRequest) => {
      const response = await authApi.register(request);
      saveAuth(response.accessToken, response.user);
    },
    [saveAuth]
  );

  // Logout
  const logout = useCallback(() => {
    clearAuth();
  }, [clearAuth]);

  // Refresh user data
  const refreshUser = useCallback(async () => {
    try {
      const userData = await authApi.getCurrentUser();
      setUser(userData);
      localStorage.setItem(USER_KEY, JSON.stringify(userData));
    } catch {
      clearAuth();
    }
  }, [clearAuth]);

  // Check auth state on mount
  useEffect(() => {
    const initAuth = async () => {
      const token = getToken();
      if (token) {
        try {
          const userData = await authApi.getCurrentUser();
          setUser(userData);
          localStorage.setItem(USER_KEY, JSON.stringify(userData));
        } catch {
          clearAuth();
        }
      }
      setIsLoading(false);
    };

    initAuth();
  }, [getToken, clearAuth]);

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        isLoading,
        login,
        register,
        logout,
        refreshUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

// Get token for API client
export function getAuthToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}
