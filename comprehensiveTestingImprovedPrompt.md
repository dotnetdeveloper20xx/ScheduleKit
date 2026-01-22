# ScheduleKit - Comprehensive Testing & Quality Assurance Protocol

## Application Overview

**Project:** ScheduleKit - Enterprise-Grade Scheduling Module
**Stack:** ASP.NET Core 8 + React 18 + TypeScript + SQL Server/In-Memory
**Architecture:** Clean Architecture + CQRS with MediatR

---

## YOUR ROLES

You must wear THREE hats simultaneously when testing this application:

### HAT 1: Senior UI/UX Designer (15+ Years Experience)
- Visual hierarchy and information architecture
- Accessibility standards (WCAG 2.1 AA)
- Modern scheduling app design patterns (Calendly, Cal.com reference)
- Typography, spacing, color theory
- Mobile-first responsive design for booking flows

**Your standard:** "Would this compete with Calendly in a portfolio review?"

### HAT 2: Senior QA Engineer (Ruthless Critic)
- Every click, every input, every edge case
- Timezone edge cases (DST, international users)
- Booking conflict scenarios
- Real-time update race conditions
- Authentication and authorization boundaries

**Your standard:** "If there's a way to break the scheduling logic, I will find it."

### HAT 3: Technical Architect
- Clean Architecture adherence
- CQRS pattern correctness
- API design best practices
- SignalR real-time reliability
- Security vulnerabilities (OWASP Top 10)

**Your standard:** "This code should pass a senior .NET developer's code review."

---

## CRITICAL INSTRUCTION

**The application is NOT complete until it passes EVERY section with 90%+ score.**

**NEVER DELIVER if:**
- Any page shows blank/empty content
- Login/Registration doesn't work
- Booking flow is broken at any step
- Available slots don't calculate correctly
- Dropdowns are empty
- Console shows errors during normal use
- Mobile booking experience is broken

---

# PART 1: FOUNDATION CHECKS

## 1.1 Application Startup Verification

```
BACKEND STARTUP:
Command: cd src/ScheduleKit.Api && dotnet run
Port: 58815

□ Application starts without errors
□ No compilation warnings
□ Health endpoint responds: GET http://localhost:58815/health
□ Swagger loads: http://localhost:58815/swagger
□ CORS configured correctly for frontend

FRONTEND STARTUP:
Command: cd ui/schedulekit-ui && node node_modules/vite/bin/vite.js --host
Port: 3000

□ Vite compiles without errors
□ No TypeScript errors
□ Application loads at http://localhost:3000
□ No console errors on initial load
□ API proxy working (requests to /api/* forwarded to backend)

STARTUP SCORE: ___/10
```

## 1.2 Database & Seed Data Verification

```
DATABASE CONNECTION:
□ In-memory database initializes on startup
□ Migrations apply automatically
□ Seed data loads on startup

SEED DATA CHECKLIST:
| Entity | Min Records | Required Variety |
|--------|-------------|------------------|
| Users | 3 | Admin, Host with events, New user |
| Event Types | 5+ | 15min, 30min, 60min durations, active/inactive |
| Weekly Availability | For each host | Different schedules per host |
| Availability Overrides | 3+ | Block days, extended hours |
| Bookings | 10+ | Confirmed, Cancelled, Past, Future |

SEED DATA QUALITY:
□ Realistic names (not "test", "user1")
□ Valid emails (@example.com)
□ Various booking statuses
□ Dates spanning past, present, future
□ Different timezones represented
□ Event types with various durations and settings

DATABASE SCORE: ___/10
```

## 1.3 API Endpoint Verification

### Authentication Endpoints
```
| Endpoint | Method | Test | Expected | Actual | Pass? |
|----------|--------|------|----------|--------|-------|
| /api/v1/auth/register | POST | Valid data | 201 + user | | □ |
| /api/v1/auth/login | POST | Valid creds | 200 + token | | □ |
| /api/v1/auth/login | POST | Invalid creds | 401 | | □ |
| /api/v1/auth/me | GET | With token | 200 + user | | □ |
| /api/v1/auth/me | GET | No token | 401 | | □ |
| /api/v1/auth/oauth/providers | GET | - | 200 + list | | □ |
```

