# ScheduleKit - Enterprise-Grade Scheduling Module

## Executive Summary

**Project Name:** ScheduleKit
**Repository:** `github.com/dotnetdeveloper20xx/schedulekit`
**License:** MIT

**Elevator Pitch:** A production-ready, embeddable scheduling module for ASP.NET Core applications. Think "Calendly as a library" - add appointment booking to any .NET application with a single NuGet package. Features real-time updates, timezone intelligence, and a polished React UI.

**Why This Project Stands Out:**
- Solves a real business problem (appointment scheduling is a $350M+ market)
- Demonstrates enterprise patterns interviewers ask about (CQRS, MediatR, Clean Architecture)
- Shows full-stack proficiency with modern tech stacks
- Production-ready code quality with comprehensive testing
- Immediately runnable demo that "just works"

---

## Technical Architecture

### Architecture Pattern: Clean Architecture + CQRS

```
┌─────────────────────────────────────────────────────────────┐
│                      Presentation Layer                      │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │   Minimal APIs  │  │   SignalR Hub   │  │  React SPA  │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                     Application Layer                        │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │    Commands     │  │     Queries     │  │  Validators │ │
│  │   (MediatR)     │  │   (MediatR)     │  │ (FluentVal) │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
│  ┌─────────────────┐  ┌─────────────────┐                   │
│  │   Behaviors     │  │    Mappers      │                   │
│  │  (Pipeline)     │  │  (Mapperly)     │                   │
│  └─────────────────┘  └─────────────────┘                   │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                       Domain Layer                           │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │    Entities     │  │ Domain Events   │  │   Services  │ │
│  │  (Rich Models)  │  │                 │  │(Pure Logic) │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
│  ┌─────────────────┐  ┌─────────────────┐                   │
│  │  Value Objects  │  │ Specifications  │                   │
│  │                 │  │                 │                   │
│  └─────────────────┘  └─────────────────┘                   │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                       │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │   EF Core +     │  │     Caching     │  │    Email    │ │
│  │   SQL Server    │  │  (IMemoryCache) │  │  (SendGrid) │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
│  ┌─────────────────┐  ┌─────────────────┐                   │
│  │  Background     │  │   External      │                   │
│  │   Jobs          │  │   Calendar API  │                   │
│  └─────────────────┘  └─────────────────┘                   │
└─────────────────────────────────────────────────────────────┘
```

### Technology Decisions

| Category | Choice | Why (Interview Answer) |
|----------|--------|------------------------|
| **Backend** | ASP.NET Core 8 | LTS, minimal APIs, native AOT ready |
| **Architecture** | Clean Architecture | Testable, maintainable, dependency rule enforcement |
| **CQRS** | MediatR | Separates read/write concerns, enables pipeline behaviors |
| **Database** | SQL Server + EF Core | Industry standard, LINQ, migrations |
| **Caching** | IMemoryCache + Output Caching | Reduces DB load on slot queries |
| **Real-time** | SignalR | Live booking updates, no polling |
| **Validation** | FluentValidation + MediatR Pipeline | Centralized, testable validation |
| **Mapping** | Mapperly | Source-generated, zero reflection overhead |
| **API Docs** | Swagger/OpenAPI | Self-documenting, client generation |
| **Logging** | Serilog + Structured Logging | Queryable logs, context enrichment |
| **Frontend** | React 18 + TypeScript | Type safety, hooks, ecosystem |
| **State** | TanStack Query | Server state, caching, optimistic updates |
| **Styling** | Tailwind CSS | Utility-first, consistent design |
| **Testing** | xUnit + FluentAssertions + Testcontainers | Real DB tests, readable assertions |

---

## Core Features (What Users Actually Want)

### Phase 1: MVP Features

| Feature | Host Value | Guest Value |
|---------|-----------|-------------|
| **Event Types** | Create multiple meeting types (15min call, 1hr consultation) | Clear options to choose from |
| **Weekly Availability** | Set working hours once, applies automatically | See real available times |
| **Date Overrides** | Block vacation days, add extra availability | No failed bookings |
| **Timezone Intelligence** | Work in your timezone, serve globally | Times shown in guest's timezone |
| **Instant Booking** | No back-and-forth emails | Book in under 30 seconds |
| **Email Confirmations** | Automatic notifications | Booking proof in inbox |
| **Booking Management** | Dashboard of all appointments | Confirmation page with details |
| **Cancellation** | Cancel with one click + reason | Clear cancellation policy |

### Phase 2: Competitive Features (What Makes Users Stay)

| Feature | Why It Matters |
|---------|----------------|
| **Google Calendar Sync** | 70% of professionals use Google Calendar - if we don't sync, they'll use Calendly |
| **Reminder Emails** | 24hr and 1hr reminders reduce no-shows by 30% |
| **Guest Reschedule Link** | Self-service reduces host workload |
| **Meeting Locations** | Zoom link, Google Meet, phone, or custom location |
| **Booking Questions** | Collect context before meetings (required for consultants) |
| **Buffer Times** | Prevent back-to-back meeting burnout |
| **Minimum Notice** | Prevent same-day bookings (24hr minimum) |
| **Booking Limits** | Max 5 meetings per day prevents overload |

