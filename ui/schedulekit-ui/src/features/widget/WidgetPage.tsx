import { useState, useEffect } from 'react';
import { useParams, useSearchParams } from 'react-router-dom';
import { format, parseISO } from 'date-fns';
import { usePublicEventType, useAvailableSlots, useAvailableDates } from '@/api/hooks/useSlots';
import { useCreatePublicBooking } from '@/api/hooks/useBookings';
import { Button, Input } from '@/components/ui';
import { cn } from '@/lib/utils';

type Step = 'date' | 'time' | 'details' | 'confirmation';

export function WidgetPage() {
  const { hostSlug, eventSlug } = useParams<{ hostSlug: string; eventSlug: string }>();
  const [searchParams] = useSearchParams();
  const primaryColor = searchParams.get('primaryColor') || '#3b82f6';

  const [step, setStep] = useState<Step>('date');
  const [selectedDate, setSelectedDate] = useState<string | null>(null);
  const [selectedSlot, setSelectedSlot] = useState<string | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    notes: '',
  });
  const [bookingResult, setBookingResult] = useState<{
    id: string;
    startTime: string;
  } | null>(null);

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

  const createBooking = useCreatePublicBooking();

  // Notify parent about height changes for responsive iframe
  useEffect(() => {
    const sendHeight = () => {
      const height = document.body.scrollHeight;
      window.parent.postMessage({ type: 'schedulekit-resize', height }, '*');
    };

    sendHeight();
    const observer = new MutationObserver(sendHeight);
    observer.observe(document.body, { subtree: true, childList: true, characterData: true });

    return () => observer.disconnect();
  }, [step, selectedDate, selectedSlot]);

  // Notify parent about booking completion
  const notifyBookingComplete = (booking: { id: string; startTime: string }) => {
    window.parent.postMessage({
      type: 'schedulekit-booking-complete',
      booking: {
        id: booking.id,
        eventType: eventType?.name,
        startTime: booking.startTime,
        guestEmail: formData.email,
      },
    }, '*');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!eventType || !selectedDate || !selectedSlot) return;

    try {
      const result = await createBooking.mutateAsync({
        eventTypeId: eventType.id,
        guestName: formData.name,
        guestEmail: formData.email,
        startTimeUtc: selectedSlot,
        guestTimezone: timezone,
        notes: formData.notes || undefined,
        questionResponses: [],
      });

      setBookingResult({
        id: result.id,
        startTime: selectedSlot,
      });
      notifyBookingComplete({ id: result.id, startTime: selectedSlot });
      setStep('confirmation');
    } catch {
      // Error handled by mutation
    }
  };

  if (eventTypeLoading) {
    return (
      <div className="flex min-h-[200px] items-center justify-center p-4">
        <div className="h-6 w-6 animate-spin rounded-full border-2 border-t-transparent" style={{ borderColor: primaryColor, borderTopColor: 'transparent' }} />
      </div>
    );
  }

  if (eventTypeError || !eventType) {
    return (
      <div className="p-4 text-center">
        <p className="text-sm text-gray-500">Event type not found</p>
      </div>
    );
  }

  // Confirmation step
  if (step === 'confirmation' && bookingResult) {
    return (
      <div className="p-6 text-center">
        <div className="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-green-100">
          <svg className="h-6 w-6 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
          </svg>
        </div>
        <h2 className="text-lg font-semibold text-gray-900">Booking Confirmed!</h2>
        <p className="mt-2 text-sm text-gray-600">
          {eventType.name} on {format(parseISO(bookingResult.startTime), 'EEEE, MMMM d, yyyy')} at {format(parseISO(bookingResult.startTime), 'h:mm a')}
        </p>
        <p className="mt-1 text-xs text-gray-500">
          A confirmation email has been sent to {formData.email}
        </p>
      </div>
    );
  }

  // Details form step
  if (step === 'details' && selectedDate && selectedSlot) {
    return (
      <div className="p-4">
        <button
          onClick={() => setStep('time')}
          className="mb-4 flex items-center gap-1 text-sm text-gray-600 hover:text-gray-900"
        >
          <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          Back
        </button>

        <div className="mb-4 rounded-lg bg-gray-50 p-3">
          <p className="text-sm font-medium text-gray-900">{eventType.name}</p>
          <p className="text-sm text-gray-600">
            {format(parseISO(selectedSlot), 'EEEE, MMMM d, yyyy')} at {format(parseISO(selectedSlot), 'h:mm a')}
          </p>
          <p className="text-xs text-gray-500">{timezone}</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">Name</label>
            <Input
              value={formData.name}
              onChange={(e) => setFormData({ ...formData, name: e.target.value })}
              required
              placeholder="Your name"
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">Email</label>
            <Input
              type="email"
              value={formData.email}
              onChange={(e) => setFormData({ ...formData, email: e.target.value })}
              required
              placeholder="you@example.com"
            />
          </div>
          <div>
            <label className="mb-1 block text-sm font-medium text-gray-700">Notes (optional)</label>
            <textarea
              value={formData.notes}
              onChange={(e) => setFormData({ ...formData, notes: e.target.value })}
              className="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
              rows={3}
              placeholder="Anything you'd like to share..."
            />
          </div>
          <Button
            type="submit"
            className="w-full"
            disabled={createBooking.isPending}
            style={{ backgroundColor: primaryColor }}
          >
            {createBooking.isPending ? 'Booking...' : 'Confirm Booking'}
          </Button>
          {createBooking.isError && (
            <p className="text-sm text-red-600">
              Failed to create booking. Please try again.
            </p>
          )}
        </form>
      </div>
    );
  }

  // Time selection step
  if (step === 'time' && selectedDate) {
    return (
      <div className="p-4">
        <button
          onClick={() => {
            setStep('date');
            setSelectedSlot(null);
          }}
          className="mb-4 flex items-center gap-1 text-sm text-gray-600 hover:text-gray-900"
        >
          <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          Back
        </button>

        <h3 className="mb-2 text-sm font-medium text-gray-900">
          {format(parseISO(selectedDate), 'EEEE, MMMM d')}
        </h3>
        <p className="mb-4 text-xs text-gray-500">{eventType.name} - {eventType.durationMinutes} min</p>

        {slotsLoading ? (
          <div className="space-y-2">
            {[...Array(4)].map((_, i) => (
              <div key={i} className="h-10 animate-pulse rounded bg-gray-100" />
            ))}
          </div>
        ) : slotsData?.slots && slotsData.slots.length > 0 ? (
          <div className="grid grid-cols-2 gap-2">
            {slotsData.slots.map((slot) => (
              <button
                key={slot.startTime}
                onClick={() => {
                  setSelectedSlot(slot.startTime);
                  setStep('details');
                }}
                className="rounded-lg border border-gray-200 px-3 py-2 text-sm font-medium transition-colors hover:border-gray-300 hover:bg-gray-50"
                style={{
                  borderColor: selectedSlot === slot.startTime ? primaryColor : undefined,
                  backgroundColor: selectedSlot === slot.startTime ? `${primaryColor}10` : undefined,
                }}
              >
                {slot.startTime.includes('T') ? format(parseISO(slot.startTime), 'h:mm a') : slot.startTime}
              </button>
            ))}
          </div>
        ) : (
          <p className="text-sm text-gray-500">No available times</p>
        )}
      </div>
    );
  }

  // Date selection step (default)
  return (
    <div className="p-4">
      <div className="mb-4 border-b pb-4">
        <h2 className="text-lg font-semibold text-gray-900">{eventType.name}</h2>
        <p className="text-sm text-gray-500">{eventType.durationMinutes} min</p>
      </div>

      <h3 className="mb-3 text-sm font-medium text-gray-900">Select a date</h3>

      {datesLoading ? (
        <div className="space-y-2">
          {[...Array(5)].map((_, i) => (
            <div key={i} className="h-12 animate-pulse rounded bg-gray-100" />
          ))}
        </div>
      ) : availableDates?.dates && availableDates.dates.filter(d => d.hasAvailability).length > 0 ? (
        <div className="max-h-[300px] space-y-2 overflow-y-auto">
          {availableDates.dates
            .filter((d) => d.hasAvailability)
            .map((dateInfo) => (
              <button
                key={dateInfo.date}
                onClick={() => {
                  setSelectedDate(dateInfo.date);
                  setStep('time');
                }}
                className={cn(
                  'w-full rounded-lg border px-4 py-3 text-left text-sm transition-colors',
                  'border-gray-200 hover:border-gray-300 hover:bg-gray-50'
                )}
              >
                <span className="font-medium">{format(parseISO(dateInfo.date), 'EEEE')}</span>
                <br />
                <span className="text-gray-500">{format(parseISO(dateInfo.date), 'MMMM d, yyyy')}</span>
              </button>
            ))}
        </div>
      ) : (
        <p className="text-sm text-gray-500">No available dates</p>
      )}

      <p className="mt-4 text-center text-xs text-gray-400">
        Timezone: {timezone}
      </p>
    </div>
  );
}
