import { useState } from 'react';
import { Link } from 'react-router-dom';
import { Card, CardContent, Button } from '@/components/ui';
import type { EventTypeResponse } from '@/api/types';
import { cn } from '@/lib/utils';
import { EmbedCodeModal } from '@/features/widget/EmbedCodeModal';

interface EventTypeCardProps {
  eventType: EventTypeResponse;
  hostSlug?: string;
  onDelete: (id: string) => void;
}

const locationLabels: Record<string, string> = {
  Zoom: 'Zoom Meeting',
  GoogleMeet: 'Google Meet',
  MicrosoftTeams: 'Microsoft Teams',
  Phone: 'Phone Call',
  InPerson: 'In Person',
  Custom: 'Custom Location',
};

const locationIcons: Record<string, React.ReactNode> = {
  Zoom: (
    <svg className="h-4 w-4" viewBox="0 0 24 24" fill="currentColor">
      <path d="M4.5 4.5h10.8c1.6 0 2.7 1.1 2.7 2.7v9.6c0 1.6-1.1 2.7-2.7 2.7H4.5c-1.6 0-2.7-1.1-2.7-2.7V7.2c0-1.6 1.1-2.7 2.7-2.7zm15.3 3.4l4.1-2.5v13.2l-4.1-2.5V7.9z" />
    </svg>
  ),
  GoogleMeet: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 10l4.553-2.276A1 1 0 0121 8.618v6.764a1 1 0 01-1.447.894L15 14M5 18h8a2 2 0 002-2V8a2 2 0 00-2-2H5a2 2 0 00-2 2v8a2 2 0 002 2z" />
    </svg>
  ),
  MicrosoftTeams: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
    </svg>
  ),
  Phone: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
    </svg>
  ),
  InPerson: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
    </svg>
  ),
  Custom: (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1" />
    </svg>
  ),
};

export function EventTypeCard({ eventType, hostSlug, onDelete }: EventTypeCardProps) {
  const [showEmbedModal, setShowEmbedModal] = useState(false);

  const colorStyles = eventType.color
    ? { borderLeftColor: eventType.color }
    : { borderLeftColor: '#3b82f6' };

  return (
    <Card
      className={cn(
        'relative overflow-hidden border-l-4 transition-shadow hover:shadow-md',
        !eventType.isActive && 'opacity-60'
      )}
      style={colorStyles}
    >
      <CardContent>
        {/* Status Badge */}
        {!eventType.isActive && (
          <span className="absolute right-4 top-4 rounded-full bg-gray-100 px-2 py-0.5 text-xs font-medium text-gray-600">
            Inactive
          </span>
        )}

        {/* Title & Duration */}
        <div className="mb-3">
          <h3 className="text-lg font-semibold text-gray-900">
            {eventType.name}
          </h3>
          <div className="flex items-center gap-2 text-sm text-gray-500">
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
            <span>{eventType.durationMinutes} minutes</span>
          </div>
        </div>

        {/* Description */}
        {eventType.description && (
          <p className="mb-4 line-clamp-2 text-sm text-gray-600">
            {eventType.description}
          </p>
        )}

        {/* Location */}
        <div className="mb-4 flex items-center gap-2 text-sm text-gray-500">
          {locationIcons[eventType.locationType] || locationIcons.Custom}
          <span>
            {eventType.locationDisplayName ||
              locationLabels[eventType.locationType] ||
              eventType.locationType}
          </span>
        </div>

        {/* Buffers */}
        {(eventType.bufferBeforeMinutes > 0 ||
          eventType.bufferAfterMinutes > 0) && (
          <div className="mb-4 flex flex-wrap gap-2 text-xs">
            {eventType.bufferBeforeMinutes > 0 && (
              <span className="rounded-full bg-gray-100 px-2 py-1 text-gray-600">
                {eventType.bufferBeforeMinutes}min before
              </span>
            )}
            {eventType.bufferAfterMinutes > 0 && (
              <span className="rounded-full bg-gray-100 px-2 py-1 text-gray-600">
                {eventType.bufferAfterMinutes}min after
              </span>
            )}
          </div>
        )}

        {/* Actions */}
        <div className="flex items-center justify-between border-t border-gray-100 pt-4">
          <div className="flex items-center gap-2">
            <Link to={`/event-types/${eventType.id}/edit`}>
              <Button variant="outline" size="sm">
                Edit
              </Button>
            </Link>
            <button
              onClick={() => {
                const bookingUrl = hostSlug
                  ? `${window.location.origin}/book/${hostSlug}/${eventType.slug}`
                  : `${window.location.origin}/book/${eventType.slug}`;
                navigator.clipboard.writeText(bookingUrl);
              }}
              className="rounded-lg p-2 text-gray-500 hover:bg-gray-100 hover:text-gray-700"
              title="Copy booking link"
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
                  d="M13.19 8.688a4.5 4.5 0 011.242 7.244l-4.5 4.5a4.5 4.5 0 01-6.364-6.364l1.757-1.757m13.35-.622l1.757-1.757a4.5 4.5 0 00-6.364-6.364l-4.5 4.5a4.5 4.5 0 001.242 7.244"
                />
              </svg>
            </button>
            {hostSlug && (
              <button
                onClick={() => setShowEmbedModal(true)}
                className="rounded-lg p-2 text-gray-500 hover:bg-gray-100 hover:text-gray-700"
                title="Get embed code"
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
                    d="M17.25 6.75L22.5 12l-5.25 5.25m-10.5 0L1.5 12l5.25-5.25m7.5-3l-4.5 16.5"
                  />
                </svg>
              </button>
            )}
          </div>
          <button
            onClick={() => onDelete(eventType.id)}
            className="rounded-lg p-2 text-gray-400 hover:bg-red-50 hover:text-red-600"
            title="Delete event type"
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
                d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0"
              />
            </svg>
          </button>
        </div>
      </CardContent>

      {/* Embed Code Modal */}
      {hostSlug && (
        <EmbedCodeModal
          isOpen={showEmbedModal}
          onClose={() => setShowEmbedModal(false)}
          hostSlug={hostSlug}
          eventSlug={eventType.slug}
        />
      )}
    </Card>
  );
}