### Phase 3: Premium Features (What Makes It Impressive)

| Feature | Technical Showcase |
|---------|-------------------|
| **Real-time Slot Updates** | SignalR - when someone books, others see it disappear instantly |
| **Embeddable Widget** | JavaScript SDK that any website can embed |
| **Analytics Dashboard** | Booking trends, popular times, conversion rates |
| **Team Scheduling** | Multiple hosts, round-robin assignment |
| **Recurring Meetings** | Weekly 1:1s that auto-book |

---

## Domain Model (Rich Domain Design)

### Aggregate: EventType

```csharp
public class EventType : BaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid HostUserId { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public Duration Duration { get; private set; }              // Value Object
    public BufferTime BufferBefore { get; private set; }        // Value Object
    public BufferTime BufferAfter { get; private set; }         // Value Object
    public MinimumNotice MinimumNotice { get; private set; }    // Value Object (e.g., 24 hours)
    public BookingWindow BookingWindow { get; private set; }    // Value Object (e.g., 60 days ahead)
    public MeetingLocation Location { get; private set; }       // Value Object (Zoom/Meet/Phone/Custom)
    public bool IsActive { get; private set; }
    public string? Color { get; private set; }

    private readonly List<BookingQuestion> _questions = new();
    public IReadOnlyCollection<BookingQuestion> Questions => _questions.AsReadOnly();

    // Domain methods - encapsulate business rules
    public Result Activate() { ... }
    public Result Deactivate() { ... }
    public Result UpdateDetails(string name, string? description) { ... }
    public Result AddQuestion(BookingQuestion question) { ... }
    public Result RemoveQuestion(Guid questionId) { ... }
    public Result ReorderQuestions(IEnumerable<Guid> orderedIds) { ... }

    // Factory method - ensures valid state
    public static Result<EventType> Create(
        Guid hostUserId,
        string name,
        int durationMinutes,
        MeetingLocation location)
    {
        // Validation logic
        // Returns Result.Failure if invalid
        // Returns Result.Success(eventType) if valid
    }
}
```

### Value Objects (Interview Gold)

```csharp
// Demonstrates value object pattern - immutable, equality by value
public record Duration
{
    public int Minutes { get; }

    private Duration(int minutes) => Minutes = minutes;

    public static Result<Duration> Create(int minutes)
    {
        if (minutes < 15) return Result.Failure<Duration>("Duration must be at least 15 minutes");
        if (minutes > 480) return Result.Failure<Duration>("Duration cannot exceed 8 hours");
        if (minutes % 5 != 0) return Result.Failure<Duration>("Duration must be in 5-minute increments");

        return Result.Success(new Duration(minutes));
    }

    public static Duration FromMinutes(int minutes) => Create(minutes).Value;

    // Predefined durations
    public static Duration FifteenMinutes => new(15);
    public static Duration ThirtyMinutes => new(30);
    public static Duration OneHour => new(60);
}

public record TimeSlot
{
    public DateOnly Date { get; }
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }
    public string Timezone { get; }

    public TimeSlot ToTimezone(string targetTimezone) { ... }
    public bool OverlapsWith(TimeSlot other) { ... }
    public bool Contains(DateTime utcTime) { ... }
}

public record MeetingLocation
{
    public LocationType Type { get; }
    public string? Details { get; }  // Zoom link, phone number, address

    public static MeetingLocation Zoom(string meetingLink) => ...
    public static MeetingLocation GoogleMeet() => ...  // Auto-generate
    public static MeetingLocation Phone(string number) => ...
    public static MeetingLocation InPerson(string address) => ...
    public static MeetingLocation Custom(string instructions) => ...
}
```

### Aggregate: Booking

```csharp
public class Booking : BaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid EventTypeId { get; private set; }
    public Guid HostUserId { get; private set; }
    public GuestInfo Guest { get; private set; }                // Value Object
    public TimeSlot ScheduledTime { get; private set; }         // Value Object
    public BookingStatus Status { get; private set; }
    public string? CancellationReason { get; private set; }
    public string? RescheduleToken { get; private set; }        // For guest self-service
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    private readonly List<QuestionResponse> _responses = new();
    public IReadOnlyCollection<QuestionResponse> Responses => _responses.AsReadOnly();

    // Domain events (MediatR notifications)
    public Result Cancel(string reason, CancelledBy cancelledBy)
    {
        if (Status == BookingStatus.Cancelled)
            return Result.Failure("Booking is already cancelled");

        if (ScheduledTime.StartTime < DateTime.UtcNow)
            return Result.Failure("Cannot cancel past bookings");

        Status = BookingStatus.Cancelled;
        CancellationReason = reason;
        CancelledAtUtc = DateTime.UtcNow;

        AddDomainEvent(new BookingCancelledEvent(this, cancelledBy));
        return Result.Success();
    }

    public Result<string> GenerateRescheduleToken()
    {
        RescheduleToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        AddDomainEvent(new RescheduleTokenGeneratedEvent(this));
        return Result.Success(RescheduleToken);
    }
}

public record GuestInfo
{
    public string Name { get; }
    public Email Email { get; }           // Value object with validation
    public string? Phone { get; }
    public string Timezone { get; }       // IANA timezone ID
}
```

