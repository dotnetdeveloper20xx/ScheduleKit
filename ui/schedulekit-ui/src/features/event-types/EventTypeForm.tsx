import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { Button, Input, Select, TextArea, type SelectOption } from '@/components/ui';
import type { CreateEventTypeRequest, EventTypeResponse } from '@/api/types';

const eventTypeSchema = z.object({
  name: z
    .string()
    .min(1, 'Name is required')
    .max(200, 'Name must not exceed 200 characters'),
  description: z
    .string()
    .max(2000, 'Description must not exceed 2000 characters')
    .optional(),
  durationMinutes: z.coerce
    .number()
    .min(15, 'Duration must be at least 15 minutes')
    .max(480, 'Duration must not exceed 8 hours'),
  bufferBeforeMinutes: z.coerce
    .number()
    .min(0, 'Buffer must be at least 0 minutes')
    .max(120, 'Buffer must not exceed 2 hours'),
  bufferAfterMinutes: z.coerce
    .number()
    .min(0, 'Buffer must be at least 0 minutes')
    .max(120, 'Buffer must not exceed 2 hours'),
  minimumNoticeMinutes: z.coerce
    .number()
    .min(0, 'Minimum notice must be at least 0')
    .max(10080, 'Minimum notice cannot exceed 7 days'),
  bookingWindowDays: z.coerce
    .number()
    .min(1, 'Booking window must be at least 1 day')
    .max(365, 'Booking window cannot exceed 365 days'),
  maxBookingsPerDay: z.coerce
    .number()
    .min(1, 'Must allow at least 1 booking per day')
    .optional()
    .or(z.literal('')),
  locationType: z.string().min(1, 'Location type is required'),
  locationDetails: z.string().optional(),
  color: z.string().optional(),
});

type EventTypeFormData = z.infer<typeof eventTypeSchema>;

interface EventTypeFormProps {
  defaultValues?: Partial<EventTypeResponse>;
  onSubmit: (data: CreateEventTypeRequest) => void;
  isLoading?: boolean;
  submitLabel?: string;
}

const locationOptions: SelectOption[] = [
  { value: 'Zoom', label: 'Zoom Meeting' },
  { value: 'GoogleMeet', label: 'Google Meet' },
  { value: 'MicrosoftTeams', label: 'Microsoft Teams' },
  { value: 'Phone', label: 'Phone Call' },
  { value: 'InPerson', label: 'In Person' },
  { value: 'Custom', label: 'Custom Location' },
];

const durationOptions: SelectOption[] = [
  { value: '15', label: '15 minutes' },
  { value: '30', label: '30 minutes' },
  { value: '45', label: '45 minutes' },
  { value: '60', label: '1 hour' },
  { value: '90', label: '1.5 hours' },
  { value: '120', label: '2 hours' },
];

const bufferOptions: SelectOption[] = [
  { value: '0', label: 'No buffer' },
  { value: '5', label: '5 minutes' },
  { value: '10', label: '10 minutes' },
  { value: '15', label: '15 minutes' },
  { value: '30', label: '30 minutes' },
  { value: '60', label: '1 hour' },
];

const minimumNoticeOptions: SelectOption[] = [
  { value: '0', label: 'No minimum notice' },
  { value: '15', label: '15 minutes' },
  { value: '30', label: '30 minutes' },
  { value: '60', label: '1 hour' },
  { value: '120', label: '2 hours' },
  { value: '240', label: '4 hours' },
  { value: '720', label: '12 hours' },
  { value: '1440', label: '24 hours' },
  { value: '2880', label: '48 hours' },
];

const bookingWindowOptions: SelectOption[] = [
  { value: '7', label: '1 week' },
  { value: '14', label: '2 weeks' },
  { value: '30', label: '30 days' },
  { value: '60', label: '60 days' },
  { value: '90', label: '90 days' },
  { value: '180', label: '6 months' },
  { value: '365', label: '1 year' },
];

const colorOptions = [
  '#3b82f6', // Blue
  '#10b981', // Green
  '#f59e0b', // Amber
  '#ef4444', // Red
  '#8b5cf6', // Purple
  '#ec4899', // Pink
  '#06b6d4', // Cyan
  '#84cc16', // Lime
];

