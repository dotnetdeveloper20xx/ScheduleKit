# ScheduleKit - Developer Onboarding Guide

Welcome to ScheduleKit! This document will help you understand our application inside and out. You will be the main go-to person for this application.

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Solution Structure](#solution-structure)
4. [Getting Started](#getting-started)
5. [Key Concepts](#key-concepts)
6. [Domain Model](#domain-model)
7. [API Endpoints](#api-endpoints)
8. [Frontend Architecture](#frontend-architecture)
9. [Testing](#testing)
10. [Common Tasks](#common-tasks)
11. [Troubleshooting](#troubleshooting)

---

## Project Overview

**ScheduleKit** is a drop-in scheduling module for ASP.NET Core applications - think "Calendly as a library". It allows hosts to:
- Create event types (e.g., "30-minute consultation")
- Define their availability (weekly schedule + date overrides)
- Receive bookings from guests
- Manage their appointments

### Tech Stack

**Backend:**
- ASP.NET Core 8 with Minimal APIs
- Entity Framework Core (SQL Server + In-Memory)
- MediatR for CQRS pattern
- FluentValidation for request validation
- Serilog for structured logging

**Frontend:**
- React 18 with TypeScript
- Vite for build tooling
- TanStack Query for server state
- Tailwind CSS for styling
- React Hook Form + Zod for forms

---

## Architecture

We follow **Clean Architecture** with **CQRS** (Command Query Responsibility Segregation).

```
┌─────────────────────────────────────────────┐
│           Presentation Layer                │
│  (API Endpoints, SignalR Hubs, React SPA)   │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│           Application Layer                  │
│  (Commands, Queries, Validators, Mappers)   │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│             Domain Layer                     │
│  (Entities, Value Objects, Domain Events)   │
└─────────────────────────────────────────────┘
                      │
┌─────────────────────────────────────────────┐
│          Infrastructure Layer                │
│  (EF Core, Repositories, External Services) │
└─────────────────────────────────────────────┘
```

### Why This Architecture?

1. **Domain Layer** has no dependencies - pure business logic
2. **Application Layer** orchestrates use cases via MediatR
3. **Infrastructure Layer** implements interfaces defined in Domain
4. **Presentation Layer** is thin - just maps HTTP to commands/queries

---

## Solution Structure

```
ScheduleKit/
├── src/
│   ├── ScheduleKit.Domain/           # Core business logic (no dependencies)
│   │   ├── Entities/                 # Aggregate roots and entities
│   │   ├── ValueObjects/             # Immutable value types
│   │   ├── Events/                   # Domain events
│   │   ├── Interfaces/               # Repository interfaces
│   │   └── Services/                 # Domain services
│   │
│   ├── ScheduleKit.Application/      # Use cases
│   │   ├── Commands/                 # Write operations
│   │   ├── Queries/                  # Read operations
│   │   ├── Behaviors/                # MediatR pipeline behaviors
│   │   ├── Validators/               # FluentValidation validators
│   │   └── Common/                   # Shared DTOs, interfaces
│   │
│   ├── ScheduleKit.Infrastructure/   # External concerns
│   │   ├── Data/                     # EF Core DbContext, configs
│   │   ├── Repositories/             # Repository implementations
│   │   └── Services/                 # External service implementations
│   │
│   ├── ScheduleKit.Api/              # HTTP layer
│   │   ├── Endpoints/                # Minimal API endpoints
│   │   ├── Hubs/                     # SignalR hubs
│   │   └── Middleware/               # Custom middleware
│   │
│   └── ScheduleKit.Demo/             # Standalone demo application
│
├── ui/
│   └── schedulekit-ui/               # React frontend
│
├── tests/
│   ├── ScheduleKit.Domain.Tests/
│   ├── ScheduleKit.Application.Tests/
│   └── ScheduleKit.Api.Tests/
│
└── docs/                             # Documentation
```

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18+
- SQL Server (or use In-Memory for development)

### Running the Application

**Backend:**
```bash
cd src/ScheduleKit.Demo
dotnet run
```
API will be available at `https://localhost:5001`

**Frontend:**
```bash
cd ui/schedulekit-ui
npm install
npm run dev
```
UI will be available at `http://localhost:3000`

### Running Tests

```bash
# All tests
dotnet test

# Specific project
dotnet test tests/ScheduleKit.Domain.Tests
```

---

## Key Concepts

### CQRS with MediatR

We separate reads (Queries) from writes (Commands):

**Command Example:**
```csharp
// Command
public record CreateEventTypeCommand : IRequest<Result<EventTypeResponse>>
{
    public string Name { get; init; }
    public int DurationMinutes { get; init; }
}

// Handler
public class CreateEventTypeCommandHandler : IRequestHandler<CreateEventTypeCommand, Result<EventTypeResponse>>
{
    public async Task<Result<EventTypeResponse>> Handle(CreateEventTypeCommand request, CancellationToken ct)
    {
        // Business logic here
    }
}
```

**Query Example:**
```csharp
public record GetEventTypesQuery : IRequest<Result<List<EventTypeResponse>>>;
```

### Result Pattern

We use the Result pattern instead of exceptions for expected failures:

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Value { get; }
    public string Error { get; }
}
```

### Value Objects

Immutable types that encapsulate validation:

```csharp
public record Duration
{
    public int Minutes { get; }

    public static Result<Duration> Create(int minutes)
    {
        if (minutes < 15) return Result.Failure<Duration>("Too short");
        return Result.Success(new Duration(minutes));
    }
}
```

---

## Domain Model

### Core Entities

| Entity | Description |
|--------|-------------|
| **EventType** | A type of meeting (e.g., "30-min consultation") |
| **Availability** | Weekly availability rules (e.g., Mon-Fri 9am-5pm) |
| **AvailabilityOverride** | Date-specific exceptions (block a day, add extra hours) |
| **Booking** | A confirmed appointment |
| **BookingQuestion** | Custom questions for booking forms |

### Entity Relationships

```
EventType (1) ──── (*) Booking
    │
    └── (*) BookingQuestion

HostUser (1) ──── (*) Availability
    │
    └── (*) AvailabilityOverride
```

---

## API Endpoints

See `improvedPrompt.md` for full API reference. Key endpoints:

| Endpoint | Description |
|----------|-------------|
| `GET /api/v1/event-types` | List host's event types |
| `POST /api/v1/event-types` | Create event type |
| `GET /api/v1/public/{host}/{event}/slots` | Get available slots |
| `POST /api/v1/public/bookings` | Create booking |

---

## Frontend Architecture

### Folder Structure

```
src/
├── api/          # API client, types, hooks
├── components/   # Reusable UI components
├── features/     # Feature-based modules
├── hooks/        # Custom React hooks
├── lib/          # Utilities
└── stores/       # Zustand stores
```

### Data Fetching with TanStack Query

```typescript
// hooks/useEventTypes.ts
export function useEventTypes() {
    return useQuery({
        queryKey: ['eventTypes'],
        queryFn: () => api.getEventTypes(),
    });
}
```

---

## Testing

### Test Pyramid

1. **Unit Tests** - Domain logic, value objects, services
2. **Integration Tests** - API endpoints with real database (Testcontainers)
3. **E2E Tests** - Full user flows (Playwright)

### Writing Tests

```csharp
[Fact]
public void Duration_Create_RejectsInvalidMinutes()
{
    var result = Duration.Create(5);

    result.IsFailure.Should().BeTrue();
    result.Error.Should().Contain("15 minutes");
}
```

---

## Common Tasks

### Adding a New Entity

1. Create entity in `Domain/Entities/`
2. Add EF configuration in `Infrastructure/Data/Configurations/`
3. Add to `ScheduleKitDbContext`
4. Create migration: `dotnet ef migrations add AddNewEntity`
5. Create repository interface and implementation
6. Create commands/queries in Application layer
7. Add API endpoints

### Adding a New Endpoint

1. Create Command/Query in `Application/Commands/` or `Application/Queries/`
2. Create Validator if needed
3. Create Handler
4. Add endpoint in `Api/Endpoints/`
5. Add tests

---

## Troubleshooting

### Common Issues

**Database connection fails:**
- Check connection string in `appsettings.json`
- For In-Memory, ensure `UseInMemory: true`

**MediatR handler not found:**
- Ensure handler is in the correct assembly
- Check DI registration in `Program.cs`

**CORS errors:**
- Check CORS policy in `Program.cs`
- Ensure frontend URL is allowed

---

## Important Files Reference

| File | Purpose |
|------|---------|
| `improvedPrompt.md` | Main specification - consult for any questions |
| `ProjectSprint.md` | Sprint progress tracking |
| `DeveloperNotes.md` | Decision log and reasoning |
| `src/ScheduleKit.Demo/Program.cs` | Application entry point |
| `src/ScheduleKit.Domain/Entities/` | Core domain models |

---

*Last Updated: 2026-01-20*