### Domain Events (Shows Event-Driven Understanding)

```csharp
// Domain events - handled by MediatR notification handlers
public record BookingCreatedEvent(Booking Booking) : IDomainEvent;
public record BookingCancelledEvent(Booking Booking, CancelledBy CancelledBy) : IDomainEvent;
public record BookingRescheduledEvent(Booking Booking, TimeSlot OldTime, TimeSlot NewTime) : IDomainEvent;
public record ReminderDueEvent(Booking Booking, ReminderType Type) : IDomainEvent;

// Handlers - decoupled side effects
public class BookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
{
    public async Task Handle(BookingCreatedEvent notification, CancellationToken ct)
    {
        // Send confirmation email to guest
        // Send notification email to host
        // Sync to Google Calendar
        // Broadcast via SignalR
    }
}
```

---

## CQRS Implementation (Interview Showcase)

### Command Example: CreateBooking

```csharp
// Command - represents intent
public record CreateBookingCommand : IRequest<Result<BookingResponse>>
{
    public Guid EventTypeId { get; init; }
    public string GuestName { get; init; }
    public string GuestEmail { get; init; }
    public string? GuestPhone { get; init; }
    public DateTime RequestedStartTimeUtc { get; init; }
    public string GuestTimezone { get; init; }
    public Dictionary<Guid, string> QuestionResponses { get; init; } = new();
}

// Validator - runs in MediatR pipeline before handler
public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.GuestName)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);

        RuleFor(x => x.GuestEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.RequestedStartTimeUtc)
            .GreaterThan(DateTime.UtcNow.AddHours(1))
            .WithMessage("Bookings must be at least 1 hour in advance");

        RuleFor(x => x.GuestTimezone)
            .Must(BeValidTimezone)
            .WithMessage("Invalid timezone");
    }

    private bool BeValidTimezone(string timezone)
        => TimeZoneInfo.TryFindSystemTimeZoneById(timezone, out _);
}

// Handler - orchestrates the use case
public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<BookingResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IEventTypeRepository _eventTypeRepository;
    private readonly ISlotCalculator _slotCalculator;
    private readonly IUnitOfWork _unitOfWork;

    public async Task<Result<BookingResponse>> Handle(
        CreateBookingCommand request,
        CancellationToken ct)
    {
        // 1. Load aggregate
        var eventType = await _eventTypeRepository.GetByIdAsync(request.EventTypeId, ct);
        if (eventType is null)
            return Result.Failure<BookingResponse>("Event type not found");

        // 2. Check slot availability (uses specification pattern)
        var isAvailable = await _slotCalculator.IsSlotAvailableAsync(
            eventType,
            request.RequestedStartTimeUtc,
            ct);

        if (!isAvailable)
            return Result.Failure<BookingResponse>("This time slot is no longer available");

        // 3. Create booking via factory method
        var bookingResult = Booking.Create(
            eventType,
            new GuestInfo(request.GuestName, Email.Create(request.GuestEmail).Value, request.GuestPhone, request.GuestTimezone),
            TimeSlot.FromUtc(request.RequestedStartTimeUtc, eventType.Duration, request.GuestTimezone));

        if (bookingResult.IsFailure)
            return Result.Failure<BookingResponse>(bookingResult.Error);

        var booking = bookingResult.Value;

        // 4. Add question responses
        foreach (var (questionId, response) in request.QuestionResponses)
        {
            booking.AddResponse(questionId, response);
        }

        // 5. Persist (domain events dispatched on SaveChanges)
        await _bookingRepository.AddAsync(booking, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // 6. Return response
        return Result.Success(booking.ToResponse());
    }
}
```

### Query Example: GetAvailableSlots

```csharp
// Query - read-only, can use different data source/optimization
public record GetAvailableSlotsQuery : IRequest<Result<AvailableSlotsResponse>>
{
    public Guid EventTypeId { get; init; }
    public DateOnly Date { get; init; }
    public string GuestTimezone { get; init; }
}

// Handler - optimized for reads
public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, Result<AvailableSlotsResponse>>
{
    private readonly ISlotCalculator _slotCalculator;
    private readonly ICacheService _cache;

    public async Task<Result<AvailableSlotsResponse>> Handle(
        GetAvailableSlotsQuery request,
        CancellationToken ct)
    {
        // Cache key includes date and event type - invalidated on booking
        var cacheKey = $"slots:{request.EventTypeId}:{request.Date}";

        var slots = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            return await _slotCalculator.CalculateAvailableSlotsAsync(
                request.EventTypeId,
                request.Date,
                ct);
        }, TimeSpan.FromMinutes(5));

        // Convert to guest timezone for display
        var convertedSlots = slots
            .Select(s => s.ToTimezone(request.GuestTimezone))
            .ToList();

        return Result.Success(new AvailableSlotsResponse
        {
            Date = request.Date,
            Timezone = request.GuestTimezone,
            Slots = convertedSlots
        });
    }
}
```

