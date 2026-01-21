import { useState, useEffect } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { PageContainer } from '@/components/layout/PageContainer';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { Select } from '@/components/ui/Select';
import { Card } from '@/components/ui/Card';
import { userApi } from '@/api/auth';
import { getErrorMessage } from '@/api/client';
import type { UserProfileResponse } from '@/api/types';

const TIMEZONES = [
  { value: 'UTC', label: 'UTC' },
  { value: 'America/New_York', label: 'Eastern Time (US & Canada)' },
  { value: 'America/Chicago', label: 'Central Time (US & Canada)' },
  { value: 'America/Denver', label: 'Mountain Time (US & Canada)' },
  { value: 'America/Los_Angeles', label: 'Pacific Time (US & Canada)' },
  { value: 'Europe/London', label: 'London' },
  { value: 'Europe/Paris', label: 'Paris' },
  { value: 'Europe/Berlin', label: 'Berlin' },
  { value: 'Asia/Tokyo', label: 'Tokyo' },
  { value: 'Asia/Shanghai', label: 'Shanghai' },
  { value: 'Australia/Sydney', label: 'Sydney' },
];

export function SettingsPage() {
  const queryClient = useQueryClient();
  const [activeTab, setActiveTab] = useState<'profile' | 'notifications'>('profile');
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // Profile form state
  const [profileForm, setProfileForm] = useState({
    name: '',
    slug: '',
    timezone: 'UTC',
  });

  // Notification form state
  const [notificationForm, setNotificationForm] = useState({
    emailNotificationsEnabled: true,
    reminderEmailsEnabled: true,
    reminderHoursBefore: 24,
  });

  // Fetch user profile
  const { data: profile, isLoading } = useQuery<UserProfileResponse>({
    queryKey: ['userProfile'],
    queryFn: userApi.getProfile,
  });

  // Update form when profile loads
  useEffect(() => {
    if (profile) {
      setProfileForm({
        name: profile.name,
        slug: profile.slug || '',
        timezone: profile.timezone,
      });
      setNotificationForm({
        emailNotificationsEnabled: profile.emailNotificationsEnabled,
        reminderEmailsEnabled: profile.reminderEmailsEnabled,
        reminderHoursBefore: profile.reminderHoursBefore,
      });
    }
  }, [profile]);

  // Update profile mutation
  const updateProfileMutation = useMutation({
    mutationFn: async () => {
      await userApi.updateProfile({
        name: profileForm.name,
        slug: profileForm.slug || undefined,
      });
      await userApi.updateTimezone({ timezone: profileForm.timezone });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userProfile'] });
      setSuccess('Profile updated successfully');
      setError(null);
      setTimeout(() => setSuccess(null), 3000);
    },
    onError: (err) => {
      setError(getErrorMessage(err));
      setSuccess(null);
    },
  });

  // Update notifications mutation
  const updateNotificationsMutation = useMutation({
    mutationFn: () => userApi.updateEmailPreferences(notificationForm),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['userProfile'] });
      setSuccess('Notification preferences updated');
      setError(null);
      setTimeout(() => setSuccess(null), 3000);
    },
    onError: (err) => {
      setError(getErrorMessage(err));
      setSuccess(null);
    },
  });

  const handleProfileSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateProfileMutation.mutate();
  };

  const handleNotificationsSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    updateNotificationsMutation.mutate();
  };

  if (isLoading) {
    return (
      <PageContainer title="Settings">
        <div className="flex items-center justify-center h-64">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600" />
        </div>
      </PageContainer>
    );
  }

  return (
    <PageContainer title="Settings">
      {/* Success/Error Messages */}
      {success && (
        <div className="mb-4 rounded-md bg-green-50 border border-green-200 p-4">
          <p className="text-sm text-green-600">{success}</p>
        </div>
      )}
      {error && (
        <div className="mb-4 rounded-md bg-red-50 border border-red-200 p-4">
          <p className="text-sm text-red-600">{error}</p>
        </div>
      )}

      {/* Tabs */}
      <div className="mb-6 border-b border-gray-200">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => setActiveTab('profile')}
            className={`py-4 px-1 text-sm font-medium border-b-2 ${
              activeTab === 'profile'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Profile
          </button>
          <button
            onClick={() => setActiveTab('notifications')}
            className={`py-4 px-1 text-sm font-medium border-b-2 ${
              activeTab === 'notifications'
                ? 'border-blue-500 text-blue-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            }`}
          >
            Notifications
          </button>
        </nav>
      </div>

      {/* Profile Tab */}
      {activeTab === 'profile' && (
        <Card>
          <form onSubmit={handleProfileSubmit} className="space-y-6 p-6">
            <h2 className="text-lg font-medium text-gray-900">Profile Information</h2>

            <Input
              label="Email"
              type="email"
              value={profile?.email || ''}
              disabled
              hint="Email cannot be changed"
            />

            <Input
              label="Name"
              value={profileForm.name}
              onChange={(e) => setProfileForm({ ...profileForm, name: e.target.value })}
              required
            />

            <Input
              label="Username"
              value={profileForm.slug}
              onChange={(e) => setProfileForm({ ...profileForm, slug: e.target.value.toLowerCase() })}
              hint="Your public booking URL: schedulekit.com/{username}"
              placeholder="john-doe"
            />

            <Select
              label="Timezone"
              value={profileForm.timezone}
              onChange={(e) => setProfileForm({ ...profileForm, timezone: e.target.value })}
              options={TIMEZONES}
            />

            <div className="flex justify-end">
              <Button
                type="submit"
                disabled={updateProfileMutation.isPending}
              >
                {updateProfileMutation.isPending ? 'Saving...' : 'Save Changes'}
              </Button>
            </div>
          </form>
        </Card>
      )}

      {/* Notifications Tab */}
      {activeTab === 'notifications' && (
        <Card>
          <form onSubmit={handleNotificationsSubmit} className="space-y-6 p-6">
            <h2 className="text-lg font-medium text-gray-900">Email Notifications</h2>

            <div className="space-y-4">
              <label className="flex items-center gap-3">
                <input
                  type="checkbox"
                  checked={notificationForm.emailNotificationsEnabled}
                  onChange={(e) =>
                    setNotificationForm({
                      ...notificationForm,
                      emailNotificationsEnabled: e.target.checked,
                    })
                  }
                  className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                />
                <span className="text-sm text-gray-700">
                  Receive email notifications for new bookings
                </span>
              </label>

              <label className="flex items-center gap-3">
                <input
                  type="checkbox"
                  checked={notificationForm.reminderEmailsEnabled}
                  onChange={(e) =>
                    setNotificationForm({
                      ...notificationForm,
                      reminderEmailsEnabled: e.target.checked,
                    })
                  }
                  className="h-4 w-4 rounded border-gray-300 text-blue-600 focus:ring-blue-500"
                />
                <span className="text-sm text-gray-700">
                  Receive reminder emails before events
                </span>
              </label>
            </div>

            {notificationForm.reminderEmailsEnabled && (
              <div className="pl-7">
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Reminder timing
                </label>
                <select
                  value={notificationForm.reminderHoursBefore}
                  onChange={(e) =>
                    setNotificationForm({
                      ...notificationForm,
                      reminderHoursBefore: parseInt(e.target.value, 10),
                    })
                  }
                  className="block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm"
                >
                  <option value={1}>1 hour before</option>
                  <option value={2}>2 hours before</option>
                  <option value={4}>4 hours before</option>
                  <option value={12}>12 hours before</option>
                  <option value={24}>24 hours before</option>
                  <option value={48}>48 hours before</option>
                  <option value={168}>1 week before</option>
                </select>
              </div>
            )}

            <div className="flex justify-end">
              <Button
                type="submit"
                disabled={updateNotificationsMutation.isPending}
              >
                {updateNotificationsMutation.isPending ? 'Saving...' : 'Save Preferences'}
              </Button>
            </div>
          </form>
        </Card>
      )}
    </PageContainer>
  );
}
