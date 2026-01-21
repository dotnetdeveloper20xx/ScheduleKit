import { useNavigate } from 'react-router-dom';
import { PageContainer, PageHeader } from '@/components/layout/PageContainer';
import { Card, CardContent } from '@/components/ui';
import { useCreateEventType } from '@/api/hooks/useEventTypes';
import { EventTypeForm } from './EventTypeForm';
import type { CreateEventTypeRequest } from '@/api/types';

export function EventTypeCreatePage() {
  const navigate = useNavigate();
  const createEventType = useCreateEventType();

  const handleSubmit = (data: CreateEventTypeRequest) => {
    createEventType.mutate(data, {
      onSuccess: () => {
        navigate('/event-types');
      },
    });
  };

  return (
    <PageContainer className="max-w-2xl">
      <PageHeader
        title="Create Event Type"
        description="Set up a new event type for people to book with you."
      />

      <Card>
        <CardContent className="py-6">
          <EventTypeForm
            onSubmit={handleSubmit}
            isLoading={createEventType.isPending}
            submitLabel="Create Event Type"
          />
        </CardContent>
      </Card>
    </PageContainer>
  );
}