### MediatR Pipeline Behaviors (Cross-Cutting Concerns)

```csharp
// Validation behavior - runs before every handler
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}

// Logging behavior - automatic request/response logging
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public async Task<TResponse> Handle(...)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        _logger.LogInformation(
            "Handled {RequestName} in {ElapsedMs}ms",
            requestName,
            stopwatch.ElapsedMilliseconds);

        return response;
    }
}

// Unit of work behavior - auto-save for commands
public class UnitOfWorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public async Task<TResponse> Handle(...)
    {
        var response = await next();
        await _unitOfWork.SaveChangesAsync(ct);
        return response;
    }
}
```

---

## API Design (REST Best Practices)

### Endpoint Structure

```
/api/v1/
├── event-types/                    # Host manages event types
│   ├── GET                         # List my event types
│   ├── POST                        # Create event type
│   ├── {id}/
│   │   ├── GET                     # Get event type details
│   │   ├── PUT                     # Update event type
│   │   ├── DELETE                  # Delete event type
│   │   └── questions/              # Nested resource
│   │       ├── GET                 # List questions
│   │       ├── POST                # Add question
│   │       └── {questionId}/
│   │           ├── PUT             # Update question
│   │           └── DELETE          # Remove question
│
├── availability/                   # Host manages availability
│   ├── GET                         # Get weekly schedule
│   ├── PUT                         # Update weekly schedule (bulk)
│   └── overrides/
│       ├── GET                     # List date overrides
│       ├── POST                    # Create override
│       └── {id}/DELETE             # Delete override
│
├── bookings/                       # Host manages bookings
│   ├── GET                         # List bookings (paginated, filtered)
│   ├── {id}/
│   │   ├── GET                     # Get booking details
│   │   └── cancel/POST             # Cancel booking
│
├── public/                         # Guest-facing (no auth)
│   ├── {hostSlug}/
│   │   ├── GET                     # Get host's public profile
│   │   └── {eventSlug}/
│   │       ├── GET                 # Get event type details
│   │       └── slots/GET           # Get available slots (?date=&timezone=)
│   └── bookings/
│       ├── POST                    # Create booking
│       └── {id}/
│           ├── GET                 # View booking (with token)
│           ├── cancel/POST         # Guest cancel (with token)
│           └── reschedule/POST     # Guest reschedule (with token)
│
└── webhooks/                       # For integrating apps
    └── POST                        # Receive calendar sync events
```

### Response Envelope (Consistent API)

```csharp
// All responses follow this structure
public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public ApiError? Error { get; init; }
    public Dictionary<string, object>? Meta { get; init; }  // Pagination, etc.
}

public class ApiError
{
    public string Code { get; init; }           // "VALIDATION_ERROR", "NOT_FOUND"
    public string Message { get; init; }        // Human-readable
    public List<FieldError>? Details { get; init; }  // Field-level errors
}

// Success: 200 OK
{
    "success": true,
    "data": {
        "id": "...",
        "name": "30-minute Consultation",
        ...
    }
}

// Paginated: 200 OK
{
    "success": true,
    "data": [...],
    "meta": {
        "page": 1,
        "pageSize": 20,
        "totalCount": 150,
        "totalPages": 8
    }
}

// Validation error: 400 Bad Request
{
    "success": false,
    "error": {
        "code": "VALIDATION_ERROR",
        "message": "One or more validation errors occurred",
        "details": [
            { "field": "guestEmail", "message": "Invalid email format" },
            { "field": "requestedTime", "message": "Time slot is no longer available" }
        ]
    }
}

// Not found: 404 Not Found
{
    "success": false,
    "error": {
        "code": "NOT_FOUND",
        "message": "Event type not found"
    }
}
```

### Idempotency (Production Requirement)

```csharp
// Prevent duplicate bookings from network retries
[HttpPost]
public async Task<IActionResult> CreateBooking(
    [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
    CreateBookingRequest request)
{
    if (idempotencyKey != null)
    {
        var cached = await _idempotencyService.GetAsync(idempotencyKey);
        if (cached != null)
            return Ok(cached);  // Return same response
    }

    var result = await _mediator.Send(request.ToCommand());

    if (idempotencyKey != null && result.IsSuccess)
    {
        await _idempotencyService.SetAsync(idempotencyKey, result.Value, TimeSpan.FromHours(24));
    }

    return result.ToActionResult();
}
```

---

## Real-Time Features (SignalR)

### Hub Design

