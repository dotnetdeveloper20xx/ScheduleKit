# ScheduleKit - Sprint Progress Tracker

## Overview

This document tracks the progress of all sprints in the ScheduleKit project. Updated as work progresses.

---

## Sprint Summary

| Sprint | Phase | Goal | Status | Progress |
|--------|-------|------|--------|----------|
| Sprint 1-2 | Foundation | Empty shell that runs and proves architecture | In Progress | 60% |
| Sprint 3-4 | Core Scheduling | Host defines availability, guests see slots | Not Started | 0% |
| Sprint 5-6 | Booking Flow | Complete booking lifecycle | Not Started | 0% |
| Sprint 7-8 | Real-Time & Polish | Production-ready feel | Not Started | 0% |

---

## Sprint 1-2: Foundation

**Goal:** Empty shell that runs and proves the architecture works

**Deliverable:** Can create event type via UI, persists to database

### Backend Tasks

| Task | Status | Notes |
|------|--------|-------|
| Solution structure with all projects | ✅ Complete | 8 projects: Domain, Application, Infrastructure, Api, Demo + 3 test projects |
| Domain entities with value objects | ✅ Complete | EventType, Booking, Availability, BookingQuestion + Value Objects |
| EF Core DbContext with configurations | ✅ Complete | In-Memory and SQL Server support, entity configurations |
| MediatR setup with pipeline behaviors | ✅ Complete | Validation and Logging behaviors |
| Basic CRUD for EventType (vertical slice) | ✅ Complete | Commands, Queries, Handlers, API Controller |
| Health check endpoint | ✅ Complete | /health endpoint |
| Swagger/OpenAPI setup | ✅ Complete | Available at /swagger |
| Structured logging with Serilog | ✅ Complete | Console and file sinks |

### Frontend Tasks

| Task | Status | Notes |
|------|--------|-------|
| Vite + React + TypeScript setup | Not Started | |
| TanStack Query configuration | Not Started | |
| API client with type generation | Not Started | |
| Layout components | Not Started | |
| Event type list/create (connects to API) | Not Started | |

### Testing Tasks

| Task | Status | Notes |
|------|--------|-------|
| Unit test project structure | ✅ Complete | 85 tests: Domain (76), Application (3), Api (6) |
| First SlotCalculator tests | Not Started | Coming in Sprint 3-4 |
| Integration test setup with Testcontainers | Not Started | |

---

## Sprint 3-4: Core Scheduling

**Goal:** Host can define availability, guests can see available slots

**Deliverable:** Can set availability and see correct slots

### Backend Tasks

| Task | Status | Notes |
|------|--------|-------|
| Availability entity and endpoints | Not Started | |
| AvailabilityOverride entity and endpoints | Not Started | |
| SlotCalculator service | Not Started | |
| Timezone handling | Not Started | |
| Public slots endpoint with caching | Not Started | |

### Frontend Tasks

| Task | Status | Notes |
|------|--------|-------|
| Weekly availability editor | Not Started | |
| Date override management | Not Started | |
| Public booking page (date selection) | Not Started | |
| Time slot grid | Not Started | |

### Testing Tasks

| Task | Status | Notes |
|------|--------|-------|
| Comprehensive SlotCalculator tests | Not Started | |
| Timezone edge cases | Not Started | |
| Integration tests for slots endpoint | Not Started | |

---

## Sprint 5-6: Booking Flow

**Goal:** Complete booking lifecycle

**Deliverable:** End-to-end booking works

### Backend Tasks

| Task | Status | Notes |
|------|--------|-------|
| Booking entity with domain methods | Not Started | |
| CreateBooking command with conflict detection | Not Started | |
| Booking confirmation endpoint | Not Started | |
| Cancel booking endpoint | Not Started | |
| Domain events (BookingCreated, BookingCancelled) | Not Started | |
| Email notification handlers | Not Started | |

### Frontend Tasks

| Task | Status | Notes |
|------|--------|-------|
| Booking form with validation | Not Started | |
| Confirmation page | Not Started | |
| Host bookings list | Not Started | |
| Cancel booking flow | Not Started | |

### Testing Tasks

| Task | Status | Notes |
|------|--------|-------|
| Booking conflict tests | Not Started | |
| Email sending verification | Not Started | |
| Full booking flow integration tests | Not Started | |

---

## Sprint 7-8: Real-Time & Polish

**Goal:** Professional, production-ready feel

**Deliverable:** Impressive demo ready for portfolio

### Backend Tasks

| Task | Status | Notes |
|------|--------|-------|
| SignalR hub for real-time updates | Not Started | |
| Guest reschedule functionality | Not Started | |
| Booking questions/responses | Not Started | |
| Background job for reminders | Not Started | |

### Frontend Tasks

| Task | Status | Notes |
|------|--------|-------|
| Real-time slot updates | Not Started | |
| Optimistic mutations | Not Started | |
| Loading skeletons | Not Started | |
| Error boundaries | Not Started | |
| Mobile responsiveness | Not Started | |
| Dark mode | Not Started | |

### Testing Tasks

| Task | Status | Notes |
|------|--------|-------|
| E2E tests with Playwright | Not Started | |
| Performance testing for slot calculation | Not Started | |

---

## Change Log

| Date | Sprint | Change | Author |
|------|--------|--------|--------|
| 2026-01-20 | Sprint 1 | Initial sprint tracking document created | Developer |
| 2026-01-20 | Sprint 1 | Completed all backend tasks - solution structure, domain, infrastructure, API | Developer |
| 2026-01-20 | Sprint 1 | Completed test project structure with 85 passing tests | Developer |
