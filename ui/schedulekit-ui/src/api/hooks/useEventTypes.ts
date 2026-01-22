import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient, getErrorMessage } from '../client';
import type {
  EventTypeResponse,
  CreateEventTypeRequest,
  UpdateEventTypeRequest,
} from '../types';

// Query keys
export const eventTypeKeys = {
  all: ['eventTypes'] as const,
  lists: () => [...eventTypeKeys.all, 'list'] as const,
  list: (filters: string) => [...eventTypeKeys.lists(), { filters }] as const,
  details: () => [...eventTypeKeys.all, 'detail'] as const,
  detail: (id: string) => [...eventTypeKeys.details(), id] as const,
};

// Get all event types
export function useEventTypes() {
  return useQuery({
    queryKey: eventTypeKeys.lists(),
    queryFn: async () => {
      const response = await apiClient.get<EventTypeResponse[]>('/EventTypes');
      return response.data;
    },
  });
}

// Get single event type by ID
export function useEventType(id: string | undefined) {
  return useQuery({
    queryKey: eventTypeKeys.detail(id ?? ''),
    queryFn: async () => {
      const response = await apiClient.get<EventTypeResponse>(
        `/EventTypes/${id}`
      );
      return response.data;
    },
    enabled: !!id,
  });
}

// Create event type
export function useCreateEventType() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: CreateEventTypeRequest) => {
      const response = await apiClient.post<EventTypeResponse>(
        '/EventTypes',
        data
      );
      return response.data;
    },
    onSuccess: () => {
      // Invalidate the list to refetch
      queryClient.invalidateQueries({ queryKey: eventTypeKeys.lists() });
    },
    onError: (error) => {
      console.error('Failed to create event type:', getErrorMessage(error));
    },
  });
}

// Update event type
export function useUpdateEventType() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      data,
    }: {
      id: string;
      data: UpdateEventTypeRequest;
    }) => {
      const response = await apiClient.put<EventTypeResponse>(
        `/EventTypes/${id}`,
        data
      );
      return response.data;
    },
    onSuccess: (data) => {
      // Update cache for the specific item
      queryClient.setQueryData(eventTypeKeys.detail(data.id), data);
      // Invalidate the list
      queryClient.invalidateQueries({ queryKey: eventTypeKeys.lists() });
    },
    onError: (error) => {
      console.error('Failed to update event type:', getErrorMessage(error));
    },
  });
}

// Delete event type
export function useDeleteEventType() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await apiClient.delete(`/EventTypes/${id}`);
      return id;
    },
    onSuccess: (deletedId) => {
      // Remove from cache
      queryClient.removeQueries({ queryKey: eventTypeKeys.detail(deletedId) });
      // Invalidate the list
      queryClient.invalidateQueries({ queryKey: eventTypeKeys.lists() });
    },
    onError: (error) => {
      console.error('Failed to delete event type:', getErrorMessage(error));
    },
  });
}

// Toggle event type active status
export function useToggleEventTypeStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, isActive }: { id: string; isActive: boolean }) => {
      const response = await apiClient.patch<EventTypeResponse>(
        `/EventTypes/${id}/status`,
        { isActive }
      );
      return response.data;
    },
    onSuccess: (data) => {
      queryClient.setQueryData(eventTypeKeys.detail(data.id), data);
      queryClient.invalidateQueries({ queryKey: eventTypeKeys.lists() });
    },
    onError: (error) => {
      console.error(
        'Failed to toggle event type status:',
        getErrorMessage(error)
      );
    },
  });
}
