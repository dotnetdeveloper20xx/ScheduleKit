import { useState, useEffect } from 'react';
import { Button, Card, CardContent, CardHeader } from '@/components/ui';
import {
  useWeeklyAvailability,
  useUpdateWeeklyAvailability,
} from '@/api/hooks/useAvailability';
import type { DayAvailabilityUpdate } from '@/api/types';
import { cn } from '@/lib/utils';

const dayNames = [
  'Sunday',
  'Monday',
  'Tuesday',
  'Wednesday',
  'Thursday',
  'Friday',
  'Saturday',
];

const timeOptions = [
  '00:00', '00:30', '01:00', '01:30', '02:00', '02:30', '03:00', '03:30',
  '04:00', '04:30', '05:00', '05:30', '06:00', '06:30', '07:00', '07:30',
  '08:00', '08:30', '09:00', '09:30', '10:00', '10:30', '11:00', '11:30',
  '12:00', '12:30', '13:00', '13:30', '14:00', '14:30', '15:00', '15:30',
  '16:00', '16:30', '17:00', '17:30', '18:00', '18:30', '19:00', '19:30',
  '20:00', '20:30', '21:00', '21:30', '22:00', '22:30', '23:00', '23:30',
];

interface DaySchedule {
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  isEnabled: boolean;
}

export function WeeklySchedule() {
  const { data: availability, isLoading } = useWeeklyAvailability();
  const updateAvailability = useUpdateWeeklyAvailability();
  const [schedule, setSchedule] = useState<DaySchedule[]>([]);
  const [hasChanges, setHasChanges] = useState(false);

  useEffect(() => {
    if (availability?.days) {
      setSchedule(
        availability.days.map((day) => ({
          dayOfWeek: day.dayOfWeek,
          startTime: day.startTime,
          endTime: day.endTime,
          isEnabled: day.isEnabled,
        }))
      );
    }
  }, [availability]);

  const handleToggleDay = (dayOfWeek: number) => {
    setSchedule((prev) =>
      prev.map((day) =>
        day.dayOfWeek === dayOfWeek ? { ...day, isEnabled: !day.isEnabled } : day
      )
    );
    setHasChanges(true);
  };

  const handleTimeChange = (
    dayOfWeek: number,
    field: 'startTime' | 'endTime',
    value: string
  ) => {
    setSchedule((prev) =>
      prev.map((day) =>
        day.dayOfWeek === dayOfWeek ? { ...day, [field]: value } : day
      )
    );
    setHasChanges(true);
  };

  const handleSave = () => {
    const updates: DayAvailabilityUpdate[] = schedule.map((day) => ({
      dayOfWeek: day.dayOfWeek,
      startTime: day.startTime,
      endTime: day.endTime,
      isEnabled: day.isEnabled,
    }));

    updateAvailability.mutate(
      { days: updates },
      {
        onSuccess: () => {
          setHasChanges(false);
        },
      }
    );
  };

  const handleReset = () => {
    if (availability?.days) {
      setSchedule(
        availability.days.map((day) => ({
          dayOfWeek: day.dayOfWeek,
          startTime: day.startTime,
          endTime: day.endTime,
          isEnabled: day.isEnabled,
        }))
      );
      setHasChanges(false);
    }
  };

  if (isLoading) {
    return (
      <Card>
        <CardContent className="animate-pulse py-8">
          <div className="space-y-4">
            {[...Array(7)].map((_, i) => (
              <div key={i} className="flex items-center gap-4">
                <div className="h-5 w-24 rounded bg-gray-200" />
                <div className="h-10 w-32 rounded bg-gray-200" />
                <div className="h-10 w-32 rounded bg-gray-200" />
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <h3 className="text-lg font-medium text-gray-900">Weekly Hours</h3>
            <p className="text-sm text-gray-500">
              Set your regular weekly availability
            </p>
          </div>
          {hasChanges && (
            <div className="flex items-center gap-2">
              <Button variant="outline" size="sm" onClick={handleReset}>
                Reset
              </Button>
              <Button
                size="sm"
                onClick={handleSave}
                isLoading={updateAvailability.isPending}
              >
                Save Changes
              </Button>
            </div>
          )}
        </div>
      </CardHeader>
      <CardContent>
        <div className="space-y-4">
          {schedule
            .sort((a, b) => a.dayOfWeek - b.dayOfWeek)
            .map((day) => (
              <div
                key={day.dayOfWeek}
                className={cn(
                  'flex items-center gap-4 rounded-lg border p-4 transition-colors',
                  day.isEnabled
                    ? 'border-gray-200 bg-white'
                    : 'border-gray-100 bg-gray-50'
                )}
              >
                {/* Day toggle */}
                <button
                  type="button"
                  onClick={() => handleToggleDay(day.dayOfWeek)}
                  className={cn(
                    'flex h-6 w-11 items-center rounded-full p-1 transition-colors',
                    day.isEnabled ? 'bg-primary-600' : 'bg-gray-200'
                  )}
                >
                  <span
                    className={cn(
                      'h-4 w-4 rounded-full bg-white shadow transition-transform',
                      day.isEnabled ? 'translate-x-5' : 'translate-x-0'
                    )}
                  />
                </button>

                {/* Day name */}
                <span
                  className={cn(
                    'w-24 text-sm font-medium',
                    day.isEnabled ? 'text-gray-900' : 'text-gray-400'
                  )}
                >
                  {dayNames[day.dayOfWeek]}
                </span>

                {/* Time selects */}
                {day.isEnabled ? (
                  <>
                    <select
                      value={day.startTime}
                      onChange={(e) =>
                        handleTimeChange(day.dayOfWeek, 'startTime', e.target.value)
                      }
                      className="rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
                    >
                      {timeOptions.map((time) => (
                        <option key={time} value={time}>
                          {time}
                        </option>
                      ))}
                    </select>

                    <span className="text-gray-500">to</span>

                    <select
                      value={day.endTime}
                      onChange={(e) =>
                        handleTimeChange(day.dayOfWeek, 'endTime', e.target.value)
                      }
                      className="rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
                    >
                      {timeOptions.map((time) => (
                        <option key={time} value={time}>
                          {time}
                        </option>
                      ))}
                    </select>
                  </>
                ) : (
                  <span className="text-sm text-gray-400">Unavailable</span>
                )}
              </div>
            ))}
        </div>
      </CardContent>
    </Card>
  );
}
