using FluentValidation;
using MediatR;
using ScheduleKit.Application.Common;
using ScheduleKit.Application.Common.DTOs;
using ScheduleKit.Application.Common.Interfaces;
using ScheduleKit.Domain.Common;
using ScheduleKit.Domain.Entities;
using ScheduleKit.Domain.Interfaces;
using ScheduleKit.Domain.Services;
using ScheduleKit.Domain.ValueObjects;

namespace ScheduleKit.Application.Commands.Bookings;

/// <summary>
/// Command to create a public booking (guest-facing).
/// </summary>
public record CreatePublicBookingCommand : ICommand<BookingConfirmationResponse>
{
    public Guid EventTypeId { get; init; }
    public string GuestName { get; init; } = string.Empty;
    public string GuestEmail { get; init; } = string.Empty;
    public string? GuestPhone { get; init; }
    public string? GuestNotes { get; init; }
    public DateTime StartTimeUtc { get; init; }
    public string GuestTimezone { get; init; } = string.Empty;
    public List<QuestionResponseDto> QuestionResponses { get; init; } = new();
}

/// <summary>
/// DTO for question response in booking.
/// </summary>
public record QuestionResponseDto
{
    public Guid QuestionId { get; init; }
    public string ResponseValue { get; init; } = string.Empty;
}

/// <summary>
/// Validator for CreatePublicBookingCommand.
/// </summary>
public class CreatePublicBookingCommandValidator : AbstractValidator<CreatePublicBookingCommand>
{
    public CreatePublicBookingCommandValidator()
    {
        RuleFor(x => x.EventTypeId)
            .NotEmpty().WithMessage("Event type ID is required.");

        RuleFor(x => x.GuestName)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.GuestEmail)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.GuestPhone)
            .MaximumLength(50).WithMessage("Phone must not exceed 50 characters.")
            .When(x => !string.IsNullOrEmpty(x.GuestPhone));

        RuleFor(x => x.GuestNotes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters.")
            .When(x => !string.IsNullOrEmpty(x.GuestNotes));

        RuleFor(x => x.StartTimeUtc)
            .NotEmpty().WithMessage("Start time is required.")
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5))
            .WithMessage("Cannot book in the past.");

        RuleFor(x => x.GuestTimezone)
            .NotEmpty().WithMessage("Timezone is required.");
    }
}