```csharp
public interface IScheduleKitClient
{
    Task SlotBooked(Guid eventTypeId, DateTime startTimeUtc);
    Task SlotReleased(Guid eventTypeId, DateTime startTimeUtc);  // On cancellation
    Task BookingCreated(BookingNotification booking);            // For host dashboard
    Task BookingCancelled(Guid bookingId);
}

public class ScheduleKitHub : Hub<IScheduleKitClient>
{
    // Guests join event type group to see real-time availability
    public async Task JoinEventTypeGroup(Guid eventTypeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"event:{eventTypeId}");
    }

    // Hosts join their personal group for dashboard updates
    public async Task JoinHostDashboard(Guid hostUserId)
    {
        // Verify user is authenticated and matches hostUserId
        await Groups.AddToGroupAsync(Context.ConnectionId, $"host:{hostUserId}");
    }
}

// Event handler broadcasts when booking is created
public class BookingCreatedSignalRHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly IHubContext<ScheduleKitHub, IScheduleKitClient> _hub;

    public async Task Handle(BookingCreatedEvent notification, CancellationToken ct)
    {
        var booking = notification.Booking;

        // Tell other guests this slot is taken
        await _hub.Clients
            .Group($"event:{booking.EventTypeId}")
            .SlotBooked(booking.EventTypeId, booking.ScheduledTime.StartTimeUtc);

        // Tell host they have a new booking
        await _hub.Clients
            .Group($"host:{booking.HostUserId}")
            .BookingCreated(booking.ToNotification());
    }
}
```

### React Integration

```typescript
// hooks/useRealtimeSlots.ts
export function useRealtimeSlots(eventTypeId: string) {
    const queryClient = useQueryClient();

    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl('/hubs/schedulekit')
            .withAutomaticReconnect()
            .build();

        connection.on('SlotBooked', (eventId: string, startTimeUtc: string) => {
            if (eventId === eventTypeId) {
                // Invalidate cache - triggers refetch
                queryClient.invalidateQueries(['slots', eventTypeId]);

                // Or optimistically remove the slot
                queryClient.setQueryData(['slots', eventTypeId], (old: Slot[]) =>
                    old?.filter(slot => slot.startTimeUtc !== startTimeUtc)
                );
            }
        });

        connection.start().then(() => {
            connection.invoke('JoinEventTypeGroup', eventTypeId);
        });

        return () => { connection.stop(); };
    }, [eventTypeId]);
}
```

---

## Frontend Architecture (Modern React)

### Project Structure

```
ui/schedulekit-ui/
├── src/
│   ├── api/
│   │   ├── client.ts              # Axios instance with interceptors
│   │   ├── types.ts               # Generated from OpenAPI
│   │   └── hooks/                 # TanStack Query hooks
│   │       ├── useEventTypes.ts
│   │       ├── useAvailability.ts
│   │       ├── useBookings.ts
│   │       └── useSlots.ts
│   │
│   ├── components/
│   │   ├── ui/                    # Primitive components (Button, Input, Modal)
│   │   ├── calendar/              # Calendar components
│   │   │   ├── WeekView.tsx
│   │   │   ├── MonthView.tsx
│   │   │   ├── TimeSlotPicker.tsx
│   │   │   └── AvailabilityEditor.tsx
│   │   └── layout/
│   │       ├── Sidebar.tsx
│   │       ├── Header.tsx
│   │       └── PageContainer.tsx
│   │
│   ├── features/
│   │   ├── event-types/
│   │   │   ├── EventTypeList.tsx
│   │   │   ├── EventTypeForm.tsx
│   │   │   └── EventTypeCard.tsx
│   │   ├── availability/
│   │   │   ├── WeeklySchedule.tsx
│   │   │   ├── DateOverrides.tsx
│   │   │   └── TimezoneSelect.tsx
│   │   ├── bookings/
│   │   │   ├── BookingList.tsx
│   │   │   ├── BookingDetail.tsx
│   │   │   └── BookingCalendar.tsx
│   │   └── public-booking/        # Guest-facing
│   │       ├── EventTypePage.tsx
│   │       ├── DatePicker.tsx
│   │       ├── TimeSlotGrid.tsx
│   │       ├── BookingForm.tsx
│   │       └── Confirmation.tsx
│   │
│   ├── hooks/
│   │   ├── useRealtimeSlots.ts    # SignalR integration
│   │   ├── useTimezone.ts
│   │   └── useMediaQuery.ts
│   │
│   ├── lib/
│   │   ├── timezone.ts            # Timezone utilities
│   │   ├── date.ts                # date-fns helpers
│   │   └── validation.ts          # Zod schemas
│   │
│   ├── stores/
│   │   └── uiStore.ts             # Zustand for UI state (sidebar, theme)
│   │
│   ├── App.tsx
│   ├── routes.tsx
│   └── main.tsx
│
├── index.html
├── package.json
├── tailwind.config.js
├── tsconfig.json
└── vite.config.ts
```

### Key React Patterns

