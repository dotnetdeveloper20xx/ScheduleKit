import { useState } from 'react';
import { Link } from 'react-router-dom';
import { format, parseISO, isPast, isFuture } from 'date-fns';
import { PageContainer, PageHeader } from '@/components/layout/PageContainer';
import { Button, Card, CardContent, SkeletonBookingCard } from '@/components/ui';
import { useBookings, useCancelBooking } from '@/api/hooks/useBookings';
import type { BookingStatus } from '@/api/types';
import { cn } from '@/lib/utils';
import { useRealtimeBookings } from '@/hooks';

const statusColors: Record<BookingStatus, { bg: string; text: string }> = {
  Confirmed: { bg: 'bg-green-100', text: 'text-green-700' },
  Cancelled: { bg: 'bg-red-100', text: 'text-red-700' },
  Completed: { bg: 'bg-blue-100', text: 'text-blue-700' },
  NoShow: { bg: 'bg-yellow-100', text: 'text-yellow-700' },
};

const statusFilters: { value: string; label: string }[] = [
  { value: '', label: 'All' },
  { value: 'Confirmed', label: 'Confirmed' },
  { value: 'Cancelled', label: 'Cancelled' },
  { value: 'Completed', label: 'Completed' },
  { value: 'NoShow', label: 'No Show' },
];