### Event Type Endpoints
```
| Endpoint | Method | Test | Expected | Actual | Pass? |
|----------|--------|------|----------|--------|-------|
| /api/v1/event-types | GET | Authenticated | 200 + list | | □ |
| /api/v1/event-types | POST | Valid data | 201 + event | | □ |
| /api/v1/event-types/{id} | GET | Valid ID | 200 + event | | □ |
| /api/v1/event-types/{id} | PUT | Valid update | 200 | | □ |
| /api/v1/event-types/{id} | DELETE | Valid ID | 204 | | □ |
```

### Availability Endpoints
```
| Endpoint | Method | Test | Expected | Actual | Pass? |
|----------|--------|------|----------|--------|-------|
| /api/v1/availability | GET | Authenticated | 200 + schedule | | □ |
| /api/v1/availability | PUT | Valid schedule | 200 | | □ |
| /api/v1/availability/overrides | GET | Authenticated | 200 + list | | □ |
| /api/v1/availability/overrides | POST | Valid override | 201 | | □ |
```

### Booking Endpoints
```
| Endpoint | Method | Test | Expected | Actual | Pass? |
|----------|--------|------|----------|--------|-------|
| /api/v1/bookings | GET | Authenticated | 200 + paginated | | □ |
| /api/v1/bookings/{id} | GET | Valid ID | 200 + booking | | □ |
| /api/v1/bookings/{id}/cancel | POST | Valid | 200 | | □ |
| /api/v1/bookings/{id}/reschedule | POST | Valid | 200 | | □ |
```

### Public Endpoints (Guest-Facing)
```
| Endpoint | Method | Test | Expected | Actual | Pass? |
|----------|--------|------|----------|--------|-------|
| /api/v1/public/{hostSlug}/{eventSlug} | GET | Valid slugs | 200 + event | | □ |
| /api/v1/public/slots/{eventTypeId} | GET | Valid + date | 200 + slots | | □ |
| /api/v1/public/bookings | POST | Valid booking | 201 | | □ |
```

### Analytics Endpoint
```
| Endpoint | Method | Test | Expected | Actual | Pass? |
|----------|--------|------|----------|--------|-------|
| /api/v1/analytics/dashboard | GET | Authenticated | 200 + stats | | □ |
```

API SCORE: ___/10

---

# PART 2: AUTHENTICATION TESTING

## 2.1 Registration Flow

```
URL: http://localhost:3000/register (or /login with register option)

□ TEST: Navigate to registration
  Expected: Form with name, email, password fields
  Actual: ______

□ TEST: Submit empty form
  Expected: Validation errors for all required fields
  Actual: ______

□ TEST: Submit invalid email
  Expected: Email format validation error
  Actual: ______

□ TEST: Submit weak password
  Expected: Password requirements error
  Actual: ______

□ TEST: Submit valid registration
  Data: Name="Test User", Email="testuser@example.com", Password="Test123!@#"
  Expected: Account created, redirected to dashboard
  Actual: ______

□ TEST: Register with existing email
  Expected: "Email already exists" error
  Actual: ______

REGISTRATION SCORE: ___/10
```

## 2.2 Login Flow

```
URL: http://localhost:3000/login

TEST CREDENTIALS (from seed data):
Email: dotnetdeveloper20xx@hotmail.com
Name: Afzal Ahmed

□ TEST: Navigate to login page
  Expected: Form with email, password, OAuth buttons
  Actual: ______

□ TEST: Submit empty form
  Expected: Validation errors
  Actual: ______

□ TEST: Submit wrong credentials
  Expected: Generic "Invalid credentials" (not revealing which field)
  Actual: ______

□ TEST: Submit correct credentials
  Expected: Success, redirect to dashboard, user info visible
  Actual: ______

□ TEST: Refresh page after login
  Expected: Session persists, still logged in
  Actual: ______

□ TEST: Access /dashboard without login
  Expected: Redirect to /login
  Actual: ______

LOGIN SCORE: ___/10
```

## 2.3 OAuth Flow (Mock)

```
□ TEST: Click "Sign in with Google" button
  Expected: Redirects to mock OAuth page
  Actual: ______

□ TEST: Complete mock OAuth flow
  Expected: Returns to app, logged in
  Actual: ______

□ TEST: OAuth callback handles errors
  Expected: Shows error message, allows retry
  Actual: ______

OAUTH SCORE: ___/10
```

## 2.4 Logout Flow

```
□ TEST: Click logout button/link
  Expected: Session cleared, redirect to login/home
  Actual: ______

□ TEST: Access protected page after logout
  Expected: Redirect to login
  Actual: ______

□ TEST: Back button after logout
  Expected: Cannot see protected content
  Actual: ______

LOGOUT SCORE: ___/10
```

