import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { format, parseISO } from 'date-fns';
import { usePublicEventType, useAvailableSlots, useAvailableDates } from '@/api/hooks/useSlots';
import { Button, Card, CardContent } from '@/components/ui';
import { cn } from '@/lib/utils';
import { BookingForm } from './BookingForm';
import { useRealtimeSlots } from '@/hooks';

export function PublicBookingPage() {
  const { hostSlug, eventSlug } = useParams<{ hostSlug: string; eventSlug: string }>();
  const [selectedDate, setSelectedDate] = useState<string | null>(null);
  const [selectedSlot, setSelectedSlot] = useState<string | null>(null);
  const [showBookingForm, setShowBookingForm] = useState(false);

  // Get user's timezone
  const timezone = Intl.DateTimeFormat().resolvedOptions().timeZone;

  const { data: eventType, isLoading: eventTypeLoading, error: eventTypeError } = usePublicEventType(
    hostSlug ?? '',
    eventSlug ?? ''
  );

  const { data: availableDates, isLoading: datesLoading } = useAvailableDates(
    eventType?.id,
    timezone
  );

  const { data: slotsData, isLoading: slotsLoading } = useAvailableSlots(
    eventType?.id,
    selectedDate ?? undefined,
    timezone
  );

  // Enable real-time slot updates via SignalR
  useRealtimeSlots({
    eventTypeId: eventType?.id ?? '',
    enabled: !!eventType?.id,
  });

  if (eventTypeLoading) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-50">
        <div className="text-center">
          <div className="mx-auto h-8 w-8 animate-spin rounded-full border-4 border-primary-600 border-t-transparent" />
          <p className="mt-4 text-sm text-gray-500">Loading...</p>
        </div>
      </div>
    );
  }

  if (eventTypeError || !eventType) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-gray-50">
        <Card className="max-w-md">
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
                d="M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z"
              />
            </svg>
            <h2 className="mt-4 text-lg font-medium text-gray-900">
              Event type not found
            </h2>
            <p className="mt-2 text-sm text-gray-500">
              This booking link may be incorrect or the event type may no longer be available.
            </p>
          </CardContent>
        </Card>
      </div>
    );
  }

  if (showBookingForm && selectedDate && selectedSlot) {
    return (
      <BookingForm
        eventType={eventType}
        selectedDate={selectedDate}
        selectedSlot={selectedSlot}
        timezone={timezone}
        onBack={() => setShowBookingForm(false)}
      />
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="mx-auto max-w-4xl px-4">
        <Card>
          <CardContent className="p-0">
            <div className="grid md:grid-cols-3">
              {/* Event Info */}
              <div className="border-b border-gray-200 p-6 md:border-b-0 md:border-r">
                <div className="mb-4">
                  <div className="flex h-12 w-12 items-center justify-center rounded-full bg-primary-100">
                    <svg
                      className="h-6 w-6 text-primary-600"
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
                </div>
                <h1 className="text-xl font-semibold text-gray-900">{eventType.name}</h1>
                {eventType.description && (
                  <p className="mt-2 text-sm text-gray-500">{eventType.description}</p>
                )}
                <div className="mt-4 space-y-2">
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
                        d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                    {eventType.durationMinutes} minutes
                  </div>
                  <div className="flex items-center gap-2 text-sm text-gray-600">
                    {eventType.locationType === 'Zoom' && (
                      <svg className="h-4 w-4" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M4.5 4.5a3 3 0 00-3 3v9a3 3 0 003 3h10.5a3 3 0 003-3v-2.25l3.75 2.5V7.25l-3.75 2.5V7.5a3 3 0 00-3-3H4.5z" />
                      </svg>
                    )}
                    {eventType.locationType === 'GoogleMeet' && (
                      <svg className="h-4 w-4" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M4.5 4.5a3 3 0 00-3 3v9a3 3 0 003 3h10.5a3 3 0 003-3v-2.25l3.75 2.5V7.25l-3.75 2.5V7.5a3 3 0 00-3-3H4.5z" />
                      </svg>
                    )}
                    {eventType.locationType === 'MicrosoftTeams' && (
                      <svg className="h-4 w-4" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M4.5 4.5a3 3 0 00-3 3v9a3 3 0 003 3h10.5a3 3 0 003-3v-2.25l3.75 2.5V7.25l-3.75 2.5V7.5a3 3 0 00-3-3H4.5z" />
                      </svg>
                    )}
                    {eventType.locationType === 'Phone' && (
                      <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 6.75c0 8.284 6.716 15 15 15h2.25a2.25 2.25 0 002.25-2.25v-1.372c0-.516-.351-.966-.852-1.091l-4.423-1.106c-.44-.11-.902.055-1.173.417l-.97 1.293c-.282.376-.769.542-1.21.38a12.035 12.035 0 01-7.143-7.143c-.162-.441.004-.928.38-1.21l1.293-.97c.363-.271.527-.734.417-1.173L6.963 3.102a1.125 1.125 0 00-1.091-.852H4.5A2.25 2.25 0 002.25 4.5v2.25z" />
                      </svg>
                    )}
                    {eventType.locationType === 'InPerson' && (
                      <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z" />
                        <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z" />
                      </svg>
                    )}
                    {eventType.locationType === 'Custom' && (
                      <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M9.879 7.519c1.171-1.025 3.071-1.025 4.242 0 1.172 1.025 1.172 2.687 0 3.712-.203.179-.43.326-.67.442-.745.361-1.45.999-1.45 1.827v.75M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9 5.25h.008v.008H12v-.008z" />
                      </svg>
                    )}
                    <span>
                      {eventType.locationDisplayName || eventType.locationType}
                      {eventType.locationDetails && (
                        <span className="ml-1 text-gray-400">({eventType.locationDetails})</span>
                      )}
                    </span>
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
                <h2 className="mb-4 text-sm font-medium text-gray-900">Select a Date</h2>
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
                <h2 className="mb-4 text-sm font-medium text-gray-900">
                  {selectedDate
                    ? `Available times for ${format(parseISO(selectedDate), 'MMM d')}`
                    : 'Select a date to see times'}
                </h2>
                {!selectedDate ? (
                  <p className="text-sm text-gray-500">
                    Please select a date first.
                  </p>
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
                  <p className="text-sm text-gray-500">
                    No available times for this date.
                  </p>
                )}

                {selectedSlot && (
                  <div className="mt-6">
                    <Button
                      className="w-full"
                      onClick={() => setShowBookingForm(true)}
                    >
                      Continue
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