/// <summary>
/// Handler for CreatePublicBookingCommand.
/// </summary>
public class CreatePublicBookingCommandHandler
    : IRequestHandler<CreatePublicBookingCommand, Result<BookingConfirmationResponse>>
{
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly IAvailabilityOverrideRepository _overrideRepository;
    private readonly ISlotCalculator _slotCalculator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRealTimeNotificationService _notificationService;
    private readonly IVideoConferenceService _videoConferenceService;
    private readonly IExternalCalendarService _calendarService;

    public CreatePublicBookingCommandHandler(
        IEventTypeRepository eventTypeRepository,
        IBookingRepository bookingRepository,
        IAvailabilityRepository availabilityRepository,
        IAvailabilityOverrideRepository overrideRepository,
        ISlotCalculator slotCalculator,
        IUnitOfWork unitOfWork,
        IRealTimeNotificationService notificationService,
        IVideoConferenceService videoConferenceService,
        IExternalCalendarService calendarService)
    {
        _eventTypeRepository = eventTypeRepository;
        _bookingRepository = bookingRepository;
        _availabilityRepository = availabilityRepository;
        _overrideRepository = overrideRepository;
        _slotCalculator = slotCalculator;
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _videoConferenceService = videoConferenceService;
        _calendarService = calendarService;
    }

    public async Task<Result<BookingConfirmationResponse>> Handle(
        CreatePublicBookingCommand request,
        CancellationToken cancellationToken)
    {
        // Get event type
        var eventType = await _eventTypeRepository.GetByIdAsync(
            request.EventTypeId, cancellationToken);

        if (eventType == null)
            return Result.Failure<BookingConfirmationResponse>("Event type not found.");

        if (!eventType.IsActive)
            return Result.Failure<BookingConfirmationResponse>("This event type is not accepting bookings.");

        // Validate the slot is available
        var date = DateOnly.FromDateTime(request.StartTimeUtc);
        var availabilities = await _availabilityRepository.GetByHostUserIdAsync(
            eventType.HostUserId, cancellationToken);
        var overrides = await _overrideRepository.GetByHostUserIdAsync(
            eventType.HostUserId, date, date, cancellationToken);
        var existingBookings = await _bookingRepository.GetByEventTypeIdAsync(
            request.EventTypeId,
            request.StartTimeUtc.AddHours(-24),
            request.StartTimeUtc.AddHours(24),
            cancellationToken);

        // TODO: Get host timezone from user preferences
        var hostTimezone = "UTC";
        var slots = _slotCalculator.CalculateSlotsForDate(
            eventType, availabilities, overrides, existingBookings, date, hostTimezone);

        var matchingSlot = slots.FirstOrDefault(s =>
            Math.Abs((s.StartTimeUtc - request.StartTimeUtc).TotalMinutes) < 2);

        if (matchingSlot == null)
            return Result.Failure<BookingConfirmationResponse>(
                "The selected time slot is not available.");

        // Calculate end time
        var endTimeUtc = request.StartTimeUtc.Add(eventType.Duration.ToTimeSpan());

        // Create TimeSlot value object
        var timeSlotResult = TimeSlot.Create(request.StartTimeUtc, eventType.Duration);
        if (timeSlotResult.IsFailure)
            return Result.Failure<BookingConfirmationResponse>(timeSlotResult.Error);

        // Create GuestInfo
        var guestInfoResult = GuestInfo.Create(
            request.GuestName,
            request.GuestEmail,
            request.GuestPhone,
            request.GuestTimezone);

        if (guestInfoResult.IsFailure)
            return Result.Failure<BookingConfirmationResponse>(guestInfoResult.Error);

        var guestInfo = guestInfoResult.Value;

        // Create booking
        var bookingResult = Booking.Create(
            eventType,
            guestInfo,
            timeSlotResult.Value,
            request.GuestNotes);

        if (bookingResult.IsFailure)
            return Result.Failure<BookingConfirmationResponse>(bookingResult.Error);

        var booking = bookingResult.Value;

        // Add question responses
        foreach (var response in request.QuestionResponses)
        {
            var addResult = booking.AddResponse(response.QuestionId, response.ResponseValue);
            if (addResult.IsFailure)
                return Result.Failure<BookingConfirmationResponse>(addResult.Error);
        }

        // Create video conference meeting if needed
        var locationType = eventType.Location.Type.ToString();
        if (_videoConferenceService.SupportedLocationTypes.Contains(locationType))
        {
            var meetingRequest = new MeetingRequest
            {
                BookingId = booking.Id,
                HostUserId = eventType.HostUserId,
                Title = $"{eventType.Name} with {request.GuestName}",
                Description = $"Booking for {eventType.Name}",
                StartTimeUtc = booking.StartTimeUtc,
                EndTimeUtc = booking.EndTimeUtc,
                DurationMinutes = eventType.Duration.Minutes,
                GuestEmail = request.GuestEmail,
                GuestName = request.GuestName,
                LocationType = locationType
            };

            var meetingResult = await _videoConferenceService.CreateMeetingAsync(
                meetingRequest, cancellationToken);

            if (meetingResult.Success)
            {
                booking.SetMeetingInfo(
                    meetingResult.JoinUrl,
                    meetingResult.Password,
                    meetingResult.ExternalMeetingId);
            }
        }

        // Create calendar event
        var calendarRequest = new CalendarEventRequest
        {
            BookingId = booking.Id,
            HostUserId = eventType.HostUserId,
            Title = $"{eventType.Name} with {request.GuestName}",
            Description = $"Booking via ScheduleKit\n\nGuest: {request.GuestName}\nEmail: {request.GuestEmail}",
            StartTimeUtc = booking.StartTimeUtc,
            EndTimeUtc = booking.EndTimeUtc,
            Location = eventType.Location.DisplayName ?? eventType.Location.Type.ToString(),
            GuestEmail = request.GuestEmail,
            GuestName = request.GuestName,
            MeetingUrl = booking.MeetingLink
        };

        var calendarResult = await _calendarService.CreateEventAsync(
            calendarRequest, cancellationToken);

        if (calendarResult.Success)
        {
            booking.SetCalendarInfo(calendarResult.ExternalEventId, calendarResult.CalendarLink);
        }

        await _bookingRepository.AddAsync(booking, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Send real-time notifications
        await _notificationService.NotifySlotBookedAsync(
            eventType.Id, booking.StartTimeUtc, cancellationToken);

        await _notificationService.NotifyBookingCreatedAsync(
            new BookingCreatedNotification
            {
                HostUserId = eventType.HostUserId,
                BookingId = booking.Id,
                EventTypeId = eventType.Id,
                EventTypeName = eventType.Name,
                GuestName = booking.GuestName,
                GuestEmail = booking.GuestEmail,
                StartTimeUtc = booking.StartTimeUtc,
                EndTimeUtc = booking.EndTimeUtc,
                GuestTimezone = booking.GuestTimezone,
                CreatedAtUtc = booking.CreatedAtUtc
            },
            cancellationToken);

        // Generate links (in production, these would be proper URLs)
        var baseUrl = "https://app.schedulekit.com";
        var cancellationLink = $"{baseUrl}/cancel/{booking.Id}";
        var rescheduleLink = $"{baseUrl}/reschedule/{booking.RescheduleToken}";

        return Result.Success(new BookingConfirmationResponse
        {
            BookingId = booking.Id,
            EventTypeName = eventType.Name,
            HostName = "Host", // TODO: Get from user preferences
            StartTimeUtc = booking.StartTimeUtc,
            EndTimeUtc = booking.EndTimeUtc,
            GuestTimezone = booking.GuestTimezone,
            MeetingLink = booking.MeetingLink,
            MeetingPassword = booking.MeetingPassword,
            CalendarLink = booking.CalendarLink,
            LocationType = eventType.Location.Type.ToString(),
            LocationDetails = eventType.Location.Details,
            LocationDisplayName = eventType.Location.DisplayName,
            CancellationLink = cancellationLink,
            RescheduleLink = rescheduleLink
        });
    }
}