---

# PART 3: CORE FEATURE TESTING

## 3.1 Dashboard Page

```
URL: http://localhost:3000/dashboard (after login)

□ TEST: Page loads with data
  Expected: Welcome message, quick stats, upcoming bookings
  Actual: ______

□ TEST: Analytics widgets show data
  Expected: Numbers for total bookings, upcoming, etc.
  Actual: ______

□ TEST: Upcoming bookings list populated
  Expected: Shows next scheduled meetings
  Actual: ______

□ TEST: Quick action buttons work
  Expected: Can navigate to create event, view bookings
  Actual: ______

DASHBOARD SCORE: ___/10
```

## 3.2 Event Types Management

### Event Types List
```
URL: http://localhost:3000/event-types

□ TEST: Page loads with event types
  Expected: List/cards of event types from seed data (NOT EMPTY)
  Actual: ______

□ TEST: Event type cards show correct info
  | Field | Expected | Displayed | Correct? |
  |-------|----------|-----------|----------|
  | Name | From seed | | □ |
  | Duration | e.g., "30 minutes" | | □ |
  | Status | Active/Inactive badge | | □ |
  | Booking link | Copy button works | | □ |

□ TEST: Create new event type button visible
  Expected: Prominent "Create Event Type" button
  Actual: ______

□ TEST: Edit button on each event type
  Expected: Opens edit form with data
  Actual: ______

□ TEST: Delete button with confirmation
  Expected: Confirmation dialog, then removes
  Actual: ______
```

### Create Event Type Form
```
URL: http://localhost:3000/event-types/new

□ TEST: Form displays all fields
  | Field | Type | Present? | Default Value |
  |-------|------|----------|---------------|
  | Name | text | □ | |
  | Slug | text | □ | Auto-generated |
  | Description | textarea | □ | |
  | Duration | dropdown/number | □ | 30 |
  | Color | color picker | □ | |
  | Buffer Before | number | □ | 0 |
  | Buffer After | number | □ | 0 |
  | Minimum Notice | number | □ | 24 hours |
  | Booking Window | number | □ | 60 days |
  | Location Type | dropdown | □ | |
  | Is Active | toggle | □ | true |

□ TEST: Duration dropdown has options
  Expected: 15, 30, 45, 60, 90, 120 minutes (NOT EMPTY)
  Actual: ______

□ TEST: Submit valid event type
  Data: Name="Quick Chat", Duration=15, etc.
  Expected: Created, redirect to list, appears in list
  Actual: ______

□ TEST: Validation errors
  - Empty name: Shows error
  - Invalid duration: Shows error
  - Duplicate slug: Shows error
```

### Edit Event Type Form
```
URL: http://localhost:3000/event-types/{id}/edit

□ TEST: Form loads with existing data
  Expected: All fields pre-populated from database
  Actual: ______

□ TEST: Modify and save
  Change: Duration from 30 to 45
  Expected: Saved, changes persist on reload
  Actual: ______

EVENT TYPES SCORE: ___/10
```

## 3.3 Availability Management

### Weekly Availability
```
URL: http://localhost:3000/availability

□ TEST: Weekly schedule displays
  Expected: Grid showing Mon-Sun with time slots
  Actual: ______

□ TEST: Existing availability loaded
  Expected: Seed data times shown (e.g., Mon-Fri 9am-5pm)
  Actual: ______

□ TEST: Toggle day on/off
  Expected: Can mark days as available/unavailable
  Actual: ______

□ TEST: Set time range for day
  Expected: Can set start time (9:00 AM) and end time (5:00 PM)
  Actual: ______

□ TEST: Add multiple time blocks per day
  Expected: Can add split availability (9-12, 2-5)
  Actual: ______

□ TEST: Save availability
  Expected: Saves successfully, persists on reload
  Actual: ______

□ TEST: Timezone selector
  Expected: User can set their timezone
  Actual: ______
```

### Availability Overrides
```
URL: http://localhost:3000/availability (overrides section/tab)

□ TEST: Override list displays
  Expected: Shows existing date-specific overrides
  Actual: ______

□ TEST: Create blocking override (vacation)
  Data: Dec 25, 2026 - Blocked
  Expected: Date marked as unavailable
  Actual: ______

□ TEST: Create extended hours override
  Data: Dec 20, 2026 - 8am-8pm (extended)
  Expected: Shows extended availability
  Actual: ______

□ TEST: Delete override
  Expected: Override removed, date reverts to weekly schedule
  Actual: ______

AVAILABILITY SCORE: ___/10
```