```typescript
// TanStack Query with optimistic updates
export function useCreateBooking() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (data: CreateBookingRequest) => api.createBooking(data),

        // Optimistic update - show slot as taken immediately
        onMutate: async (newBooking) => {
            await queryClient.cancelQueries(['slots', newBooking.eventTypeId]);

            const previousSlots = queryClient.getQueryData(['slots', newBooking.eventTypeId]);

            queryClient.setQueryData(['slots', newBooking.eventTypeId], (old: Slot[]) =>
                old?.filter(slot => slot.startTime !== newBooking.requestedStartTime)
            );

            return { previousSlots };
        },

        // Rollback on error
        onError: (err, newBooking, context) => {
            queryClient.setQueryData(
                ['slots', newBooking.eventTypeId],
                context?.previousSlots
            );
        },

        // Refetch to ensure consistency
        onSettled: (data, error, variables) => {
            queryClient.invalidateQueries(['slots', variables.eventTypeId]);
        },
    });
}

// Form with Zod validation
const bookingSchema = z.object({
    guestName: z.string().min(1, 'Name is required').max(100),
    guestEmail: z.string().email('Invalid email'),
    guestPhone: z.string().optional(),
    selectedSlot: z.string().min(1, 'Please select a time'),
});

export function BookingForm({ eventType, selectedDate }: Props) {
    const form = useForm<z.infer<typeof bookingSchema>>({
        resolver: zodResolver(bookingSchema),
    });

    const createBooking = useCreateBooking();

    const onSubmit = (data: z.infer<typeof bookingSchema>) => {
        createBooking.mutate({
            eventTypeId: eventType.id,
            ...data,
        });
    };

    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)}>
                {/* Form fields */}
            </form>
        </Form>
    );
}
```

### Accessibility (a11y) Requirements

```typescript
// TimeSlotGrid with keyboard navigation
export function TimeSlotGrid({ slots, selectedSlot, onSelect }: Props) {
    const [focusedIndex, setFocusedIndex] = useState(0);

    const handleKeyDown = (e: KeyboardEvent) => {
        switch (e.key) {
            case 'ArrowRight':
                setFocusedIndex(i => Math.min(i + 1, slots.length - 1));
                break;
            case 'ArrowLeft':
                setFocusedIndex(i => Math.max(i - 1, 0));
                break;
            case 'Enter':
            case ' ':
                onSelect(slots[focusedIndex]);
                break;
        }
    };

    return (
        <div
            role="listbox"
            aria-label="Available time slots"
            onKeyDown={handleKeyDown}
        >
            {slots.map((slot, index) => (
                <button
                    key={slot.startTime}
                    role="option"
                    aria-selected={selectedSlot?.startTime === slot.startTime}
                    tabIndex={focusedIndex === index ? 0 : -1}
                    onClick={() => onSelect(slot)}
                    className={cn(
                        'slot-button',
                        selectedSlot?.startTime === slot.startTime && 'selected'
                    )}
                >
                    {format(new Date(slot.startTime), 'h:mm a')}
                </button>
            ))}
        </div>
    );
}
```

---

## Testing Strategy (Portfolio Differentiator)

### Test Pyramid

```
                    ┌─────────┐
                    │   E2E   │  ← Playwright (critical paths)
                   ┌┴─────────┴┐
                   │Integration│  ← Testcontainers + real DB
                  ┌┴───────────┴┐
                  │    Unit     │  ← Business logic, algorithms
                 └──────────────┘
```

### Unit Tests (SlotCalculator Example)

```csharp
public class SlotCalculatorTests
{
    private readonly SlotCalculator _sut;

    [Fact]
    public void CalculateSlots_ReturnsCorrectSlots_ForStandardWorkday()
    {
        // Arrange
        var availability = new WeeklyAvailability()
            .SetDay(DayOfWeek.Monday, TimeOnly.Parse("09:00"), TimeOnly.Parse("17:00"));
        var eventType = EventTypeBuilder.Create()
            .WithDuration(30)
            .Build();
        var date = new DateOnly(2024, 1, 15); // A Monday

        // Act
        var slots = _sut.CalculateSlots(availability, eventType, date, existingBookings: []);

        // Assert
        slots.Should().HaveCount(16); // 8 hours / 30 min = 16 slots
        slots.First().StartTime.Should().Be(TimeOnly.Parse("09:00"));
        slots.Last().StartTime.Should().Be(TimeOnly.Parse("16:30"));
    }

    [Fact]
    public void CalculateSlots_ExcludesBookedSlots()
    {
        // Arrange
        var existingBooking = BookingBuilder.Create()
            .At(new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc))
            .WithDuration(30)
            .Build();

        // Act
        var slots = _sut.CalculateSlots(availability, eventType, date, [existingBooking]);

        // Assert
        slots.Should().NotContain(s => s.StartTime == TimeOnly.Parse("10:00"));
    }

    [Fact]
    public void CalculateSlots_RespectsBufferTimes()
    {
        // Arrange
        var eventType = EventTypeBuilder.Create()
            .WithDuration(30)
            .WithBufferBefore(15)
            .WithBufferAfter(15)
            .Build();
        var existingBooking = BookingBuilder.Create()
            .At(new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc))
            .Build();

        // Act
        var slots = _sut.CalculateSlots(availability, eventType, date, [existingBooking]);

        // Assert
        // 10:00 booking with 15min buffers blocks 9:30-10:45
        slots.Should().NotContain(s => s.StartTime == TimeOnly.Parse("09:30"));
        slots.Should().NotContain(s => s.StartTime == TimeOnly.Parse("10:00"));
        slots.Should().Contain(s => s.StartTime == TimeOnly.Parse("10:30")); // 10:30 start + 30min = 11:00 end, after 10:45 buffer
    }

    [Theory]
    [InlineData("America/New_York", "09:00", "14:00")]  // EST is UTC-5
    [InlineData("Europe/London", "09:00", "09:00")]     // Same as UTC in winter
    [InlineData("Asia/Tokyo", "09:00", "00:00")]        // JST is UTC+9
    public void CalculateSlots_ConvertsTimezoneCorrectly(
        string hostTimezone,
        string hostLocalTime,
        string expectedUtcTime)
    {
        // Test timezone conversion logic
    }
}
```

