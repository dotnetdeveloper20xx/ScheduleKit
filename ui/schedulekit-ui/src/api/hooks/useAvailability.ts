import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient, getErrorMessage } from '../client';
import type {
  WeeklyAvailabilityResponse,
  UpdateWeeklyAvailabilityRequest,
  AvailabilityOverrideResponse,
  CreateAvailabilityOverrideRequest,
} from '../types';

// Query keys
export const availabilityKeys = {
  all: ['availability'] as const,
  weekly: () => [...availabilityKeys.all, 'weekly'] as const,
  overrides: () => [...availabilityKeys.all, 'overrides'] as const,
  overridesList: (fromDate?: string, toDate?: string) =>
    [...availabilityKeys.overrides(), { fromDate, toDate }] as const,
};

// Get weekly availability
export function useWeeklyAvailability() {
  return useQuery({
    queryKey: availabilityKeys.weekly(),
    queryFn: async () => {
      const response =
        await apiClient.get<WeeklyAvailabilityResponse>('/Availability');
      return response.data;
    },
  });
}

// Update weekly availability
export function useUpdateWeeklyAvailability() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: UpdateWeeklyAvailabilityRequest) => {
      const response = await apiClient.put<WeeklyAvailabilityResponse>(
        '/Availability',
        data
      );
      return response.data;
    },
    onSuccess: (data) => {
      queryClient.setQueryData(availabilityKeys.weekly(), data);
    },
    onError: (error) => {
      console.error(
        'Failed to update availability:',
        getErrorMessage(error)
      );
    },
  });
}

// Get availability overrides
export function useAvailabilityOverrides(fromDate?: string, toDate?: string) {
  return useQuery({
    queryKey: availabilityKeys.overridesList(fromDate, toDate),
    queryFn: async () => {
      const params = new URLSearchParams();
      if (fromDate) params.append('fromDate', fromDate);
      if (toDate) params.append('toDate', toDate);

      const response = await apiClient.get<AvailabilityOverrideResponse[]>(
        `/Availability/overrides?${params.toString()}`
      );
      return response.data;
    },
  });
}

// Create availability override
export function useCreateAvailabilityOverride() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (data: CreateAvailabilityOverrideRequest) => {
      const response = await apiClient.post<AvailabilityOverrideResponse>(
        '/Availability/overrides',
        data
      );
      return response.data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: availabilityKeys.overrides() });
    },
    onError: (error) => {
      console.error('Failed to create override:', getErrorMessage(error));
    },
  });
}

// Delete availability override
export function useDeleteAvailabilityOverride() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await apiClient.delete(`/Availability/overrides/${id}`);
      return id;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: availabilityKeys.overrides() });
    },
    onError: (error) => {
      console.error('Failed to delete override:', getErrorMessage(error));
    },
  });
}