## 3.4 Bookings Management (Host View)

```
URL: http://localhost:3000/bookings

□ TEST: Bookings list displays
  Expected: Table/list of bookings from seed data (NOT EMPTY)
  Actual: ______

□ TEST: Bookings show correct info
  | Column | Expected | Displayed | Correct? |
  |--------|----------|-----------|----------|
  | Guest Name | From seed | | □ |
  | Event Type | Name | | □ |
  | Date/Time | Formatted | | □ |
  | Status | Badge (Confirmed/Cancelled) | | □ |

□ TEST: Filter by status
  Expected: Can filter to show Confirmed/Cancelled/All
  Actual: ______

□ TEST: Filter by date range
  Expected: Can filter upcoming/past bookings
  Actual: ______

□ TEST: Pagination works
  Expected: Can navigate pages if 10+ bookings
  Actual: ______

□ TEST: View booking details
  Expected: Modal/page with full booking info
  Actual: ______

□ TEST: Cancel booking
  Expected: Confirmation dialog, status changes to Cancelled
  Actual: ______

□ TEST: Reschedule booking
  Expected: Can select new date/time, booking updates
  Actual: ______

BOOKINGS MANAGEMENT SCORE: ___/10
```

## 3.5 Public Booking Flow (Guest Experience)

**This is the most critical flow - test thoroughly!**

```
URL: http://localhost:3000/book/{hostSlug}/{eventSlug}
Example: http://localhost:3000/book/afzal-ahmed/30-minute-consultation
```

### Step 1: Event Type Page
```
□ TEST: Page loads with event details
  Expected: Name, duration, description visible
  Actual: ______

□ TEST: Host info displayed
  Expected: Host name, avatar (if any)
  Actual: ______

□ TEST: Calendar/date picker visible
  Expected: Can see available dates (highlighted)
  Actual: ______
```

### Step 2: Date Selection
```
□ TEST: Available dates highlighted
  Expected: Only dates within booking window shown as available
  Actual: ______

□ TEST: Unavailable dates disabled
  - Past dates: Greyed out
  - Override blocked dates: Greyed out
  - Beyond booking window: Greyed out

□ TEST: Select a date
  Expected: Shows available time slots for that date
  Actual: ______
```

### Step 3: Time Slot Selection
```
□ TEST: Time slots display correctly
  Expected: Slots based on:
  - Host's availability for that day
  - Event duration
  - Buffer times
  - Minimum notice period
  - Existing bookings removed
  Actual: ______

□ TEST: Slots shown in guest's timezone
  Expected: Times displayed in detected/selected timezone
  Actual: ______

□ TEST: Select a time slot
  Expected: Slot highlighted, proceed to form
  Actual: ______

□ TEST: Already booked slots not shown
  Expected: Slots with existing bookings hidden
  Actual: ______
```

### Step 4: Booking Form
```
□ TEST: Form displays with required fields
  | Field | Required | Present? |
  |-------|----------|----------|
  | Name | Yes | □ |
  | Email | Yes | □ |
  | Notes | No | □ |
  | Phone | No | □ |

□ TEST: Selected date/time shown
  Expected: Summary of chosen slot visible
  Actual: ______

□ TEST: Submit empty form
  Expected: Validation errors for name, email
  Actual: ______

□ TEST: Submit valid booking
  Data: Name="John Doe", Email="john@example.com"
  Expected: Booking created, redirect to confirmation
  Actual: ______
```

### Step 5: Confirmation Page
```
□ TEST: Confirmation page displays
  Expected: Shows booking details
  Actual: ______

□ TEST: All details correct
  | Info | Expected | Displayed | Correct? |
  |------|----------|-----------|----------|
  | Event Name | From selection | | □ |
  | Date/Time | Selected slot | | □ |
  | Host Name | Host's name | | □ |
  | Guest Name | Submitted | | □ |
  | Meeting Link | If video enabled | | □ |

□ TEST: Add to Calendar link
  Expected: Can download .ics or add to calendar
  Actual: ______

□ TEST: Reschedule link
  Expected: Link to reschedule the booking
  Actual: ______

□ TEST: Cancel link
  Expected: Link to cancel the booking
  Actual: ______

PUBLIC BOOKING SCORE: ___/10
```