### Integration Tests (Testcontainers)

```csharp
public class BookingEndpointTests : IClassFixture<ScheduleKitWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ScheduleKitDbContext _dbContext;

    [Fact]
    public async Task CreateBooking_ReturnsConflict_WhenSlotAlreadyBooked()
    {
        // Arrange
        var eventType = await SeedEventTypeAsync();
        var existingBooking = await SeedBookingAsync(eventType.Id, DateTime.UtcNow.AddDays(1).Date.AddHours(10));

        var request = new CreateBookingRequest
        {
            EventTypeId = eventType.Id,
            GuestName = "Jane Doe",
            GuestEmail = "jane@example.com",
            RequestedStartTimeUtc = existingBooking.StartTimeUtc, // Same time!
            GuestTimezone = "America/New_York"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/public/bookings", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        body.Error.Code.Should().Be("SLOT_UNAVAILABLE");
    }

    [Fact]
    public async Task CreateBooking_Success_SendsConfirmationEmail()
    {
        // Arrange
        var eventType = await SeedEventTypeAsync();
        var emailService = Services.GetRequiredService<Mock<IEmailService>>();

        var request = new CreateBookingRequest { ... };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/public/bookings", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        emailService.Verify(
            x => x.SendBookingConfirmationAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

// WebApplicationFactory with Testcontainers
public class ScheduleKitWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace SQL Server connection with container
            services.RemoveAll<DbContextOptions<ScheduleKitDbContext>>();
            services.AddDbContext<ScheduleKitDbContext>(options =>
                options.UseSqlServer(_sqlContainer.GetConnectionString()));
        });
    }
}
```

### E2E Tests (Playwright)

```typescript
// tests/booking-flow.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Booking Flow', () => {
    test('guest can complete booking successfully', async ({ page }) => {
        // Navigate to public booking page
        await page.goto('/book/demo-host/30-min-consultation');

        // Verify event type details shown
        await expect(page.getByRole('heading', { name: '30-minute Consultation' })).toBeVisible();

        // Select a date (first available)
        const availableDate = page.locator('[data-available="true"]').first();
        await availableDate.click();

        // Select a time slot
        const timeSlot = page.getByRole('option').first();
        await timeSlot.click();
        await expect(timeSlot).toHaveAttribute('aria-selected', 'true');

        // Fill in guest details
        await page.getByLabel('Name').fill('Test Guest');
        await page.getByLabel('Email').fill('guest@example.com');

        // Submit booking
        await page.getByRole('button', { name: 'Confirm Booking' }).click();

        // Verify confirmation page
        await expect(page).toHaveURL(/\/booking\/.*\/confirmed/);
        await expect(page.getByText('Booking Confirmed')).toBeVisible();
        await expect(page.getByText('Test Guest')).toBeVisible();
        await expect(page.getByText('guest@example.com')).toBeVisible();
    });

    test('shows error when slot becomes unavailable', async ({ page, context }) => {
        await page.goto('/book/demo-host/30-min-consultation');

        // Select date and time
        await page.locator('[data-available="true"]').first().click();
        const selectedSlot = page.getByRole('option').first();
        const slotTime = await selectedSlot.textContent();
        await selectedSlot.click();

        // Simulate another user booking the same slot (via API)
        await context.request.post('/api/v1/public/bookings', {
            data: { /* same time slot */ }
        });

        // Try to submit - should fail gracefully
        await page.getByLabel('Name').fill('Late Booker');
        await page.getByLabel('Email').fill('late@example.com');
        await page.getByRole('button', { name: 'Confirm Booking' }).click();

        // Should show helpful error
        await expect(page.getByText('This time slot is no longer available')).toBeVisible();
        await expect(page.getByText('Please select another time')).toBeVisible();
    });
});
```

---

## Repository Structure

