// API Response Types

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  error?: ApiError;
  meta?: Record<string, unknown>;
}

export interface ApiError {
  code: string;
  message: string;
  details?: FieldError[];
}

export interface FieldError {
  field: string;
  message: string;
}

// Event Type Types

export interface EventTypeResponse {
  id: string;
  hostUserId: string;
  name: string;
  slug: string;
  description?: string;
  durationMinutes: number;
  bufferBeforeMinutes: number;
  bufferAfterMinutes: number;
  minimumNoticeMinutes: number;
  bookingWindowDays: number;
  maxBookingsPerDay?: number;
  locationType: LocationType;
  locationDetails?: string;
  locationDisplayName?: string;
  isActive: boolean;
  color?: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
  questions: BookingQuestionResponse[];
}

export interface BookingQuestionResponse {
  id: string;
  questionText: string;
  type: QuestionType;
  isRequired: boolean;
  options: string[];
  displayOrder: number;
}

export interface AddBookingQuestionRequest {
  questionText: string;
  type: QuestionType;
  isRequired: boolean;
  options?: string[];
}

export interface UpdateBookingQuestionRequest {
  questionText: string;
  type: QuestionType;
  isRequired: boolean;
  options?: string[];
}

export interface ReorderQuestionsRequest {
  questionIds: string[];
}

export interface RescheduleBookingInfoResponse {
  bookingId: string;
  eventTypeId: string;
  eventTypeName: string;
  durationMinutes: number;
  guestName: string;
  guestEmail: string;
  currentStartTimeUtc: string;
  currentEndTimeUtc: string;
  guestTimezone: string;
  status: string;
  locationType: string;
  locationDetails?: string;
  locationDisplayName?: string;
}

export interface PublicRescheduleRequest {
  newStartTimeUtc: string;
}

export type LocationType =
  | 'Zoom'
  | 'GoogleMeet'
  | 'MicrosoftTeams'
  | 'Phone'
  | 'InPerson'
  | 'Custom';

export type QuestionType =
  | 'Text'
  | 'TextArea'
  | 'SingleSelect'
  | 'MultiSelect'
  | 'Checkbox';

export interface CreateEventTypeRequest {
  name: string;
  description?: string;
  durationMinutes: number;
  bufferBeforeMinutes: number;
  bufferAfterMinutes: number;
  minimumNoticeMinutes: number;
  bookingWindowDays: number;
  maxBookingsPerDay?: number;
  locationType: string;
  locationDetails?: string;
  color?: string;
}

export interface UpdateEventTypeRequest {
  name: string;
  description?: string;
  durationMinutes: number;
  bufferBeforeMinutes: number;
  bufferAfterMinutes: number;
  minimumNoticeMinutes: number;
  bookingWindowDays: number;
  maxBookingsPerDay?: number;
  locationType: string;
  locationDetails?: string;
  color?: string;
}

// Paginated Response

export interface PaginatedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

// Availability Types

export interface WeeklyAvailabilityResponse {
  hostUserId: string;
  timezone: string;
  days: DayAvailabilityResponse[];
}

export interface DayAvailabilityResponse {
  id: string;
  dayOfWeek: number;
  dayName: string;
  startTime: string;
  endTime: string;
  isEnabled: boolean;
}

export interface UpdateWeeklyAvailabilityRequest {
  days: DayAvailabilityUpdate[];
}

export interface DayAvailabilityUpdate {
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  isEnabled: boolean;
}

export interface AvailabilityOverrideResponse {
  id: string;
  hostUserId: string;
  date: string;
  startTime?: string;
  endTime?: string;
  isBlocked: boolean;
  isFullDayBlock: boolean;
  reason?: string;
  createdAtUtc: string;
}

export interface CreateAvailabilityOverrideRequest {
  date: string;
  startTime?: string;
  endTime?: string;
  isBlocked: boolean;
  reason?: string;
}

// Slots Types

export interface AvailableSlotsResponse {
  date: string;
  timezone: string;
  eventTypeId: string;
  slots: TimeSlotResponse[];
}

export interface TimeSlotResponse {
  startTime: string;
  endTime: string;
  startTimeUtc: string;
  endTimeUtc: string;
  isAvailable: boolean;
}

export interface AvailableDatesResponse {
  eventTypeId: string;
  fromDate: string;
  toDate: string;
  timezone: string;
  dates: DateAvailabilityResponse[];
}

export interface DateAvailabilityResponse {
  date: string;
  dayOfWeek: number;
  hasAvailability: boolean;
  availableSlotCount: number;
}

// Booking Types

export interface BookingResponse {
  id: string;
  eventTypeId: string;
  eventTypeName: string;
  hostUserId: string;
  guestName: string;
  guestEmail: string;
  guestPhone?: string;
  guestNotes?: string;
  startTimeUtc: string;
  endTimeUtc: string;
  guestTimezone: string;
  status: BookingStatus;
  cancellationReason?: string;
  cancelledAtUtc?: string;
  meetingLink?: string;
  meetingPassword?: string;
  calendarLink?: string;
  locationType: string;
  locationDetails?: string;
  locationDisplayName?: string;
  createdAtUtc: string;
  responses: BookingQuestionAnswerResponse[];
}

export type BookingStatus = 'Confirmed' | 'Cancelled' | 'Completed' | 'NoShow';

export interface BookingQuestionAnswerResponse {
  questionId: string;
  questionText: string;
  responseValue: string;
}

export interface CreatePublicBookingRequest {
  eventTypeId: string;
  guestName: string;
  guestEmail: string;
  guestPhone?: string;
  guestNotes?: string;
  startTimeUtc: string;
  guestTimezone: string;
  questionResponses?: QuestionResponseRequest[];
}

export interface QuestionResponseRequest {
  questionId: string;
  responseValue: string;
}

export interface BookingConfirmationResponse {
  bookingId: string;
  eventTypeName: string;
  hostName: string;
  startTimeUtc: string;
  endTimeUtc: string;
  guestTimezone: string;
  meetingLink?: string;
  meetingPassword?: string;
  calendarLink?: string;
  locationType: string;
  locationDetails?: string;
  locationDisplayName?: string;
  cancellationLink: string;
  rescheduleLink: string;
}

// Auth Types

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
  slug?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserResponse;
}

export interface UserResponse {
  id: string;
  email: string;
  name: string;
  slug?: string;
  timezone: string;
}

export interface UserProfileResponse {
  id: string;
  email: string;
  name: string;
  slug?: string;
  timezone: string;
  emailNotificationsEnabled: boolean;
  reminderEmailsEnabled: boolean;
  reminderHoursBefore: number;
  isActive: boolean;
  lastLoginAt?: string;
}

export interface UpdateProfileRequest {
  name: string;
  slug?: string;
}

export interface UpdateTimezoneRequest {
  timezone: string;
}

export interface UpdateEmailPreferencesRequest {
  emailNotificationsEnabled: boolean;
  reminderEmailsEnabled: boolean;
  reminderHoursBefore: number;
}

// OAuth Types

export interface OAuthProvidersResponse {
  providers: OAuthProviderInfo[];
}

export interface OAuthProviderInfo {
  name: string;
  displayName: string;
  iconClass: string;
}

export interface OAuthAuthorizeResponse {
  authorizationUrl: string;
  state: string;
}

export interface OAuthCallbackRequest {
  provider: string;
  code: string;
  state: string;
  redirectUri: string;
}