## 3.6 Slot Calculation Logic (Critical Algorithm)

```
SCENARIO TESTS:

□ TEST: Basic availability
  Host available: Mon 9am-5pm
  Event duration: 30 min
  Expected slots: 9:00, 9:30, 10:00... 4:30 (16 slots)
  Actual: ______

□ TEST: Buffer time respected
  Buffer before: 15 min, Buffer after: 15 min
  Duration: 30 min
  Effective block: 60 min
  Expected: Fewer slots available
  Actual: ______

□ TEST: Existing booking blocks slot
  Booking at 10:00-10:30
  Expected: 10:00 slot not available
  Actual: ______

□ TEST: Minimum notice respected
  Minimum notice: 24 hours
  Current time: Jan 22, 2pm
  Expected: No slots for Jan 22, 23
  Actual: ______

□ TEST: Booking window respected
  Booking window: 60 days
  Expected: Dates beyond 60 days not selectable
  Actual: ______

□ TEST: Override blocks date
  Override: Jan 25 blocked
  Expected: Jan 25 shows no slots
  Actual: ______

□ TEST: Override extends hours
  Normal: 9am-5pm
  Override: 8am-8pm on Jan 20
  Expected: Extra slots on Jan 20
  Actual: ______

□ TEST: Timezone conversion
  Host timezone: America/New_York (EST)
  Guest timezone: Europe/London (GMT)
  Host slot: 9:00 AM EST
  Expected: Guest sees 2:00 PM GMT
  Actual: ______

SLOT CALCULATION SCORE: ___/10
```

## 3.7 Real-Time Updates (SignalR)

```
□ TEST: SignalR connection established
  Expected: No connection errors in console
  Actual: ______

□ TEST: Booking notification to host
  Action: Guest makes new booking
  Expected: Host dashboard updates in real-time
  Actual: ______

□ TEST: Slot disappears when booked
  Setup: Two browser tabs on same booking page
  Action: Tab 1 books a slot
  Expected: Tab 2 sees slot disappear
  Actual: ______

□ TEST: Cancellation releases slot
  Action: Cancel a booking
  Expected: Slot reappears for others
  Actual: ______

SIGNALR SCORE: ___/10
```

## 3.8 Analytics Dashboard

```
URL: http://localhost:3000/analytics (or dashboard section)

□ TEST: Stats display correctly
  | Metric | Shows Data? | Reasonable Value? |
  |--------|-------------|-------------------|
  | Total Bookings | □ | □ |
  | Upcoming | □ | □ |
  | Completed | □ | □ |
  | Cancelled | □ | □ |
  | This Week | □ | □ |
  | This Month | □ | □ |

□ TEST: Charts/graphs render
  Expected: Visual representation of booking trends
  Actual: ______

□ TEST: Popular times shown
  Expected: Heat map or list of most booked times
  Actual: ______

ANALYTICS SCORE: ___/10
```

## 3.9 Embeddable Widget

```
□ TEST: Widget script loads
  URL: http://localhost:3000/widget.js (or similar)
  Expected: Script file accessible
  Actual: ______

□ TEST: Widget renders in iframe
  Create test HTML with embed code
  Expected: Booking interface appears
  Actual: ______

□ TEST: Widget booking flow works
  Expected: Can complete booking within widget
  Actual: ______

□ TEST: Widget styling customizable
  Expected: Host colors/branding applied
  Actual: ______

WIDGET SCORE: ___/10
```

---

# PART 4: UI/UX EVALUATION

## 4.1 First Impressions (5-Second Test)

For each main page, evaluate:

```
PAGE: Login Page
□ Clarity: Purpose obvious within 5 seconds?
□ Hierarchy: Login form is the focus?
□ Professional: Looks like a real product?
□ Action: "Login" button is prominent?
SCORE: ___/10

PAGE: Dashboard
□ Clarity: User understands what they can do?
□ Hierarchy: Key metrics/actions prominent?
□ Professional: Clean, organized layout?
□ Action: Primary actions visible?
SCORE: ___/10

PAGE: Public Booking Page
□ Clarity: Guest knows how to book?
□ Hierarchy: Calendar/slots are the focus?
□ Professional: Trust-inspiring design?
□ Action: Clear path to complete booking?
SCORE: ___/10

FIRST IMPRESSIONS AVERAGE: ___/10
```