export function BookingsListPage() {
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [page, setPage] = useState(1);

  // Enable real-time booking notifications
  useRealtimeBookings({
    enabled: true,
  });

  const { data, isLoading } = useBookings({
    page,
    pageSize: 20,
    status: statusFilter || undefined,
  });

  const cancelBooking = useCancelBooking();

  const handleCancel = (bookingId: string) => {
    if (window.confirm('Are you sure you want to cancel this booking?')) {
      cancelBooking.mutate({ id: bookingId });
    }
  };

  return (
    <PageContainer>
      <PageHeader
        title="Bookings"
        description="Manage your upcoming and past bookings."
      />

      {/* Filters */}
      <div className="mb-6 flex items-center gap-4">
        <div className="flex items-center gap-2">
          <span className="text-sm font-medium text-gray-700">Status:</span>
          <div className="flex rounded-lg border border-gray-200 bg-gray-50 p-1">
            {statusFilters.map((filter) => (
              <button
                key={filter.value}
                onClick={() => {
                  setStatusFilter(filter.value);
                  setPage(1);
                }}
                className={cn(
                  'rounded-md px-3 py-1.5 text-sm font-medium transition-colors',
                  statusFilter === filter.value
                    ? 'bg-white text-gray-900 shadow-sm'
                    : 'text-gray-600 hover:text-gray-900'
                )}
              >
                {filter.label}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Bookings List */}
      {isLoading ? (
        <div className="space-y-4">
          {[1, 2, 3, 4, 5].map((i) => (
            <SkeletonBookingCard key={i} />
          ))}
        </div>
      ) : data?.items && data.items.length > 0 ? (
        <div className="space-y-4">
          {data.items.map((booking) => {
            const startTime = parseISO(booking.startTimeUtc);
            const endTime = parseISO(booking.endTimeUtc);
            const isUpcoming = isFuture(startTime);
            const isPastBooking = isPast(endTime);
            const canCancel = booking.status === 'Confirmed' && isUpcoming;

            return (
              <Card key={booking.id}>
                <CardContent className="p-4">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      <div className="flex items-center gap-3">
                        <h3 className="font-medium text-gray-900">
                          {booking.guestName}
                        </h3>
                        <span
                          className={cn(
                            'inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium',
                            statusColors[booking.status].bg,
                            statusColors[booking.status].text
                          )}
                        >
                          {booking.status}
                        </span>
                        {isUpcoming && booking.status === 'Confirmed' && (
                          <span className="inline-flex items-center rounded-full bg-primary-100 px-2.5 py-0.5 text-xs font-medium text-primary-700">
                            Upcoming
                          </span>
                        )}
                      </div>

                      <p className="mt-1 text-sm text-gray-500">
                        {booking.eventTypeName}
                      </p>

                      <div className="mt-2 flex items-center gap-4 text-sm text-gray-600">
                        <span className="flex items-center gap-1">
                          <svg
                            className="h-4 w-4"
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
                          {format(startTime, 'EEEE, MMMM d, yyyy')}
                        </span>
                        <span className="flex items-center gap-1">
                          <svg
                            className="h-4 w-4"
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
                          {format(startTime, 'h:mm a')} - {format(endTime, 'h:mm a')}
                        </span>
                      </div>

                      <div className="mt-2 flex items-center gap-4 text-sm text-gray-500">
                        <span className="flex items-center gap-1">
                          <svg
                            className="h-4 w-4"
                            fill="none"
                            viewBox="0 0 24 24"
                            strokeWidth={1.5}
                            stroke="currentColor"
                          >
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              d="M21.75 6.75v10.5a2.25 2.25 0 01-2.25 2.25h-15a2.25 2.25 0 01-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0019.5 4.5h-15a2.25 2.25 0 00-2.25 2.25m19.5 0v.243a2.25 2.25 0 01-1.07 1.916l-7.5 4.615a2.25 2.25 0 01-2.36 0L3.32 8.91a2.25 2.25 0 01-1.07-1.916V6.75"
                            />
                          </svg>
                          {booking.guestEmail}
                        </span>
                        {booking.guestPhone && (
                          <span className="flex items-center gap-1">
                            <svg
                              className="h-4 w-4"
                              fill="none"
                              viewBox="0 0 24 24"
                              strokeWidth={1.5}
                              stroke="currentColor"
                            >
                              <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                d="M2.25 6.75c0 8.284 6.716 15 15 15h2.25a2.25 2.25 0 002.25-2.25v-1.372c0-.516-.351-.966-.852-1.091l-4.423-1.106c-.44-.11-.902.055-1.173.417l-.97 1.293c-.282.376-.769.542-1.21.38a12.035 12.035 0 01-7.143-7.143c-.162-.441.004-.928.38-1.21l1.293-.97c.363-.271.527-.734.417-1.173L6.963 3.102a1.125 1.125 0 00-1.091-.852H4.5A2.25 2.25 0 002.25 4.5v2.25z"
                              />
                            </svg>
                            {booking.guestPhone}
                          </span>
                        )}
                      </div>

                      {booking.guestNotes && (
                        <p className="mt-2 rounded-lg bg-gray-50 p-2 text-sm text-gray-600">
                          {booking.guestNotes}
                        </p>
                      )}

                      {booking.cancellationReason && (
                        <p className="mt-2 rounded-lg bg-red-50 p-2 text-sm text-red-700">
                          Cancelled: {booking.cancellationReason}
                        </p>
                      )}
                    </div>

                    <div className="flex items-center gap-2">
                      <Link to={`/bookings/${booking.id}`}>
                        <Button variant="outline" size="sm">
                          View
                        </Button>
                      </Link>
                      {canCancel && (
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleCancel(booking.id)}
                          disabled={cancelBooking.isPending}
                        >
                          Cancel
                        </Button>
                      )}
                    </div>
                  </div>
                </CardContent>
              </Card>
            );
          })}

          {/* Pagination */}
          {data.totalPages > 1 && (
            <div className="flex items-center justify-between border-t border-gray-200 pt-4">
              <p className="text-sm text-gray-500">
                Showing {(page - 1) * 20 + 1} to{' '}
                {Math.min(page * 20, data.totalCount)} of {data.totalCount}{' '}
                bookings
              </p>
              <div className="flex items-center gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={page === 1}
                  onClick={() => setPage((p) => p - 1)}
                >
                  Previous
                </Button>
                <span className="text-sm text-gray-600">
                  Page {page} of {data.totalPages}
                </span>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={page === data.totalPages}
                  onClick={() => setPage((p) => p + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </div>
      ) : (
        <Card>
          <CardContent className="py-12 text-center">
            <svg
              className="mx-auto h-12 w-12 text-gray-300"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={1}
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5"
              />
            </svg>
            <h3 className="mt-4 text-lg font-medium text-gray-900">
              No bookings yet
            </h3>
            <p className="mt-2 text-sm text-gray-500">
              When guests book time with you, their bookings will appear here.
            </p>
          </CardContent>
        </Card>
      )}
    </PageContainer>
  );
}
