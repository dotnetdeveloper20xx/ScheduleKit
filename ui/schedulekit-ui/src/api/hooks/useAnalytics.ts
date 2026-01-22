import { useQuery } from '@tanstack/react-query';
import { apiClient } from '../client';

export interface DashboardAnalyticsResponse {
  totalBookings: number;
  confirmedBookings: number;
  cancelledBookings: number;
  completedBookings: number;
  noShowBookings: number;
  completionRate: number;
  cancellationRate: number;
  upcomingBookings: number;
  todaysBookings: number;
  thisWeeksBookings: number;
  thisMonthsBookings: number;
  bookingsLast30Days: DailyBookingCount[];
  popularHours: HourlyBookingCount[];
  topEventTypes: EventTypeBookingCount[];
}

export interface DailyBookingCount {
  date: string;
  count: number;
}

export interface HourlyBookingCount {
  hour: number;
  count: number;
}

export interface EventTypeBookingCount {
  eventTypeName: string;
  count: number;
}

export const analyticsKeys = {
  all: ['analytics'] as const,
  dashboard: () => [...analyticsKeys.all, 'dashboard'] as const,
};

export function useDashboardAnalytics() {
  return useQuery({
    queryKey: analyticsKeys.dashboard(),
    queryFn: async () => {
      const response = await apiClient.get<DashboardAnalyticsResponse>(
        '/Analytics/dashboard'
      );
      return response.data;
    },
    staleTime: 5 * 60 * 1000, // 5 minutes
    refetchInterval: 5 * 60 * 1000, // Refetch every 5 minutes
  });
}
