import { useQuery } from '@tanstack/react-query';
import { apiClient } from '../client';
import type {
  AvailableSlotsResponse,
  AvailableDatesResponse,
  EventTypeResponse,
} from '../types';

// Query keys
export const slotsKeys = {
  all: ['slots'] as const,
  forDate: (eventTypeId: string, date: string, timezone: string) =>
    [...slotsKeys.all, 'date', eventTypeId, date, timezone] as const,
  availableDates: (eventTypeId: string, timezone: string) =>
    [...slotsKeys.all, 'dates', eventTypeId, timezone] as const,
  publicEventType: (hostSlug: string, eventSlug: string) =>
    ['public', 'eventType', hostSlug, eventSlug] as const,
};

// Get available slots for a specific date
export function useAvailableSlots(
  eventTypeId: string | undefined,
  date: string | undefined,
  timezone: string = 'UTC'
) {
  return useQuery({
    queryKey: slotsKeys.forDate(eventTypeId ?? '', date ?? '', timezone),
    queryFn: async () => {
      const params = new URLSearchParams({
        date: date!,
        timezone,
      });

      const response = await apiClient.get<AvailableSlotsResponse>(
        `/public/slots/${eventTypeId}?${params.toString()}`
      );
      return response.data;
    },
    enabled: !!eventTypeId && !!date,
    staleTime: 1000 * 60, // 1 minute
  });
}

// Get available dates within booking window
export function useAvailableDates(
  eventTypeId: string | undefined,
  timezone: string = 'UTC'
) {
  return useQuery({
    queryKey: slotsKeys.availableDates(eventTypeId ?? '', timezone),
    queryFn: async () => {
      const params = new URLSearchParams({ timezone });

      const response = await apiClient.get<AvailableDatesResponse>(
        `/public/dates/${eventTypeId}?${params.toString()}`
      );
      return response.data;
    },
    enabled: !!eventTypeId,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
}

// Get public event type by slug
export function usePublicEventType(hostSlug: string, eventSlug: string) {
  return useQuery({
    queryKey: slotsKeys.publicEventType(hostSlug, eventSlug),
    queryFn: async () => {
      const response = await apiClient.get<EventTypeResponse>(
        `/public/${hostSlug}/${eventSlug}`
      );
      return response.data;
    },
    enabled: !!hostSlug && !!eventSlug,
  });
}
