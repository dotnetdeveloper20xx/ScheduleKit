import { format, parseISO } from 'date-fns';
import { Card, CardContent, Button } from '@/components/ui';
import type { BookingConfirmationResponse } from '@/api/types';

interface BookingConfirmationProps {
  confirmation: BookingConfirmationResponse;
}

export function BookingConfirmation({ confirmation }: BookingConfirmationProps) {
  const startTime = parseISO(confirmation.startTimeUtc);
  const endTime = parseISO(confirmation.endTimeUtc);

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="mx-auto max-w-lg px-4">
        <Card>
          <CardContent className="p-8 text-center">
            {/* Success Icon */}
            <div className="mx-auto flex h-16 w-16 items-center justify-center rounded-full bg-green-100">
              <svg
                className="h-8 w-8 text-green-600"
                fill="none"
                viewBox="0 0 24 24"
                strokeWidth={2}
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  d="M4.5 12.75l6 6 9-13.5"
                />
              </svg>
            </div>

            <h1 className="mt-6 text-2xl font-semibold text-gray-900">
              You are scheduled
            </h1>
            <p className="mt-2 text-gray-600">
              A calendar invitation has been sent to your email address.
            </p>

            {/* Meeting Details */}
            <div className="mt-8 rounded-lg border border-gray-200 bg-gray-50 p-6 text-left">
              <h2 className="font-medium text-gray-900">
                {confirmation.eventTypeName}
              </h2>
              <p className="mt-1 text-sm text-gray-500">
                with {confirmation.hostName}
              </p>

              <div className="mt-4 space-y-3">
                <div className="flex items-start gap-3">
                  <svg
                    className="mt-0.5 h-5 w-5 text-gray-400"
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
                  <div>
                    <p className="font-medium text-gray-900">
                      {format(startTime, 'EEEE, MMMM d, yyyy')}
                    </p>
                    <p className="text-sm text-gray-500">
                      {format(startTime, 'h:mm a')} - {format(endTime, 'h:mm a')}
                    </p>
                    <p className="text-xs text-gray-400">{confirmation.guestTimezone}</p>
                  </div>
                </div>

                {confirmation.meetingLink && (
                  <div className="flex items-start gap-3">
                    <svg
                      className="mt-0.5 h-5 w-5 text-gray-400"
                      fill="none"
                      viewBox="0 0 24 24"
                      strokeWidth={1.5}
                      stroke="currentColor"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        d="M13.19 8.688a4.5 4.5 0 011.242 7.244l-4.5 4.5a4.5 4.5 0 01-6.364-6.364l1.757-1.757m13.35-.622l1.757-1.757a4.5 4.5 0 00-6.364-6.364l-4.5 4.5a4.5 4.5 0 001.242 7.244"
                      />
                    </svg>
                    <div>
                      <p className="font-medium text-gray-900">Meeting Link</p>
                      <a
                        href={confirmation.meetingLink}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-sm text-primary-600 hover:text-primary-700"
                      >
                        {confirmation.meetingLink}
                      </a>
                    </div>
                  </div>
                )}
              </div>
            </div>

            {/* Actions */}
            <div className="mt-6 flex flex-col gap-3">
              {confirmation.rescheduleLink && (
                <Button
                  variant="outline"
                  onClick={() => window.location.href = confirmation.rescheduleLink}
                >
                  Reschedule
                </Button>
              )}
              {confirmation.cancellationLink && (
                <button
                  onClick={() => window.location.href = confirmation.cancellationLink}
                  className="text-sm text-gray-500 hover:text-gray-700"
                >
                  Cancel booking
                </button>
              )}
            </div>

            {/* Add to Calendar */}
            <div className="mt-8 border-t border-gray-200 pt-6">
              <p className="text-sm text-gray-500">Add to your calendar</p>
              <div className="mt-3 flex justify-center gap-4">
                <button className="rounded-lg border border-gray-200 p-3 hover:bg-gray-50">
                  <svg className="h-6 w-6" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M19.5 3h-15A1.5 1.5 0 003 4.5v15A1.5 1.5 0 004.5 21h15a1.5 1.5 0 001.5-1.5v-15A1.5 1.5 0 0019.5 3zM12 18a6 6 0 110-12 6 6 0 010 12z" />
                  </svg>
                </button>
                <button className="rounded-lg border border-gray-200 p-3 hover:bg-gray-50">
                  <svg className="h-6 w-6" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M17 2H7c-1.1 0-2 .9-2 2v16c0 1.1.9 2 2 2h10c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2zm-5 2c.83 0 1.5.67 1.5 1.5S12.83 7 12 7s-1.5-.67-1.5-1.5S11.17 4 12 4zm0 16c-2.21 0-4-1.79-4-4h2c0 1.1.9 2 2 2s2-.9 2-2h2c0 2.21-1.79 4-4 4zm4-7H8v-2h8v2z" />
                  </svg>
                </button>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
