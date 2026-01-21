import { Link } from 'react-router-dom';
import { PageContainer, PageHeader } from '@/components/layout/PageContainer';
import { Button, Card, CardContent } from '@/components/ui';
import { useEventTypes, useDeleteEventType } from '@/api/hooks/useEventTypes';
import { EventTypeCard } from './EventTypeCard';

export function EventTypeListPage() {
  const { data: eventTypes, isLoading, error } = useEventTypes();
  const deleteEventType = useDeleteEventType();

  const handleDelete = (id: string) => {
    if (window.confirm('Are you sure you want to delete this event type?')) {
      deleteEventType.mutate(id);
    }
  };

  return (
    <PageContainer>
      <PageHeader
        title="Event Types"
        description="Create and manage your event types for scheduling."
        actions={
          <Link to="/event-types/new">
            <Button>
              <svg
                className="mr-2 h-5 w-5"
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
              New Event Type
            </Button>
          </Link>
        }
      />

      {/* Loading State */}
      {isLoading && (
        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          {[1, 2, 3].map((i) => (
            <Card key={i} className="animate-pulse">
              <CardContent>
                <div className="mb-4 h-4 w-3/4 rounded bg-gray-200" />
                <div className="mb-2 h-3 w-1/2 rounded bg-gray-200" />
                <div className="h-3 w-full rounded bg-gray-200" />
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* Error State */}
      {error && (
        <Card className="border-red-200 bg-red-50">
          <CardContent className="flex items-center gap-3 text-red-700">
            <svg
              className="h-5 w-5 flex-shrink-0"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={1.5}
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M12 9v3.75m9-.75a9 9 0 11-18 0 9 9 0 0118 0zm-9 3.75h.008v.008H12v-.008z"
              />
            </svg>
            <p>Failed to load event types. Please try again.</p>
          </CardContent>
        </Card>
      )}

      {/* Empty State */}
      {!isLoading && !error && eventTypes?.length === 0 && (
        <Card>
          <CardContent className="py-12 text-center">
            <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-gray-100">
              <svg
                className="h-8 w-8 text-gray-400"
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
              No event types yet
            </h3>
            <p className="mb-6 text-gray-500">
              Create your first event type to start accepting bookings.
            </p>
            <Link to="/event-types/new">
              <Button>Create Event Type</Button>
            </Link>
          </CardContent>
        </Card>
      )}

      {/* Event Types Grid */}
      {!isLoading && !error && eventTypes && eventTypes.length > 0 && (
        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          {eventTypes.map((eventType) => (
            <EventTypeCard
              key={eventType.id}
              eventType={eventType}
              onDelete={handleDelete}
            />
          ))}
        </div>
      )}
    </PageContainer>
  );
}
