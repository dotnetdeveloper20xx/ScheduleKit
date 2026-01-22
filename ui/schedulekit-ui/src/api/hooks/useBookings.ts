import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiClient, getErrorMessage } from '../client';
import type {
  BookingResponse,
  CreatePublicBookingRequest,
  BookingConfirmationResponse,
  PaginatedResponse,
} from '../types';

// Query keys
export const bookingsKeys = {
  all: ['bookings'] as const,
  list: (params?: { page?: number; pageSize?: number; status?: string }) =>
    [...bookingsKeys.all, 'list', params] as const,
  detail: (id: string) => [...bookingsKeys.all, 'detail', id] as const,
};

// Get bookings list (for host)
export function useBookings(params?: {
  page?: number;
  pageSize?: number;
  status?: string;
}) {
  return useQuery({
    queryKey: bookingsKeys.list(params),
    queryFn: async () => {
      const searchParams = new URLSearchParams();
      if (params?.page) searchParams.append('page', params.page.toString());
      if (params?.pageSize)
        searchParams.append('pageSize', params.pageSize.toString());
      if (params?.status) searchParams.append('status', params.status);

      const response = await apiClient.get<PaginatedResponse<BookingResponse>>(
        `/Bookings?${searchParams.toString()}`
      );
      return response.data;
    },
  });
}

// Get single booking
export function useBooking(id: string) {
  return useQuery({
    queryKey: bookingsKeys.detail(id),
    queryFn: async () => {
      const response = await apiClient.get<BookingResponse>(`/Bookings/${id}`);
      return response.data;
    },
    enabled: !!id,
  });
}

// Create public booking (guest-facing)
export function useCreatePublicBooking() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: CreatePublicBookingRequest) => {
      const response = await apiClient.post<BookingConfirmationResponse>(
        '/public/bookings',
        data
      );
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: bookingsKeys.all });
    },
    onError: (error) => {
      console.error('Failed to create booking:', getErrorMessage(error));
    },
  });
}

// Cancel booking
export function useCancelBooking() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      reason,
    }: {
      id: string;
      reason?: string;
    }) => {
      await apiClient.post(`/Bookings/${id}/cancel`, { reason });
      return id;
    },
    onSuccess: (id) => {
      queryClient.invalidateQueries({ queryKey: bookingsKeys.all });
      queryClient.invalidateQueries({ queryKey: bookingsKeys.detail(id) });
    },
    onError: (error) => {
      console.error('Failed to cancel booking:', getErrorMessage(error));
    },
  });
}

// Reschedule booking
export function useRescheduleBooking() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      newStartTimeUtc,
    }: {
      id: string;
      newStartTimeUtc: string;
    }) => {
      const response = await apiClient.post<BookingResponse>(
        `/Bookings/${id}/reschedule`,
        { newStartTimeUtc }
      );
      return response.data;
    },
    onSuccess: (data) => {
      queryClient.invalidateQueries({ queryKey: bookingsKeys.all });
      queryClient.invalidateQueries({ queryKey: bookingsKeys.detail(data.id) });
    },
    onError: (error) => {
      console.error('Failed to reschedule booking:', getErrorMessage(error));
    },
  });
}