## 4.2 Visual Consistency

```
□ Same font family throughout
□ Consistent color scheme
□ Button styles match across pages
□ Same spacing/padding patterns
□ Icons from same set (consistent style)
□ Headers styled consistently
□ Form inputs styled consistently
□ Cards/panels have same shadow/border treatment

CONSISTENCY SCORE: ___/10
```

## 4.3 Typography

```
FONT HIERARCHY CHECK:
| Element | Size | Weight | Readable? |
|---------|------|--------|-----------|
| Page titles (H1) | | | □ |
| Section headers (H2) | | | □ |
| Body text | | | □ |
| Button text | | | □ |
| Form labels | | | □ |
| Helper text | | | □ |

□ Clear size progression between levels
□ Body text minimum 16px
□ Line height 1.4-1.6 for readability
□ Sufficient contrast (WCAG AA: 4.5:1)
□ No ALL CAPS for long text
□ Maximum line length 75 characters

TYPOGRAPHY SCORE: ___/10
```

## 4.4 Color System

```
COLOR PALETTE:
- Primary: ______ (used for: )
- Secondary: ______ (used for: )
- Success: ______ (booking confirmed)
- Warning: ______ (approaching limit)
- Error: ______ (validation errors)
- Background: ______
- Text: ______

□ Colors used consistently
□ Primary used for primary actions
□ Error states use error color
□ Success states use success color
□ Sufficient contrast for accessibility
□ Color not the ONLY indicator (icons/text also)

COLOR SCORE: ___/10
```

## 4.5 Responsive Design

```
DESKTOP (1920px):
□ Layout uses space well
□ Content not stretched edge-to-edge
□ Sidebar proportional
Issues: ______

LAPTOP (1440px):
□ Layout still optimal
□ All features accessible
Issues: ______

TABLET (768px):
□ Navigation collapses to hamburger
□ Booking calendar usable
□ Forms fit screen
Issues: ______

MOBILE (375px):
□ All content accessible
□ No horizontal scroll
□ Time slots tappable (44px min)
□ Booking form usable
□ Confirmation readable
Issues: ______

RESPONSIVE SCORE: ___/10
```

## 4.6 Loading & Empty States

```
LOADING STATES:
□ Page loads: Spinner or skeleton
□ Data fetching: Loading indicator
□ Form submission: Button shows loading
□ No empty flashes before content

EMPTY STATES:
□ No bookings: Helpful message + CTA
□ No event types: Prompt to create first
□ No search results: Clear message
□ No slots available: Explains why

ERROR STATES:
□ Form errors: Near relevant fields
□ API errors: User-friendly message
□ Network error: Retry option
□ 404: Custom page with navigation

FEEDBACK SCORE: ___/10
```

## 4.7 Form Design

```
For booking form, event type form, availability form:

LAYOUT:
□ Single column (easier to complete)
□ Logical field grouping
□ Required fields marked (asterisk)
□ Labels above or beside fields (consistent)

INPUTS:
□ Correct input types (email, tel, date)
□ Appropriate field widths
□ Dropdowns populated (NOT EMPTY)
□ Date pickers for dates
□ Time pickers for times

VALIDATION:
□ Real-time validation on blur
□ Error messages near fields
□ Error messages explain fix
□ Success indicators for valid

FORM UX SCORE: ___/10
```

## 4.8 Accessibility (WCAG 2.1 AA)

```
PERCEIVABLE:
□ All images have alt text
□ Color is not only indicator
□ Contrast ratios meet 4.5:1 (text) / 3:1 (UI)
□ Text can resize to 200%

OPERABLE:
□ All functionality via keyboard
□ No keyboard traps
□ Focus states visible
□ Skip links present
□ No time limits (or extendable)

UNDERSTANDABLE:
□ Language specified (html lang)
□ Navigation consistent
□ Error identification clear
□ Labels provided for inputs

KEYBOARD NAVIGATION TEST:
□ Tab through entire booking flow
□ Can complete booking keyboard-only
□ Focus order is logical
□ Escape closes modals
□ Enter activates buttons

ACCESSIBILITY SCORE: ___/10
```

---

# PART 5: TECHNICAL QUALITY AUDIT

## 5.1 Backend Quality

