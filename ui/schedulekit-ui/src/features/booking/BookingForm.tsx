import { useState } from 'react';
import { format, parseISO } from 'date-fns';
import { Button, Card, CardContent, Input, TextArea } from '@/components/ui';
import { useCreatePublicBooking } from '@/api/hooks/useBookings';
import type { EventTypeResponse, CreatePublicBookingRequest } from '@/api/types';
import { BookingConfirmation } from './BookingConfirmation';

interface BookingFormProps {
  eventType: EventTypeResponse;
  selectedDate: string;
  selectedSlot: string;
  timezone: string;
  onBack: () => void;
}

export function BookingForm({
  eventType,
  selectedDate,
  selectedSlot,
  timezone,
  onBack,
}: BookingFormProps) {
  const [guestName, setGuestName] = useState('');
  const [guestEmail, setGuestEmail] = useState('');
  const [guestPhone, setGuestPhone] = useState('');
  const [guestNotes, setGuestNotes] = useState('');
  const [questionResponses, setQuestionResponses] = useState<Record<string, string>>({});
  const [errors, setErrors] = useState<Record<string, string>>({});

  const createBooking = useCreatePublicBooking();

  // Calculate start time UTC from selected date and slot
  const getStartTimeUtc = () => {
    // The slot time is already in the guest's timezone format
    // We need to convert it to UTC
    const [hours, minutes] = selectedSlot.split(':').map(Number);
    const dateObj = parseISO(selectedDate);
    dateObj.setHours(hours, minutes, 0, 0);

    // For now, we'll send the local datetime and let the backend handle timezone conversion
    // The backend will receive the timezone parameter to properly convert
    return dateObj.toISOString();
  };

  const validate = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!guestName.trim()) {
      newErrors.name = 'Name is required';
    }

    if (!guestEmail.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(guestEmail)) {
      newErrors.email = 'Please enter a valid email address';
    }

    // Validate required questions
    eventType.questions
      .filter((q) => q.isRequired)
      .forEach((q) => {
        if (!questionResponses[q.id]?.trim()) {
          newErrors[`question_${q.id}`] = 'This field is required';
        }
      });

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!validate()) return;

    const data: CreatePublicBookingRequest = {
      eventTypeId: eventType.id,
      guestName: guestName.trim(),
      guestEmail: guestEmail.trim(),
      guestPhone: guestPhone.trim() || undefined,
      guestNotes: guestNotes.trim() || undefined,
      startTimeUtc: getStartTimeUtc(),
      guestTimezone: timezone,
      questionResponses: Object.entries(questionResponses)
        .filter(([, value]) => value.trim())
        .map(([questionId, responseValue]) => ({
          questionId,
          responseValue: responseValue.trim(),
        })),
    };

    createBooking.mutate(data);
  };

  if (createBooking.isSuccess && createBooking.data) {
    return <BookingConfirmation confirmation={createBooking.data} />;
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="mx-auto max-w-2xl px-4">
        <Card>
          <CardContent className="p-6">
            {/* Header */}
            <div className="mb-6">
              <button
                onClick={onBack}
                className="mb-4 flex items-center gap-1 text-sm text-gray-600 hover:text-gray-900"
              >
                <svg
                  className="h-4 w-4"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth={1.5}
                  stroke="currentColor"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M15.75 19.5L8.25 12l7.5-7.5"
                  />
                </svg>
                Back
              </button>
              <h1 className="text-xl font-semibold text-gray-900">{eventType.name}</h1>
              <div className="mt-2 flex items-center gap-4 text-sm text-gray-600">
                <span className="flex items-center gap-1">
                  <svg
                    className="h-4 w-4"
                    fill="none"
                    viewBox="0 0 24 24"
                    strokeWidth={1.5}
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5"
                    />
                  </svg>
                  {format(parseISO(selectedDate), 'EEEE, MMMM d, yyyy')}
                </span>
                <span className="flex items-center gap-1">
                  <svg
                    className="h-4 w-4"
                    fill="none"
                    viewBox="0 0 24 24"
                    strokeWidth={1.5}
                    stroke="currentColor"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      d="M12 6v6h4.5m4.5 0a9 9 0 11-18 0 9 9 0 0118 0z"
                    />
                  </svg>
                  {selectedSlot}
                </span>
                <span>{eventType.durationMinutes} min</span>
              </div>
            </div>

            {/* Form */}
            <form onSubmit={handleSubmit} className="space-y-4">
              <Input
                label="Your name *"
                value={guestName}
                onChange={(e) => setGuestName(e.target.value)}
                error={errors.name}
                placeholder="John Doe"
              />

              <Input
                label="Email address *"
                type="email"
                value={guestEmail}
                onChange={(e) => setGuestEmail(e.target.value)}
                error={errors.email}
                placeholder="john@example.com"
              />

              <Input
                label="Phone number"
                type="tel"
                value={guestPhone}
                onChange={(e) => setGuestPhone(e.target.value)}
                placeholder="+1 (555) 123-4567"
              />

              <TextArea
                label="Additional notes"
                value={guestNotes}
                onChange={(e) => setGuestNotes(e.target.value)}
                placeholder="Share anything that will help prepare for our meeting..."
                rows={3}
              />

              {/* Custom Questions */}
              {eventType.questions.length > 0 && (
                <div className="space-y-4 border-t border-gray-200 pt-4">
                  <h3 className="text-sm font-medium text-gray-900">
                    Additional Questions
                  </h3>
                  {eventType.questions
                    .sort((a, b) => a.displayOrder - b.displayOrder)
                    .map((question) => (
                      <div key={question.id}>
                        {question.type === 'Text' && (
                          <Input
                            label={`${question.questionText}${question.isRequired ? ' *' : ''}`}
                            value={questionResponses[question.id] || ''}
                            onChange={(e) =>
                              setQuestionResponses((prev) => ({
                                ...prev,
                                [question.id]: e.target.value,
                              }))
                            }
                            error={errors[`question_${question.id}`]}
                          />
                        )}
                        {question.type === 'TextArea' && (
                          <TextArea
                            label={`${question.questionText}${question.isRequired ? ' *' : ''}`}
                            value={questionResponses[question.id] || ''}
                            onChange={(e) =>
                              setQuestionResponses((prev) => ({
                                ...prev,
                                [question.id]: e.target.value,
                              }))
                            }
                            error={errors[`question_${question.id}`]}
                            rows={3}
                          />
                        )}
                        {question.type === 'SingleSelect' && (
                          <div>
                            <label className="mb-1.5 block text-sm font-medium text-gray-700">
                              {question.questionText}
                              {question.isRequired && ' *'}
                            </label>
                            <select
                              value={questionResponses[question.id] || ''}
                              onChange={(e) =>
                                setQuestionResponses((prev) => ({
                                  ...prev,
                                  [question.id]: e.target.value,
                                }))
                              }
                              className="block w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:border-primary-500 focus:outline-none focus:ring-1 focus:ring-primary-500"
                            >
                              <option value="">Select an option</option>
                              {question.options.map((option) => (
                                <option key={option} value={option}>
                                  {option}
                                </option>
                              ))}
                            </select>
                            {errors[`question_${question.id}`] && (
                              <p className="mt-1 text-sm text-red-600">
                                {errors[`question_${question.id}`]}
                              </p>
                            )}
                          </div>
                        )}
                        {question.type === 'Checkbox' && (
                          <div>
                            <label className="flex items-center gap-2">
                              <input
                                type="checkbox"
                                checked={questionResponses[question.id] === 'true'}
                                onChange={(e) =>
                                  setQuestionResponses((prev) => ({
                                    ...prev,
                                    [question.id]: e.target.checked.toString(),
                                  }))
                                }
                                className="h-4 w-4 rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                              />
                              <span className="text-sm text-gray-700">
                                {question.questionText}
                                {question.isRequired && ' *'}
                              </span>
                            </label>
                            {errors[`question_${question.id}`] && (
                              <p className="mt-1 text-sm text-red-600">
                                {errors[`question_${question.id}`]}
                              </p>
                            )}
                          </div>
                        )}
                      </div>
                    ))}
                </div>
              )}

              {createBooking.error && (
                <div className="rounded-lg bg-red-50 p-4 text-sm text-red-700">
                  Failed to create booking. Please try again.
                </div>
              )}

              <div className="flex gap-3 pt-4">
                <Button
                  type="button"
                  variant="outline"
                  onClick={onBack}
                  className="flex-1"
                >
                  Back
                </Button>
                <Button
                  type="submit"
                  isLoading={createBooking.isPending}
                  className="flex-1"
                >
                  Schedule Meeting
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
