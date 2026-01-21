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

                {/* Location */}
                <div className="flex items-start gap-3">
                  {confirmation.locationType === 'Zoom' && (
                    <svg className="mt-0.5 h-5 w-5 text-blue-500" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M4.5 4.5a3 3 0 00-3 3v9a3 3 0 003 3h10.5a3 3 0 003-3v-2.25l3.75 2.5V7.25l-3.75 2.5V7.5a3 3 0 00-3-3H4.5z" />
                    </svg>
                  )}
                  {confirmation.locationType === 'GoogleMeet' && (
                    <svg className="mt-0.5 h-5 w-5 text-green-600" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M4.5 4.5a3 3 0 00-3 3v9a3 3 0 003 3h10.5a3 3 0 003-3v-2.25l3.75 2.5V7.25l-3.75 2.5V7.5a3 3 0 00-3-3H4.5z" />
                    </svg>
                  )}
                  {confirmation.locationType === 'MicrosoftTeams' && (
                    <svg className="mt-0.5 h-5 w-5 text-purple-600" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M4.5 4.5a3 3 0 00-3 3v9a3 3 0 003 3h10.5a3 3 0 003-3v-2.25l3.75 2.5V7.25l-3.75 2.5V7.5a3 3 0 00-3-3H4.5z" />
                    </svg>
                  )}
                  {confirmation.locationType === 'Phone' && (
                    <svg className="mt-0.5 h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 6.75c0 8.284 6.716 15 15 15h2.25a2.25 2.25 0 002.25-2.25v-1.372c0-.516-.351-.966-.852-1.091l-4.423-1.106c-.44-.11-.902.055-1.173.417l-.97 1.293c-.282.376-.769.542-1.21.38a12.035 12.035 0 01-7.143-7.143c-.162-.441.004-.928.38-1.21l1.293-.97c.363-.271.527-.734.417-1.173L6.963 3.102a1.125 1.125 0 00-1.091-.852H4.5A2.25 2.25 0 002.25 4.5v2.25z" />
                    </svg>
                  )}
                  {confirmation.locationType === 'InPerson' && (
                    <svg className="mt-0.5 h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M15 10.5a3 3 0 11-6 0 3 3 0 016 0z" />
                      <path strokeLinecap="round" strokeLinejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1115 0z" />
                    </svg>
                  )}
                  {confirmation.locationType === 'Custom' && (
                    <svg className="mt-0.5 h-5 w-5 text-gray-400" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M9.879 7.519c1.171-1.025 3.071-1.025 4.242 0 1.172 1.025 1.172 2.687 0 3.712-.203.179-.43.326-.67.442-.745.361-1.45.999-1.45 1.827v.75M21 12a9 9 0 11-18 0 9 9 0 0118 0zm-9 5.25h.008v.008H12v-.008z" />
                    </svg>
                  )}
                  <div>
                    <p className="font-medium text-gray-900">
                      {confirmation.locationDisplayName || confirmation.locationType}
                    </p>
                    {confirmation.locationDetails && (
                      <p className="text-sm text-gray-500">{confirmation.locationDetails}</p>
                    )}
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
                        className="text-sm text-primary-600 hover:text-primary-700 break-all"
                      >
                        Join Meeting
                      </a>
                      {confirmation.meetingPassword && (
                        <p className="mt-1 text-sm text-gray-500">
                          Password: <code className="rounded bg-gray-100 px-1">{confirmation.meetingPassword}</code>
                        </p>
                      )}
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
              {confirmation.calendarLink ? (
                <div className="mt-3">
                  <a
                    href={confirmation.calendarLink}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="inline-flex items-center gap-2 rounded-lg border border-gray-200 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
                  >
                    <svg className="h-5 w-5 text-green-600" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 012.25-2.25h13.5A2.25 2.25 0 0121 7.5v11.25m-18 0A2.25 2.25 0 005.25 21h13.5A2.25 2.25 0 0021 18.75m-18 0v-7.5A2.25 2.25 0 015.25 9h13.5A2.25 2.25 0 0121 11.25v7.5" />
                    </svg>
                    View in Calendar
                  </a>
                  <p className="mt-2 text-xs text-gray-400">Event synced to your calendar</p>
                </div>
              ) : (
                <div className="mt-3 flex justify-center gap-4">
                  <button className="rounded-lg border border-gray-200 p-3 hover:bg-gray-50" title="Google Calendar">
                    <svg className="h-6 w-6" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M19.5 3h-15A1.5 1.5 0 003 4.5v15A1.5 1.5 0 004.5 21h15a1.5 1.5 0 001.5-1.5v-15A1.5 1.5 0 0019.5 3zM12 18a6 6 0 110-12 6 6 0 010 12z" />
                    </svg>
                  </button>
                  <button className="rounded-lg border border-gray-200 p-3 hover:bg-gray-50" title="Outlook">
                    <svg className="h-6 w-6" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M17 2H7c-1.1 0-2 .9-2 2v16c0 1.1.9 2 2 2h10c1.1 0 2-.9 2-2V4c0-1.1-.9-2-2-2zm-5 2c.83 0 1.5.67 1.5 1.5S12.83 7 12 7s-1.5-.67-1.5-1.5S11.17 4 12 4zm0 16c-2.21 0-4-1.79-4-4h2c0 1.1.9 2 2 2s2-.9 2-2h2c0 2.21-1.79 4-4 4zm4-7H8v-2h8v2z" />
                    </svg>
                  </button>
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
