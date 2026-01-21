import { useQuery, useMutation } from '@tanstack/react-query';
import { apiClient, getErrorMessage } from '../client';
import type {
  RescheduleBookingInfoResponse,
  PublicRescheduleRequest,
  BookingResponse,
} from '../types';

// Get reschedule info by token
export function useRescheduleInfo(token: string | undefined) {
  return useQuery({
    queryKey: ['reschedule', token],
    queryFn: async () => {
      const response = await apiClient.get<RescheduleBookingInfoResponse>(
        `/public/reschedule/${token}`
      );
      return response.data;
    },
    enabled: !!token,
    retry: false,
  });
}

// Perform the reschedule
export function useRescheduleBooking() {
  return useMutation({
    mutationFn: async ({
      token,
      data,
    }: {
      token: string;
      data: PublicRescheduleRequest;
    }) => {
      const response = await apiClient.post<BookingResponse>(
        `/public/reschedule/${token}`,
        data
      );
      return response.data;
    },
    onError: (error) => {
      console.error('Failed to reschedule booking:', getErrorMessage(error));
    },
  });
}
