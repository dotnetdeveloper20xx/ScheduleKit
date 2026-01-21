import { useState } from 'react';
import { format, addMonths, parseISO } from 'date-fns';
import { Button, Card, CardContent, CardHeader, Input } from '@/components/ui';
import {
  useAvailabilityOverrides,
  useCreateAvailabilityOverride,
  useDeleteAvailabilityOverride,
} from '@/api/hooks/useAvailability';
import type { CreateAvailabilityOverrideRequest } from '@/api/types';
import { cn } from '@/lib/utils';

export function DateOverrides() {
  const today = format(new Date(), 'yyyy-MM-dd');
  const threeMonthsFromNow = format(addMonths(new Date(), 3), 'yyyy-MM-dd');

  const { data: overrides, isLoading } = useAvailabilityOverrides(
    today,
    threeMonthsFromNow
  );
  const createOverride = useCreateAvailabilityOverride();
  const deleteOverride = useDeleteAvailabilityOverride();

  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState<CreateAvailabilityOverrideRequest>({
    date: '',
    startTime: '',
    endTime: '',
    isBlocked: true,
    reason: '',
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    const data: CreateAvailabilityOverrideRequest = {
      date: formData.date,
      isBlocked: formData.isBlocked,
      reason: formData.reason || undefined,
    };

    // Only include times if not blocking full day
    if (formData.startTime && formData.endTime) {
      data.startTime = formData.startTime;
      data.endTime = formData.endTime;
    }

    createOverride.mutate(data, {
      onSuccess: () => {
        setShowForm(false);
        setFormData({
          date: '',
          startTime: '',
          endTime: '',
          isBlocked: true,
          reason: '',
        });
      },
    });
  };

  const handleDelete = (id: string) => {
    if (window.confirm('Are you sure you want to delete this override?')) {
      deleteOverride.mutate(id);
    }
  };

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <h3 className="text-lg font-medium text-gray-900">Date Overrides</h3>
            <p className="text-sm text-gray-500">
              Block specific dates or add extra availability
            </p>
          </div>
          <Button onClick={() => setShowForm(!showForm)}>
            {showForm ? 'Cancel' : 'Add Override'}
          </Button>
        </div>
      </CardHeader>
      <CardContent>
        {/* Add Override Form */}
        {showForm && (
          <form onSubmit={handleSubmit} className="mb-6 rounded-lg border border-gray-200 bg-gray-50 p-4">
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
              <Input
                label="Date"
                type="date"
                required
                min={today}
                value={formData.date}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, date: e.target.value }))
                }
              />

              <div>
                <label className="mb-1.5 block text-sm font-medium text-gray-700">
                  Override Type
                </label>
                <select
                  value={formData.isBlocked ? 'block' : 'extra'}
                  onChange={(e) =>
                    setFormData((prev) => ({
                      ...prev,
                      isBlocked: e.target.value === 'block',
                    }))
                  }
                  className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
                >
                  <option value="block">Block Time</option>
                  <option value="extra">Add Extra Availability</option>
                </select>
              </div>

              <Input
                label="Start Time (optional for full day block)"
                type="time"
                value={formData.startTime}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, startTime: e.target.value }))
                }
              />

              <Input
                label="End Time"
                type="time"
                value={formData.endTime}
                onChange={(e) =>
                  setFormData((prev) => ({ ...prev, endTime: e.target.value }))
                }
              />

              <div className="sm:col-span-2">
                <Input
                  label="Reason (optional)"
                  placeholder="e.g., Vacation, Doctor's appointment"
                  value={formData.reason}
                  onChange={(e) =>
                    setFormData((prev) => ({ ...prev, reason: e.target.value }))
                  }
                />
              </div>
            </div>

            <div className="mt-4 flex justify-end gap-2">
              <Button
                type="button"
                variant="outline"
                onClick={() => setShowForm(false)}
              >
                Cancel
              </Button>
              <Button type="submit" isLoading={createOverride.isPending}>
                Add Override
              </Button>
            </div>
          </form>
        )}

        {/* Overrides List */}
        {isLoading ? (
          <div className="animate-pulse space-y-3">
            {[1, 2, 3].map((i) => (
              <div key={i} className="h-16 rounded-lg bg-gray-100" />
            ))}
          </div>
        ) : overrides && overrides.length > 0 ? (
          <div className="space-y-3">
            {overrides.map((override) => (
              <div
                key={override.id}
                className={cn(
                  'flex items-center justify-between rounded-lg border p-4',
                  override.isBlocked
                    ? 'border-red-200 bg-red-50'
                    : 'border-green-200 bg-green-50'
                )}
              >
                <div>
                  <div className="flex items-center gap-2">
                    <span
                      className={cn(
                        'inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium',
                        override.isBlocked
                          ? 'bg-red-100 text-red-700'
                          : 'bg-green-100 text-green-700'
                      )}
                    >
                      {override.isBlocked ? 'Blocked' : 'Extra'}
                    </span>
                    <span className="font-medium text-gray-900">
                      {format(parseISO(override.date), 'EEEE, MMMM d, yyyy')}
                    </span>
                  </div>
                  <div className="mt-1 text-sm text-gray-500">
                    {override.isFullDayBlock ? (
                      'Full day'
                    ) : (
                      <>
                        {override.startTime} - {override.endTime}
                      </>
                    )}
                    {override.reason && (
                      <span className="ml-2">• {override.reason}</span>
                    )}
                  </div>
                </div>
                <button
                  onClick={() => handleDelete(override.id)}
                  className="rounded-lg p-2 text-gray-400 hover:bg-white hover:text-red-600"
                  title="Delete override"
                >
                  <svg
                    className="h-5 w-5"
                    fill="none"
                    viewBox="0 0 24 24"
                    strokeWidth={1.5}
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M14.74 9l-.346 9m-4.788 0L9.26 9m9.968-3.21c.342.052.682.107 1.022.166m-1.022-.165L18.16 19.673a2.25 2.25 0 01-2.244 2.077H8.084a2.25 2.25 0 01-2.244-2.077L4.772 5.79m14.456 0a48.108 48.108 0 00-3.478-.397m-12 .562c.34-.059.68-.114 1.022-.165m0 0a48.11 48.11 0 013.478-.397m7.5 0v-.916c0-1.18-.91-2.164-2.09-2.201a51.964 51.964 0 00-3.32 0c-1.18.037-2.09 1.022-2.09 2.201v.916m7.5 0a48.667 48.667 0 00-7.5 0"
                    />
                  </svg>
                </button>
              </div>
            ))}
          </div>
        ) : (
          <div className="py-8 text-center">
            <svg
              className="mx-auto h-12 w-12 text-gray-300"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth={1}
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5"
              />
            </svg>
            <p className="mt-2 text-sm text-gray-500">
              No date overrides yet. Add one to block time off or add extra
              availability.
            </p>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
