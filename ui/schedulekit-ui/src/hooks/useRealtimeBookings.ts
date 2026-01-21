import { useEffect, useRef, useCallback } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import * as signalR from '@microsoft/signalr';
import { useAuth } from '@/contexts/AuthContext';

const HUB_URL = '/hubs/schedulekit';

interface BookingNotification {
  id: string;
  eventTypeId: string;
  eventTypeName: string;
  guestName: string;
  guestEmail: string;
  startTimeUtc: string;
  endTimeUtc: string;
  guestTimezone: string;
  createdAtUtc: string;
}

interface UseRealtimeBookingsOptions {
  onBookingCreated?: (booking: BookingNotification) => void;
  onBookingCancelled?: (bookingId: string, reason?: string) => void;
  onBookingRescheduled?: (bookingId: string, oldTime: string, newTime: string) => void;
  enabled?: boolean;
}

export function useRealtimeBookings(options: UseRealtimeBookingsOptions = {}) {
  const { onBookingCreated, onBookingCancelled, onBookingRescheduled, enabled = true } = options;
  const { isAuthenticated } = useAuth();
  const queryClient = useQueryClient();
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  const handleBookingCreated = useCallback(
    (booking: BookingNotification) => {
      // Invalidate bookings list to show new booking
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });

      // Call custom handler if provided
      onBookingCreated?.(booking);
    },
    [queryClient, onBookingCreated]
  );

  const handleBookingCancelled = useCallback(
    (bookingId: string, reason?: string) => {
      // Invalidate bookings list
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
      queryClient.invalidateQueries({ queryKey: ['booking', bookingId] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });

      // Call custom handler if provided
      onBookingCancelled?.(bookingId, reason);
    },
    [queryClient, onBookingCancelled]
  );

  const handleBookingRescheduled = useCallback(
    (bookingId: string, oldTime: string, newTime: string) => {
      // Invalidate bookings list
      queryClient.invalidateQueries({ queryKey: ['bookings'] });
      queryClient.invalidateQueries({ queryKey: ['booking', bookingId] });
      queryClient.invalidateQueries({ queryKey: ['dashboard'] });

      // Call custom handler if provided
      onBookingRescheduled?.(bookingId, oldTime, newTime);
    },
    [queryClient, onBookingRescheduled]
  );

  useEffect(() => {
    if (!enabled || !isAuthenticated) {
      return;
    }

    // Get token for authentication
    const token = localStorage.getItem('schedulekit_token');
    if (!token) {
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${HUB_URL}?access_token=${token}`)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connectionRef.current = connection;

    // Register event handlers
    connection.on('BookingCreated', handleBookingCreated);
    connection.on('BookingCancelled', handleBookingCancelled);
    connection.on('BookingRescheduled', handleBookingRescheduled);

    // Start connection and join host dashboard
    connection
      .start()
      .then(() => {
        console.log('SignalR connected (host dashboard)');
        return connection.invoke('JoinHostDashboard');
      })
      .then(() => {
        console.log('Joined host dashboard');
      })
      .catch((err) => {
        console.error('SignalR connection error:', err);
      });

    // Cleanup on unmount
    return () => {
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection
          .invoke('LeaveHostDashboard')
          .catch(() => {})
          .finally(() => {
            connection.stop();
          });
      } else {
        connection.stop();
      }
    };
  }, [enabled, isAuthenticated, handleBookingCreated, handleBookingCancelled, handleBookingRescheduled]);

  return {
    isConnected: connectionRef.current?.state === signalR.HubConnectionState.Connected,
  };
}
