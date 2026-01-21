import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { format, parseISO } from 'date-fns';
import { useRescheduleInfo, useRescheduleBooking } from '@/api/hooks/useReschedule';
import { useAvailableSlots, useAvailableDates } from '@/api/hooks/useSlots';
import { Button, Card, CardContent } from '@/components/ui';
import { cn } from '@/lib/utils';

export function ReschedulePage() {
  const { token } = useParams<{ token: string }>();
  const navigate = useNavigate();
  const [selectedDate, setSelectedDate] = useState<string | null>(null);
  const [selectedSlot, setSelectedSlot] = useState<string | null>(null);
  const [isSuccess, setIsSuccess] = useState(false);

  const { data: bookingInfo, isLoading: infoLoading, error: infoError } = useRescheduleInfo(token);
  const rescheduleBooking = useRescheduleBooking();

  // Use the guest's original timezone
  const timezone = bookingInfo?.guestTimezone ?? Intl.DateTimeFormat().resolvedOptions().timeZone;

  const { data: availableDates, isLoading: datesLoading } = useAvailableDates(
    bookingInfo?.eventTypeId,
    timezone
  );

  const { data: slotsData, isLoading: slotsLoading } = useAvailableSlots(
    bookingInfo?.eventTypeId,
    selectedDate ?? undefined,
    timezone
  );

  const handleReschedule = () => {
    if (!token || !selectedSlot || !selectedDate || !slotsData) return;

    // Find the slot to get the UTC time
    const slot = slotsData.slots.find(s => s.startTime === selectedSlot);
    if (!slot) return;

    rescheduleBooking.mutate(
      {
        token,
        data: { newStartTimeUtc: slot.startTimeUtc },
      },
      {
        onSuccess: () => {
          setIsSuccess(true);
        },
      }
    );
  };

  if (infoLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="mx-auto h-8 w-8 animate-spin rounded-full border-4 border-primary-600 border-t-transparent" />
          <p className="mt-4 text-sm text-gray-500">Loading booking details...</p>
        </div>
      </div>
    );
  }

  if (infoError || !bookingInfo) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-50">
        <Card className="max-w-md">
          <CardContent className="py-12 text-center">
            <svg
              className="mx-auto h-12 w-12 text-red-400"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={1}
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z"
              />
            </svg>
            <h2 className="mt-4 text-lg font-medium text-gray-900">
              Unable to Reschedule
            </h2>
            <p className="mt-2 text-sm text-gray-500">
              This reschedule link may be invalid, expired, or the booking cannot be rescheduled.
            </p>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (isSuccess) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-50">
        <Card className="max-w-md">
          <CardContent className="py-12 text-center">
            <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-green-100">
              <svg
                className="h-8 w-8 text-green-600"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={2}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M4.5 12.75l6 6 9-13.5"
                />
              </svg>
            </div>
            <h2 className="mt-6 text-xl font-semibold text-gray-900">
              Booking Rescheduled
            </h2>
            <p className="mt-2 text-gray-500">
              Your booking has been successfully rescheduled. You will receive a confirmation email shortly.
            </p>
            <p className="mt-4 font-medium text-gray-900">
              {selectedDate && format(parseISO(selectedDate), 'EEEE, MMMM d, yyyy')}
            </p>
            <p className="text-gray-600">{selectedSlot}</p>
          </CardContent>
        </Card>
      </div>
    );
  }

  const currentTime = parseISO(bookingInfo.currentStartTimeUtc);

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="mx-auto max-w-4xl px-4">
        <Card>
          <CardContent className="p-0">
            <div className="grid md:grid-cols-3">
              {/* Current Booking Info */}
              <div className="border-b border-gray-200 p-6 md:border-b-0 md:border-r">
                <h2 className="text-lg font-semibold text-gray-900">Reschedule Booking</h2>
                <p className="mt-1 text-sm text-gray-500">{bookingInfo.eventTypeName}</p>

                <div className="mt-6 space-y-3">
                  <div>
                    <p className="text-xs font-medium uppercase text-gray-500">Current Time</p>
                    <p className="mt-1 font-medium text-gray-900">
                      {format(currentTime, 'EEEE, MMMM d, yyyy')}
                    </p>
                    <p className="text-sm text-gray-600">
                      {format(currentTime, 'h:mm a')} ({bookingInfo.durationMinutes} min)
                    </p>
                  </div>

                  <div>
                    <p className="text-xs font-medium uppercase text-gray-500">Guest</p>
                    <p className="mt-1 text-gray-900">{bookingInfo.guestName}</p>
                    <p className="text-sm text-gray-500">{bookingInfo.guestEmail}</p>
                  </div>

                  <div className="flex items-center gap-2 text-sm text-gray-600">
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
                        d="M12 21a9.004 9.004 0 008.716-6.747M12 21a9.004 9.004 0 01-8.716-6.747M12 21c2.485 0 4.5-4.03 4.5-9S14.485 3 12 3m0 18c-2.485 0-4.5-4.03-4.5-9S9.515 3 12 3m0 0a8.997 8.997 0 017.843 4.582M12 3a8.997 8.997 0 00-7.843 4.582m15.686 0A11.953 11.953 0 0112 10.5c-2.998 0-5.74-1.1-7.843-2.918m15.686 0A8.959 8.959 0 0121 12c0 .778-.099 1.533-.284 2.253m0 0A17.919 17.919 0 0112 16.5c-3.162 0-6.133-.815-8.716-2.247m0 0A9.015 9.015 0 013 12c0-1.605.42-3.113 1.157-4.418"
                      />
                    </svg>
                    {timezone}
                  </div>
                </div>
              </div>

              {/* Date Selection */}
              <div className="border-b border-gray-200 p-6 md:border-b-0 md:border-r">
                <h3 className="mb-4 text-sm font-medium text-gray-900">Select New Date</h3>
                {datesLoading ? (
                  <div className="animate-pulse space-y-2">
                    {[...Array(5)].map((_, i) => (
                      <div key={i} className="h-10 rounded bg-gray-100" />
                    ))}
                  </div>
                ) : availableDates?.dates && availableDates.dates.length > 0 ? (
                  <div className="max-h-80 space-y-2 overflow-y-auto">
                    {availableDates.dates
                      .filter((d) => d.hasAvailability)
                      .map((dateInfo) => (
                        <button
                          key={dateInfo.date}
                          onClick={() => {
                            setSelectedDate(dateInfo.date);
                            setSelectedSlot(null);
                          }}
                          className={cn(
                            'w-full rounded-lg border px-4 py-3 text-left text-sm transition-colors',
                            selectedDate === dateInfo.date
                              ? 'border-primary-500 bg-primary-50 text-primary-700'
                              : 'border-gray-200 hover:border-gray-300 hover:bg-gray-50'
                          )}
                        >
                          <span className="font-medium">
                            {format(parseISO(dateInfo.date), 'EEEE')}
                          </span>
                          <br />
                          <span className="text-gray-500">
                            {format(parseISO(dateInfo.date), 'MMMM d, yyyy')}
                          </span>
                        </button>
                      ))}
                  </div>
                ) : (
                  <p className="text-sm text-gray-500">
                    No available dates in the booking window.
                  </p>
                )}
              </div>

              {/* Time Slots */}
              <div className="p-6">
                <h3 className="mb-4 text-sm font-medium text-gray-900">
                  {selectedDate
                    ? `Available times for ${format(parseISO(selectedDate), 'MMM d')}`
                    : 'Select a date to see times'}
                </h3>
                {!selectedDate ? (
                  <p className="text-sm text-gray-500">Please select a date first.</p>
                ) : slotsLoading ? (
                  <div className="animate-pulse space-y-2">
                    {[...Array(4)].map((_, i) => (
                      <div key={i} className="h-10 rounded bg-gray-100" />
                    ))}
                  </div>
                ) : slotsData?.slots && slotsData.slots.length > 0 ? (
                  <div className="max-h-80 space-y-2 overflow-y-auto">
                    {slotsData.slots.map((slot) => (
                      <button
                        key={slot.startTime}
                        onClick={() => setSelectedSlot(slot.startTime)}
                        className={cn(
                          'w-full rounded-lg border px-4 py-3 text-sm font-medium transition-colors',
                          selectedSlot === slot.startTime
                            ? 'border-primary-500 bg-primary-50 text-primary-700'
                            : 'border-gray-200 hover:border-gray-300 hover:bg-gray-50'
                        )}
                      >
                        {slot.startTime}
                      </button>
                    ))}
                  </div>
                ) : (
                  <p className="text-sm text-gray-500">No available times for this date.</p>
                )}

                {selectedSlot && (
                  <div className="mt-6">
                    <Button
                      className="w-full"
                      onClick={handleReschedule}
                      isLoading={rescheduleBooking.isPending}
                    >
                      Confirm Reschedule
                    </Button>
                  </div>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
