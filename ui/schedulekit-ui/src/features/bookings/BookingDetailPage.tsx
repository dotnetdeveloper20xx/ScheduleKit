import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { format, parseISO, isFuture } from 'date-fns';
import { PageContainer } from '@/components/layout/PageContainer';
import { Button, Card, CardContent, CardHeader, TextArea } from '@/components/ui';
import { useBooking, useCancelBooking } from '@/api/hooks/useBookings';
import type { BookingStatus } from '@/api/types';
import { cn } from '@/lib/utils';

const statusColors: Record<BookingStatus, { bg: string; text: string }> = {
  Confirmed: { bg: 'bg-green-100', text: 'text-green-700' },
  Cancelled: { bg: 'bg-red-100', text: 'text-red-700' },
  Completed: { bg: 'bg-blue-100', text: 'text-blue-700' },
  NoShow: { bg: 'bg-yellow-100', text: 'text-yellow-700' },
};

export function BookingDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data: booking, isLoading, error } = useBooking(id ?? '');
  const cancelBooking = useCancelBooking();

  const [showCancelDialog, setShowCancelDialog] = useState(false);
  const [cancelReason, setCancelReason] = useState('');

  const handleCancel = () => {
    if (!id) return;
    cancelBooking.mutate(
      { id, reason: cancelReason || undefined },
      {
        onSuccess: () => {
          setShowCancelDialog(false);
          setCancelReason('');
        },
      }
    );
  };

  if (isLoading) {
    return (
      <PageContainer>
        <div className="animate-pulse space-y-6">
          <div className="h-8 w-48 rounded bg-gray-200" />
          <div className="h-64 rounded-lg bg-gray-100" />
        </div>
      </PageContainer>
    );
  }

  if (error || !booking) {
    return (
      <PageContainer>
        <Card>
          <CardContent className="py-12 text-center">
            <h3 className="text-lg font-medium text-gray-900">
              Booking not found
            </h3>
            <p className="mt-2 text-sm text-gray-500">
              This booking may have been deleted or you don't have access to it.
            </p>
            <Link to="/bookings" className="mt-4 inline-block">
              <Button>Back to Bookings</Button>
            </Link>
          </CardContent>
        </Card>
      </PageContainer>
    );
  }

  const startTime = parseISO(booking.startTimeUtc);
  const endTime = parseISO(booking.endTimeUtc);
  const isUpcoming = isFuture(startTime);
  const canCancel = booking.status === 'Confirmed' && isUpcoming;

  return (
    <PageContainer>
      {/* Header */}
      <div className="mb-6">
        <Link
          to="/bookings"
          className="mb-4 flex items-center gap-1 text-sm text-gray-600 hover:text-gray-900"
        >
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
              d="M15.75 19.5L8.25 12l7.5-7.5"
            />
          </svg>
          Back to Bookings
        </Link>
        <div className="flex items-center justify-between">
          <div>
            <div className="flex items-center gap-3">
              <h1 className="text-2xl font-semibold text-gray-900">
                {booking.guestName}
              </h1>
              <span
                className={cn(
                  'inline-flex items-center rounded-full px-3 py-1 text-sm font-medium',
                  statusColors[booking.status].bg,
                  statusColors[booking.status].text
                )}
              >
                {booking.status}
              </span>
            </div>
            <p className="mt-1 text-gray-500">{booking.eventTypeName}</p>
          </div>
          {canCancel && (
            <Button
              variant="outline"
              onClick={() => setShowCancelDialog(true)}
            >
              Cancel Booking
            </Button>
          )}
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        {/* Main Details */}
        <div className="lg:col-span-2 space-y-6">
          {/* Date & Time */}
          <Card>
            <CardHeader>
              <h3 className="text-lg font-medium text-gray-900">
                Date & Time
              </h3>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="flex items-start gap-4">
                  <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-primary-100">
                    <svg
                      className="h-5 w-5 text-primary-600"
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
                  <div>
                    <p className="font-medium text-gray-900">
                      {format(startTime, 'EEEE, MMMM d, yyyy')}
                    </p>
                    <p className="text-sm text-gray-500">
                      {format(startTime, 'h:mm a')} - {format(endTime, 'h:mm a')}
                    </p>
                    <p className="text-xs text-gray-400 mt-1">
                      Guest timezone: {booking.guestTimezone}
                    </p>
                  </div>
                </div>

                {booking.meetingLink && (
                  <div className="flex items-start gap-4">
                    <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-blue-100">
                      <svg
                        className="h-5 w-5 text-blue-600"
                        fill="none"
                        viewBox="0 0 24 24"
                        strokeWidth={1.5}
                        stroke="currentColor"
                      >
                        <path
                          strokeLinecap="round"
                          strokeLinejoin="round"
                          d="M13.19 8.688a4.5 4.5 0 011.242 7.244l-4.5 4.5a4.5 4.5 0 01-6.364-6.364l1.757-1.757m13.35-.622l1.757-1.757a4.5 4.5 0 00-6.364-6.364l-4.5 4.5a4.5 4.5 0 001.242 7.244"
                        />
                      </svg>
                    </div>
                    <div>
                      <p className="font-medium text-gray-900">Meeting Link</p>
                      <a
                        href={booking.meetingLink}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-primary-600 hover:text-primary-700"
                      >
                        {booking.meetingLink}
                      </a>
                    </div>
                  </div>
                )}
              </div>
            </CardContent>
          </Card>

          {/* Guest Notes */}
          {booking.guestNotes && (
            <Card>
              <CardHeader>
                <h3 className="text-lg font-medium text-gray-900">
                  Guest Notes
                </h3>
              </CardHeader>
              <CardContent>
                <p className="text-gray-600 whitespace-pre-wrap">
                  {booking.guestNotes}
                </p>
              </CardContent>
            </Card>
          )}

          {/* Question Responses */}
          {booking.responses && booking.responses.length > 0 && (
            <Card>
              <CardHeader>
                <h3 className="text-lg font-medium text-gray-900">
                  Additional Information
                </h3>
              </CardHeader>
              <CardContent>
                <dl className="space-y-4">
                  {booking.responses.map((response) => (
                    <div key={response.questionId}>
                      <dt className="text-sm font-medium text-gray-700">
                        {response.questionText || 'Question'}
                      </dt>
                      <dd className="mt-1 text-sm text-gray-600">
                        {response.responseValue}
                      </dd>
                    </div>
                  ))}
                </dl>
              </CardContent>
            </Card>
          )}

          {/* Cancellation Info */}
          {booking.status === 'Cancelled' && (
            <Card>
              <CardHeader>
                <h3 className="text-lg font-medium text-red-900">
                  Cancellation Details
                </h3>
              </CardHeader>
              <CardContent>
                <p className="text-sm text-gray-600">
                  Cancelled on{' '}
                  {booking.cancelledAtUtc
                    ? format(parseISO(booking.cancelledAtUtc), 'MMMM d, yyyy h:mm a')
                    : 'Unknown date'}
                </p>
                {booking.cancellationReason && (
                  <p className="mt-2 text-sm text-gray-600">
                    Reason: {booking.cancellationReason}
                  </p>
                )}
              </CardContent>
            </Card>
          )}
        </div>

        {/* Sidebar - Guest Info */}
        <div>
          <Card>
            <CardHeader>
              <h3 className="text-lg font-medium text-gray-900">Guest</h3>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 items-center justify-center rounded-full bg-gray-200">
                    <svg
                      className="h-6 w-6 text-gray-600"
                      fill="none"
                      viewBox="0 0 24 24"
                      strokeWidth={1.5}
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        d="M15.75 6a3.75 3.75 0 11-7.5 0 3.75 3.75 0 017.5 0zM4.501 20.118a7.5 7.5 0 0114.998 0A17.933 17.933 0 0112 21.75c-2.676 0-5.216-.584-7.499-1.632z"
                      />
                    </svg>
                  </div>
                  <div>
                    <p className="font-medium text-gray-900">
                      {booking.guestName}
                    </p>
                    <p className="text-sm text-gray-500">{booking.guestEmail}</p>
                  </div>
                </div>

                <div className="space-y-2 pt-4 border-t border-gray-200">
                  <a
                    href={`mailto:${booking.guestEmail}`}
                    className="flex items-center gap-2 text-sm text-gray-600 hover:text-gray-900"
                  >
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
                  </a>

                  {booking.guestPhone && (
                    <a
                      href={`tel:${booking.guestPhone}`}
                      className="flex items-center gap-2 text-sm text-gray-600 hover:text-gray-900"
                    >
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
                    </a>
                  )}
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Booking Meta */}
          <Card className="mt-4">
            <CardContent className="p-4">
              <dl className="space-y-2 text-sm">
                <div className="flex justify-between">
                  <dt className="text-gray-500">Booking ID</dt>
                  <dd className="font-mono text-gray-900 truncate max-w-[150px]" title={booking.id}>
                    {booking.id.substring(0, 8)}...
                  </dd>
                </div>
                <div className="flex justify-between">
                  <dt className="text-gray-500">Created</dt>
                  <dd className="text-gray-900">
                    {format(parseISO(booking.createdAtUtc), 'MMM d, yyyy')}
                  </dd>
                </div>
              </dl>
            </CardContent>
          </Card>
        </div>
      </div>

      {/* Cancel Dialog */}
      {showCancelDialog && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <Card className="w-full max-w-md mx-4">
            <CardHeader>
              <h3 className="text-lg font-medium text-gray-900">
                Cancel Booking
              </h3>
              <p className="text-sm text-gray-500">
                Are you sure you want to cancel this booking with {booking.guestName}?
              </p>
            </CardHeader>
            <CardContent>
              <TextArea
                label="Cancellation Reason (optional)"
                value={cancelReason}
                onChange={(e) => setCancelReason(e.target.value)}
                placeholder="Let the guest know why you're cancelling..."
                rows={3}
              />

              <div className="mt-4 flex justify-end gap-3">
                <Button
                  variant="outline"
                  onClick={() => {
                    setShowCancelDialog(false);
                    setCancelReason('');
                  }}
                >
                  Keep Booking
                </Button>
                <Button
                  onClick={handleCancel}
                  isLoading={cancelBooking.isPending}
                  className="bg-red-600 hover:bg-red-700"
                >
                  Cancel Booking
                </Button>
              </div>
            </CardContent>
          </Card>
        </div>
      )}
    </PageContainer>
  );
}
