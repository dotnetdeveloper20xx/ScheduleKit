import { Link } from 'react-router-dom';
import { PageContainer, PageHeader } from '@/components/layout/PageContainer';
import { Card, CardContent } from '@/components/ui';
import { useEventTypes } from '@/api/hooks/useEventTypes';
import { useRealtimeBookings } from '@/hooks';

export function DashboardPage() {
  const { data: eventTypes, isLoading } = useEventTypes();

  // Enable real-time booking notifications for host dashboard
  useRealtimeBookings({
    enabled: true,
  });

  const stats = [
    {
      name: 'Event Types',
      value: isLoading ? '...' : eventTypes?.length ?? 0,
      href: '/event-types',
      icon: (
        <svg
          className="h-6 w-6"
          fill="none"
          viewBox="0 0 24 24"
          strokeWidth={1.5}
          stroke="currentColor"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5"
          />
        </svg>
      ),
    },
    {
      name: 'Upcoming Bookings',
      value: 0,
      href: '/bookings',
      icon: (
        <svg
          className="h-6 w-6"
          fill="none"
          viewBox="0 0 24 24"
          strokeWidth={1.5}
          stroke="currentColor"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M9 12h3.75M9 15h3.75M9 18h3.75m3 .75H18a2.25 2.25 0 002.25-2.25V6.108c0-1.135-.845-2.098-1.976-2.192a48.424 48.424 0 00-1.123-.08m-5.801 0c-.065.21-.1.433-.1.664 0 .414.336.75.75.75h4.5a.75.75 0 00.75-.75 2.25 2.25 0 00-.1-.664m-5.8 0A2.251 2.251 0 0113.5 2.25H15c1.012 0 1.867.668 2.15 1.586m-5.8 0c-.376.023-.75.05-1.124.08C9.095 4.01 8.25 4.973 8.25 6.108V8.25m0 0H4.875c-.621 0-1.125.504-1.125 1.125v11.25c0 .621.504 1.125 1.125 1.125h9.75c.621 0 1.125-.504 1.125-1.125V9.375c0-.621-.504-1.125-1.125-1.125H8.25zM6.75 12h.008v.008H6.75V12zm0 3h.008v.008H6.75V15zm0 3h.008v.008H6.75V18z"
          />
        </svg>
      ),
    },
    {
      name: "Today's Meetings",
      value: 0,
      href: '/bookings',
      icon: (
        <svg
          className="h-6 w-6"
          fill="none"
          viewBox="0 0 24 24"
          strokeWidth={1.5}
          stroke="currentColor"
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z"
          />
        </svg>
      ),
    },
  ];

  return (
    <PageContainer>
      <PageHeader
        title="Dashboard"
        description="Welcome back! Here's an overview of your scheduling."
      />

      {/* Stats Grid */}
      <div className="mb-8 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        {stats.map((stat) => (
          <Link key={stat.name} to={stat.href}>
            <Card className="transition-shadow hover:shadow-md">
              <CardContent className="flex items-center gap-4">
                <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary-50 text-primary-600">
                  {stat.icon}
                </div>
                <div>
                  <p className="text-sm font-medium text-gray-500">{stat.name}</p>
                  <p className="text-2xl font-semibold text-gray-900">
                    {stat.value}
                  </p>
                </div>
              </CardContent>
            </Card>
          </Link>
        ))}
      </div>

      {/* Quick Actions */}
      <div className="mb-8">
        <h2 className="mb-4 text-lg font-medium text-gray-900">Quick Actions</h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <Link
            to="/event-types/new"
            className="flex items-center gap-3 rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md"
          >
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-green-50 text-green-600">
              <svg
                className="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M12 4.5v15m7.5-7.5h-15"
                />
              </svg>
            </div>
            <div>
              <p className="font-medium text-gray-900">Create Event Type</p>
              <p className="text-sm text-gray-500">Add a new meeting type</p>
            </div>
          </Link>

          <Link
            to="/availability"
            className="flex items-center gap-3 rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md"
          >
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-blue-50 text-blue-600">
              <svg
                className="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z"
                />
              </svg>
            </div>
            <div>
              <p className="font-medium text-gray-900">Set Availability</p>
              <p className="text-sm text-gray-500">Define your schedule</p>
            </div>
          </Link>

          <Link
            to="/bookings"
            className="flex items-center gap-3 rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md"
          >
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-purple-50 text-purple-600">
              <svg
                className="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M2.25 13.5h3.86a2.25 2.25 0 012.012 1.244l.256.512a2.25 2.25 0 002.013 1.244h3.218a2.25 2.25 0 002.013-1.244l.256-.512a2.25 2.25 0 012.013-1.244h3.859m-19.5.338V18a2.25 2.25 0 002.25 2.25h15A2.25 2.25 0 0021.75 18v-4.162c0-.224-.034-.447-.1-.661L19.24 5.338a2.25 2.25 0 00-2.15-1.588H6.911a2.25 2.25 0 00-2.15 1.588L2.35 13.177a2.25 2.25 0 00-.1.661z"
                />
              </svg>
            </div>
            <div>
              <p className="font-medium text-gray-900">View Bookings</p>
              <p className="text-sm text-gray-500">Manage appointments</p>
            </div>
          </Link>

          <Link
            to="/event-types"
            className="flex items-center gap-3 rounded-lg border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md"
          >
            <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-orange-50 text-orange-600">
              <svg
                className="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M13.5 6H5.25A2.25 2.25 0 003 8.25v10.5A2.25 2.25 0 005.25 21h10.5A2.25 2.25 0 0018 18.75V10.5m-10.5 6L21 3m0 0h-5.25M21 3v5.25"
                />
              </svg>
            </div>
            <div>
              <p className="font-medium text-gray-900">Share Link</p>
              <p className="text-sm text-gray-500">Copy your booking link</p>
            </div>
          </Link>
        </div>
      </div>

      {/* Getting Started */}
      {(!eventTypes || eventTypes.length === 0) && !isLoading && (
        <Card>
          <CardContent className="py-8 text-center">
            <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-primary-50">
              <svg
                className="h-8 w-8 text-primary-600"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5"
                />
              </svg>
            </div>
            <h3 className="mb-2 text-lg font-medium text-gray-900">
              Get started with ScheduleKit
            </h3>
            <p className="mb-6 text-gray-500">
              Create your first event type to start accepting bookings.
            </p>
            <Link
              to="/event-types/new"
              className="btn btn-primary inline-flex items-center gap-2"
            >
              <svg
                className="h-5 w-5"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={1.5}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M12 4.5v15m7.5-7.5h-15"
                />
              </svg>
              Create Event Type
            </Link>
          </CardContent>
        </Card>
      )}
    </PageContainer>
  );
}
