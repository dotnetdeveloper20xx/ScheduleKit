import { useNavigate, useParams } from 'react-router-dom';
import { PageContainer, PageHeader } from '@/components/layout/PageContainer';
import { Card, CardContent } from '@/components/ui';
import { useEventType, useUpdateEventType } from '@/api/hooks/useEventTypes';
import { EventTypeForm } from './EventTypeForm';
import type { CreateEventTypeRequest } from '@/api/types';

export function EventTypeEditPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { data: eventType, isLoading, error } = useEventType(id);
  const updateEventType = useUpdateEventType();

  const handleSubmit = (data: CreateEventTypeRequest) => {
    if (!id) return;

    updateEventType.mutate(
      { id, data },
      {
        onSuccess: () => {
          navigate('/event-types');
        },
      }
    );
  };

  if (isLoading) {
    return (
      <PageContainer className="max-w-2xl">
        <PageHeader title="Edit Event Type" />
        <Card>
          <CardContent className="animate-pulse py-6">
            <div className="mb-4 h-4 w-3/4 rounded bg-gray-200" />
            <div className="mb-2 h-4 w-1/2 rounded bg-gray-200" />
            <div className="h-32 w-full rounded bg-gray-200" />
          </CardContent>
        </Card>
      </PageContainer>
    );
  }

  if (error || !eventType) {
    return (
      <PageContainer className="max-w-2xl">
        <PageHeader title="Edit Event Type" />
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
            <p>Event type not found or failed to load.</p>
          </CardContent>
        </Card>
      </PageContainer>
    );
  }

  return (
    <PageContainer className="max-w-2xl">
      <PageHeader
        title="Edit Event Type"
        description={`Editing "${eventType.name}"`}
      />

      <Card>
        <CardContent className="py-6">
          <EventTypeForm
            defaultValues={eventType}
            onSubmit={handleSubmit}
            isLoading={updateEventType.isPending}
            submitLabel="Save Changes"
          />
        </CardContent>
      </Card>
    </PageContainer>
  );
}