export function EventTypeForm({
  defaultValues,
  onSubmit,
  isLoading = false,
  submitLabel = 'Create Event Type',
}: EventTypeFormProps) {
  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<EventTypeFormData>({
    resolver: zodResolver(eventTypeSchema),
    defaultValues: {
      name: defaultValues?.name ?? '',
      description: defaultValues?.description ?? '',
      durationMinutes: defaultValues?.durationMinutes ?? 30,
      bufferBeforeMinutes: defaultValues?.bufferBeforeMinutes ?? 0,
      bufferAfterMinutes: defaultValues?.bufferAfterMinutes ?? 0,
      minimumNoticeMinutes: defaultValues?.minimumNoticeMinutes ?? 60,
      bookingWindowDays: defaultValues?.bookingWindowDays ?? 60,
      maxBookingsPerDay: defaultValues?.maxBookingsPerDay ?? '',
      locationType: defaultValues?.locationType ?? 'Zoom',
      locationDetails: defaultValues?.locationDetails ?? '',
      color: defaultValues?.color ?? '#3b82f6',
    },
  });

  const selectedColor = watch('color');
  const locationType = watch('locationType');

  const showLocationDetails = ['Phone', 'InPerson', 'Custom'].includes(
    locationType
  );

  const locationDetailsLabel =
    locationType === 'Phone'
      ? 'Phone Number'
      : locationType === 'InPerson'
        ? 'Address'
        : 'Instructions';

  const handleFormSubmit = (data: EventTypeFormData) => {
    onSubmit({
      name: data.name,
      description: data.description || undefined,
      durationMinutes: data.durationMinutes,
      bufferBeforeMinutes: data.bufferBeforeMinutes,
      bufferAfterMinutes: data.bufferAfterMinutes,
      minimumNoticeMinutes: data.minimumNoticeMinutes,
      bookingWindowDays: data.bookingWindowDays,
      maxBookingsPerDay: data.maxBookingsPerDay ? Number(data.maxBookingsPerDay) : undefined,
      locationType: data.locationType,
      locationDetails: data.locationDetails || undefined,
      color: data.color || undefined,
    });
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6">
      {/* Basic Info */}
      <div className="space-y-4">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">Basic Information</h3>

        <Input
          label="Event Name"
          placeholder="e.g., 30-minute Consultation"
          error={errors.name?.message}
          required
          {...register('name')}
        />

        <TextArea
          label="Description"
          placeholder="Describe what this meeting is about..."
          rows={3}
          error={errors.description?.message}
          {...register('description')}
        />
      </div>

      {/* Duration & Buffers */}
      <div className="space-y-4">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">Duration & Buffers</h3>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <Select
            label="Duration"
            options={durationOptions}
            error={errors.durationMinutes?.message}
            required
            {...register('durationMinutes')}
          />

          <Select
            label="Buffer Before"
            options={bufferOptions}
            error={errors.bufferBeforeMinutes?.message}
            hint="Time before meeting"
            {...register('bufferBeforeMinutes')}
          />

          <Select
            label="Buffer After"
            options={bufferOptions}
            error={errors.bufferAfterMinutes?.message}
            hint="Time after meeting"
            {...register('bufferAfterMinutes')}
          />
        </div>
      </div>

      {/* Scheduling Controls */}
      <div className="space-y-4">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">Scheduling Controls</h3>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
          <Select
            label="Minimum Notice"
            options={minimumNoticeOptions}
            error={errors.minimumNoticeMinutes?.message}
            hint="How far in advance guests must book"
            {...register('minimumNoticeMinutes')}
          />

          <Select
            label="Booking Window"
            options={bookingWindowOptions}
            error={errors.bookingWindowDays?.message}
            hint="How far ahead guests can book"
            {...register('bookingWindowDays')}
          />

          <Input
            label="Max Bookings Per Day"
            type="number"
            min={1}
            placeholder="Unlimited"
            error={errors.maxBookingsPerDay?.message}
            hint="Leave empty for no limit"
            {...register('maxBookingsPerDay')}
          />
        </div>
      </div>

      {/* Location */}
      <div className="space-y-4">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">Meeting Location</h3>

        <Select
          label="Location Type"
          options={locationOptions}
          error={errors.locationType?.message}
          required
          {...register('locationType')}
        />

        {showLocationDetails && (
          <Input
            label={locationDetailsLabel}
            placeholder={
              locationType === 'Phone'
                ? '+1 (555) 123-4567'
                : locationType === 'InPerson'
                  ? '123 Main St, City, State'
                  : 'Enter meeting instructions...'
            }
            error={errors.locationDetails?.message}
            {...register('locationDetails')}
          />
        )}
      </div>

      {/* Color */}
      <div className="space-y-4">
        <h3 className="text-lg font-medium text-gray-900 dark:text-white">Appearance</h3>

        <div>
          <label className="mb-2 block text-sm font-medium text-gray-700">
            Color
          </label>
          <div className="flex flex-wrap gap-2">
            {colorOptions.map((color) => (
              <button
                key={color}
                type="button"
                onClick={() => setValue('color', color)}
                className={`h-8 w-8 rounded-full border-2 transition-transform hover:scale-110 ${
                  selectedColor === color
                    ? 'border-gray-900 ring-2 ring-gray-900 ring-offset-2'
                    : 'border-transparent'
                }`}
                style={{ backgroundColor: color }}
                title={color}
              />
            ))}
          </div>
        </div>
      </div>

      {/* Submit */}
      <div className="flex items-center justify-end gap-3 border-t border-gray-200 pt-6">
        <Button type="button" variant="outline" onClick={() => history.back()}>
          Cancel
        </Button>
        <Button type="submit" isLoading={isLoading}>
          {submitLabel}
        </Button>
      </div>
    </form>
  );
}
