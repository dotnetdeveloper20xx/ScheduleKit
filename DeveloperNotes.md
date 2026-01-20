# ScheduleKit - Developer Notes

This document captures the thinking, decisions, actions, and achievements throughout the development of ScheduleKit. Use this to understand the "why" behind every decision.

---

## Table of Contents

1. [Project Initialization](#project-initialization)
2. [Sprint 1-2: Foundation](#sprint-1-2-foundation)
3. [Architecture Decisions](#architecture-decisions)
4. [Technical Decisions Log](#technical-decisions-log)
5. [Lessons Learned](#lessons-learned)

---

## Project Initialization

### Date: 2026-01-20

**Context:**
Starting the ScheduleKit project - a Calendly-like scheduling module for ASP.NET Core applications.

**What We Did:**
1. Created initial project specification (`improvedPrompt.md`)
2. Set up documentation tracking files:
   - `ProjectSprint.md` - Sprint progress
   - `Onboarding.md` - New developer guide
   - `DeveloperNotes.md` - This file

**Key Decisions Made:**

| Decision | Choice | Reasoning |
|----------|--------|-----------|
| Architecture | Clean Architecture + CQRS | Testable, maintainable, demonstrates interview-worthy patterns |
| CQRS Library | MediatR | Industry standard, pipeline behaviors for cross-cutting concerns |
| ORM | Entity Framework Core | .NET standard, excellent migrations, LINQ support |
| Database | SQL Server + In-Memory | In-Memory for dev speed, SQL Server for production realism |
| Frontend | React 18 + TypeScript | Type safety, modern patterns, wide adoption |
| State Management | TanStack Query | Server state caching, optimistic updates built-in |

**Why These Choices Matter for Portfolio:**
- Clean Architecture shows understanding of dependency management
- CQRS demonstrates understanding of read/write optimization
- MediatR shows knowledge of mediator pattern and pipeline processing
- Result pattern shows mature error handling (no exceptions for flow control)
- Value objects show DDD understanding

---

## Sprint 1-2: Foundation

### Goal
Create an empty shell that runs and proves the architecture works. Deliverable: Create event type via UI, persists to database.

---

### Session 1: Project Structure Setup

**Date:** 2026-01-20

**What We're Building:**
```
ScheduleKit.sln
├── src/
│   ├── ScheduleKit.Domain/         # No dependencies
│   ├── ScheduleKit.Application/    # Depends on Domain
│   ├── ScheduleKit.Infrastructure/ # Depends on Application, Domain
│   ├── ScheduleKit.Api/            # Depends on Application, Infrastructure
│   └── ScheduleKit.Demo/           # The runnable demo app
├── tests/
│   ├── ScheduleKit.Domain.Tests/
│   ├── ScheduleKit.Application.Tests/
│   └── ScheduleKit.Api.Tests/
└── ui/
    └── schedulekit-ui/             # React app
```

**Why This Structure:**
1. **Domain has no dependencies** - Can be tested in isolation, business logic is pure
2. **Application depends only on Domain** - Use cases don't know about database
3. **Infrastructure implements Domain interfaces** - Dependency Inversion Principle
4. **Api is thin** - Just maps HTTP to commands/queries
5. **Demo is separate** - Shows how to integrate ScheduleKit into any app

**Actions Taken:**
- [ ] Create solution file
- [ ] Create Domain project
- [ ] Create Application project
- [ ] Create Infrastructure project
- [ ] Create Api project
- [ ] Create Demo project
- [ ] Create test projects
- [ ] Set up project references

---

## Architecture Decisions

### ADR-001: Use CQRS with MediatR

**Status:** Accepted

**Context:**
Need to structure the application layer. Options considered:
1. Traditional service classes
2. CQRS with MediatR
3. Vertical slice architecture

**Decision:**
Use CQRS with MediatR.

**Reasoning:**
- Separates read and write concerns (different optimization needs)
- Pipeline behaviors handle cross-cutting concerns (validation, logging, transactions)
- Each handler is single-purpose and testable
- Interview talking point - commonly asked about

**Consequences:**
- More files (command + handler per operation)
- Learning curve for new developers
- BUT: Very testable and maintainable

---

### ADR-002: Result Pattern Over Exceptions

**Status:** Accepted

**Context:**
How to handle expected failures (validation errors, not found, conflicts)?

**Decision:**
Use Result<T> pattern for expected failures. Reserve exceptions for unexpected errors.

**Reasoning:**
- Exceptions are expensive and shouldn't be used for flow control
- Result pattern makes failure a first-class citizen
- Caller must handle both success and failure
- Shows mature error handling in interviews

**Code Example:**
```csharp
// Instead of:
public EventType GetById(Guid id)
{
    var entity = _db.Find(id);
    if (entity == null) throw new NotFoundException();
    return entity;
}

// We do:
public Result<EventType> GetById(Guid id)
{
    var entity = _db.Find(id);
    if (entity == null) return Result.Failure<EventType>("Not found");
    return Result.Success(entity);
}
```

---

### ADR-003: Value Objects for Domain Concepts

**Status:** Accepted

**Context:**
How to represent domain concepts like Duration, TimeSlot, Email?

**Decision:**
Use Value Objects - immutable records with factory methods.

**Reasoning:**
- Encapsulates validation at creation time
- Makes invalid state unrepresentable
- Self-documenting code
- Shows DDD understanding

**Example:**
```csharp
public record Duration
{
    public int Minutes { get; }

    private Duration(int minutes) => Minutes = minutes;

    public static Result<Duration> Create(int minutes)
    {
        if (minutes < 15) return Result.Failure<Duration>("Minimum 15 minutes");
        if (minutes > 480) return Result.Failure<Duration>("Maximum 8 hours");
        return Result.Success(new Duration(minutes));
    }
}
```

---

### ADR-004: In-Memory Database for Development

**Status:** Accepted

**Context:**
Need fast development iteration without SQL Server setup.

**Decision:**
Use EF Core In-Memory provider for development, SQL Server for production.

**Reasoning:**
- Zero setup for developers
- Fast test execution
- Same EF Core API, easy to switch
- Production uses real SQL Server for realistic behavior

**Configuration:**
```csharp
if (config.UseInMemory)
    options.UseInMemoryDatabase("ScheduleKit");
else
    options.UseSqlServer(connectionString);
```

---

## Technical Decisions Log

| Date | Decision | Why | Impact |
|------|----------|-----|--------|
| 2026-01-20 | Use .NET 8 | LTS, Minimal APIs, native AOT ready | Long-term support |
| 2026-01-20 | Use Minimal APIs over Controllers | Less boilerplate, modern approach | Cleaner endpoints |
| 2026-01-20 | Use FluentValidation | Testable, fluent syntax | Better validation UX |
| 2026-01-20 | Use Serilog | Structured logging, multiple sinks | Better debugging |
| 2026-01-20 | Use Mapperly over AutoMapper | Source-generated, zero reflection | Better performance |

---

## Lessons Learned

*This section will be updated as we encounter and solve problems.*

### Lesson 1: (Template)

**Problem:**
*Describe the problem encountered*

**What We Tried:**
*List approaches attempted*

**Solution:**
*What actually worked*

**Takeaway:**
*What to remember for the future*

---

## Questions & Answers

*Document questions that came up and their answers for future reference.*

| Question | Answer | Source |
|----------|--------|--------|
| Why Clean Architecture? | Testability, dependency rule, clear boundaries | improvedPrompt.md |
| Why MediatR? | Pipeline behaviors, decoupling, interview-worthy | improvedPrompt.md |

---

## Reference Links

- [Main Specification](./improvedPrompt.md) - ALWAYS consult this first
- [Sprint Progress](./ProjectSprint.md) - Track what's done
- [Onboarding Guide](./Onboarding.md) - For new developers

---

*Last Updated: 2026-01-20*
