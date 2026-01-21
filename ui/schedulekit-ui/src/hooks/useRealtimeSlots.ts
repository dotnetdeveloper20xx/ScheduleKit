import { useEffect, useRef, useCallback } from 'react';
import { useQueryClient } from '@tanstack/react-query';
import * as signalR from '@microsoft/signalr';

const HUB_URL = '/hubs/schedulekit';

interface UseRealtimeSlotsOptions {
  eventTypeId: string;
  enabled?: boolean;
}

export function useRealtimeSlots({ eventTypeId, enabled = true }: UseRealtimeSlotsOptions) {
  const queryClient = useQueryClient();
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  const handleSlotBooked = useCallback(
    (bookedEventTypeId: string, startTimeUtc: string) => {
      if (bookedEventTypeId === eventTypeId) {
        // Invalidate slots query to refetch
        queryClient.invalidateQueries({ queryKey: ['availableSlots', eventTypeId] });
        queryClient.invalidateQueries({ queryKey: ['availableDates', eventTypeId] });
      }
    },
    [eventTypeId, queryClient]
  );

  const handleSlotReleased = useCallback(
    (releasedEventTypeId: string, startTimeUtc: string) => {
      if (releasedEventTypeId === eventTypeId) {
        // Invalidate slots query to refetch
        queryClient.invalidateQueries({ queryKey: ['availableSlots', eventTypeId] });
        queryClient.invalidateQueries({ queryKey: ['availableDates', eventTypeId] });
      }
    },
    [eventTypeId, queryClient]
  );

  useEffect(() => {
    if (!enabled || !eventTypeId) {
      return;
    }

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    connectionRef.current = connection;

    // Register event handlers
    connection.on('SlotBooked', handleSlotBooked);
    connection.on('SlotReleased', handleSlotReleased);

    // Start connection and join event type group
    connection
      .start()
      .then(() => {
        console.log('SignalR connected');
        return connection.invoke('JoinEventTypeGroup', eventTypeId);
      })
      .then(() => {
        console.log(`Joined event type group: ${eventTypeId}`);
      })
      .catch((err) => {
        console.error('SignalR connection error:', err);
      });

    // Cleanup on unmount
    return () => {
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection
          .invoke('LeaveEventTypeGroup', eventTypeId)
          .catch(() => {})
          .finally(() => {
            connection.stop();
          });
      } else {
        connection.stop();
      }
    };
  }, [eventTypeId, enabled, handleSlotBooked, handleSlotReleased]);

  return {
    isConnected: connectionRef.current?.state === signalR.HubConnectionState.Connected,
  };
}