```
ARCHITECTURE:
□ Clean Architecture layers separated
□ Domain has no external dependencies
□ Application uses MediatR correctly
□ Infrastructure implements interfaces

CODE QUALITY:
□ No compiler warnings
□ Consistent naming conventions
□ No commented-out code
□ Proper error handling

SECURITY:
□ Input validation on all endpoints
□ SQL injection prevention (EF Core parameterized)
□ Authentication on protected routes
□ No sensitive data in logs
□ CORS restricted to frontend origin

API DESIGN:
□ RESTful conventions followed
□ Proper HTTP status codes
□ Consistent response envelope
□ Pagination on list endpoints
□ Validation errors return 400 with details

BACKEND SCORE: ___/10
```

## 5.2 Frontend Quality

```
STRUCTURE:
□ Components organized logically
□ API hooks in dedicated folder
□ Types defined properly
□ No any types (TypeScript strict)

STATE MANAGEMENT:
□ TanStack Query for server state
□ Proper cache invalidation
□ Optimistic updates where helpful
□ Loading/error states handled

ERROR HANDLING:
□ API errors caught and displayed
□ Error boundaries for crashes
□ Form validation client-side

PERFORMANCE:
□ No unnecessary re-renders
□ Large lists paginated
□ Images optimized (if any)
□ Bundle size reasonable

FRONTEND SCORE: ___/10
```

## 5.3 Console & Network Check

```
During normal application use:

BROWSER CONSOLE:
□ No JavaScript errors
□ No React warnings
□ No failed network requests
□ SignalR connects without errors

NETWORK TAB:
□ API calls return 2xx for success
□ No CORS errors
□ Response times < 500ms
□ No duplicate requests

CONSOLE SCORE: ___/10
```

---

# PART 6: SCORING & DELIVERY DECISION

## 6.1 Score Summary

```
FOUNDATION:
- Startup: ___/10
- Database/Seed: ___/10
- API Endpoints: ___/10
FOUNDATION SUBTOTAL: ___/30

AUTHENTICATION:
- Registration: ___/10
- Login: ___/10
- OAuth: ___/10
- Logout: ___/10
AUTH SUBTOTAL: ___/40

CORE FEATURES:
- Dashboard: ___/10
- Event Types: ___/10
- Availability: ___/10
- Bookings Management: ___/10
- Public Booking Flow: ___/10
- Slot Calculation: ___/10
- SignalR Real-time: ___/10
- Analytics: ___/10
- Widget: ___/10
FEATURES SUBTOTAL: ___/90

UI/UX:
- First Impressions: ___/10
- Visual Consistency: ___/10
- Typography: ___/10
- Color System: ___/10
- Responsive: ___/10
- Loading/Empty States: ___/10
- Form Design: ___/10
- Accessibility: ___/10
UI/UX SUBTOTAL: ___/80

TECHNICAL:
- Backend Quality: ___/10
- Frontend Quality: ___/10
- Console/Network: ___/10
TECHNICAL SUBTOTAL: ___/30

═══════════════════════════════════════
OVERALL SCORE: ___/270 = ___%
═══════════════════════════════════════
```

## 6.2 Critical Failures Checklist

**Any of these = CANNOT DELIVER:**

```
□ Empty pages with no data
□ Login doesn't work with documented credentials
□ Public booking flow broken at any step
□ Available slots calculation incorrect
□ Empty dropdowns in forms
□ Console errors during normal use
□ Mobile booking experience broken
□ SignalR not connecting
```

## 6.3 Delivery Decision

```
[ ] READY TO DELIVER (Score >= 90%, no critical failures)
[ ] NEEDS FIXES (Score 70-89% or critical failures present)
[ ] NOT READY (Score < 70%, multiple critical failures)

Issues to fix before delivery:
1. ______
2. ______
3. ______
```

---

# QUICK START REFERENCE

## Test Credentials
```
Email: dotnetdeveloper20xx@hotmail.com
Name: Afzal Ahmed
Note: Using in-memory database - data resets on restart
```

## Startup Commands
```bash
# Backend (port 58815)
cd src/ScheduleKit.Api
dotnet run

# Frontend (port 3000)
cd ui/schedulekit-ui
node node_modules/vite/bin/vite.js --host
```

## Key URLs
```
Frontend: http://localhost:3000
Login: http://localhost:3000/login
Dashboard: http://localhost:3000/dashboard
Swagger: http://localhost:58815/swagger
Health: http://localhost:58815/health
```

---

**Remember: A scheduling application with broken booking flow, incorrect slot calculation, or amateur UI is NOT COMPLETE. The guest booking experience must be flawless.**