```
schedulekit/
├── README.md
├── LICENSE
├── ScheduleKit.sln
├── .github/
│   └── workflows/
│       ├── ci.yml                          # Build + test on PR
│       └── release.yml                     # Publish to NuGet (future)
│
├── docs/
│   ├── getting-started.md
│   ├── integration-guide.md
│   ├── api-reference.md
│   └── architecture.md
│
├── src/
│   ├── ScheduleKit.Domain/                 # Core business logic (no dependencies)
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   ├── Specifications/
│   │   ├── Services/
│   │   └── Interfaces/
│   │
│   ├── ScheduleKit.Application/            # Use cases (MediatR handlers)
│   │   ├── Commands/
│   │   ├── Queries/
│   │   ├── Behaviors/
│   │   ├── Validators/
│   │   └── Mappers/
│   │
│   ├── ScheduleKit.Infrastructure/         # External concerns
│   │   ├── Data/
│   │   │   ├── ScheduleKitDbContext.cs
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── Interceptors/               # Audit, domain events
│   │   ├── Repositories/
│   │   ├── Services/
│   │   │   ├── EmailService.cs
│   │   │   ├── CacheService.cs
│   │   │   └── CalendarSyncService.cs
│   │   └── BackgroundJobs/
│   │       └── ReminderJob.cs
│   │
│   ├── ScheduleKit.Api/                    # HTTP layer
│   │   ├── Endpoints/
│   │   ├── Hubs/                           # SignalR
│   │   ├── Middleware/
│   │   └── Extensions/
│   │
│   └── ScheduleKit.Demo/                   # Standalone demo app
│       ├── Program.cs
│       ├── SeedData.cs
│       └── appsettings.json
│
├── ui/
│   └── schedulekit-ui/                     # React SPA
│
├── tests/
│   ├── ScheduleKit.Domain.Tests/
│   ├── ScheduleKit.Application.Tests/
│   ├── ScheduleKit.Api.Tests/              # Integration tests
│   └── ScheduleKit.E2E.Tests/              # Playwright
│
└── samples/
    └── AspNetCoreIntegration/              # Example of adding to existing app
```

---

## Implementation Phases

### Phase 1: Foundation (Sprint 1-2)

**Goal:** Empty shell that runs and proves the architecture works

**Backend:**
- Solution structure with all projects
- Domain entities with value objects
- EF Core DbContext with configurations
- MediatR setup with pipeline behaviors
- Basic CRUD for EventType (one full vertical slice)
- Health check endpoint
- Swagger/OpenAPI setup
- Structured logging with Serilog

**Frontend:**
- Vite + React + TypeScript setup
- TanStack Query configuration
- API client with type generation
- Layout components
- Event type list/create (connects to API)

**Testing:**
- Unit test project structure
- First SlotCalculator tests
- Integration test setup with Testcontainers

**Deliverable:** Can create event type via UI, persists to database

---

### Phase 2: Core Scheduling (Sprint 3-4)

**Goal:** Host can define availability, guests can see available slots

**Backend:**
- Availability entity and endpoints
- AvailabilityOverride entity and endpoints
- SlotCalculator service (the hard algorithm)
- Timezone handling
- Public slots endpoint with caching

**Frontend:**
- Weekly availability editor
- Date override management
- Public booking page (date selection)
- Time slot grid

**Testing:**
- Comprehensive SlotCalculator tests
- Timezone edge cases
- Integration tests for slots endpoint

**Deliverable:** Can set availability and see correct slots

---

### Phase 3: Booking Flow (Sprint 5-6)

**Goal:** Complete booking lifecycle

**Backend:**
- Booking entity with domain methods
- CreateBooking command with conflict detection
- Booking confirmation endpoint
- Cancel booking endpoint
- Domain events (BookingCreated, BookingCancelled)
- Email notification handlers

**Frontend:**
- Booking form with validation
- Confirmation page
- Host bookings list
- Cancel booking flow

**Testing:**
- Booking conflict tests
- Email sending verification
- Full booking flow integration tests

**Deliverable:** End-to-end booking works

---

### Phase 4: Real-Time & Polish (Sprint 7-8)

**Goal:** Professional, production-ready feel

**Backend:**
- SignalR hub for real-time updates
- Guest reschedule functionality
- Booking questions/responses
- Background job for reminders

**Frontend:**
- Real-time slot updates
- Optimistic mutations
- Loading skeletons
- Error boundaries
- Mobile responsiveness
- Dark mode

**Testing:**
- E2E tests with Playwright
- Performance testing for slot calculation

**Deliverable:** Impressive demo ready for portfolio

---

## Interview Talking Points

When presenting this project, highlight:

1. **"Why this architecture?"**
   - Clean Architecture for testability and flexibility
   - CQRS because reads and writes have different optimization needs
   - MediatR for decoupling and cross-cutting concerns

2. **"What was the hardest part?"**
   - Slot calculation with timezone conversion
   - Handling race conditions in booking (idempotency)
   - Real-time updates without over-fetching

3. **"How did you handle [X]?"**
   - Validation: FluentValidation in MediatR pipeline
   - Caching: Output caching + invalidation on booking
   - Testing: Testcontainers for real database tests

4. **"What would you add with more time?"**
   - Google Calendar two-way sync
   - Team/round-robin scheduling
   - Payment integration for paid consultations
   - Multi-tenancy for SaaS deployment

5. **"Show me the code you're most proud of"**
   - SlotCalculator algorithm
   - Domain event handlers
   - Real-time SignalR integration
   - Optimistic UI updates

---

## Success Metrics

The project is portfolio-ready when:

- [ ] Clone and run in under 2 minutes
- [ ] Demo scenarios work flawlessly
- [ ] Code coverage > 80% on business logic
- [ ] No TypeScript/C# compiler warnings
- [ ] Lighthouse score > 90 on public booking page
- [ ] README has GIF showing the flow
- [ ] Can explain any code when asked
- [ ] Integration guide is tested and works
