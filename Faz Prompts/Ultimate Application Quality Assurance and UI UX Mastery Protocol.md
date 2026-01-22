# Claude Prompt: Comprehensive Application Testing & Validation Suite

I completely understand your frustration. You're asking Claude to build applications, but you get:
- Empty pages with no data
- Login that doesn't work
- UI elements that look broken
- Features that exist in code but don't actually function
- Having to explain the same issues repeatedly

This prompt creates a **rigorous testing protocol** that Claude must execute BEFORE declaring any application "complete."

---

## The Prompt

````markdown
# Comprehensive Application Testing & Validation Protocol
## Mandatory Pre-Delivery Checklist for AI-Built Applications

## CRITICAL INSTRUCTION

**You are NOT finished building an application until you have completed EVERY section of this testing protocol and documented the results.**

When I ask you to build an application, you MUST:
1. Build the application
2. Run this ENTIRE testing protocol
3. Fix ALL issues found
4. Re-test until EVERYTHING passes
5. Only THEN deliver the application to me

**DO NOT** tell me the application is ready if:
- Any page shows empty/blank content
- Any form doesn't submit properly
- Any button doesn't perform its action
- Login/logout doesn't work end-to-end
- The database has no seed data
- Any feature is "stubbed out" or placeholder

---

## PART 1: FOUNDATION CHECKS (Must Pass Before Anything Else)

### 1.1 Application Startup Verification

```
□ CHECK: Application starts without errors
  - Backend starts and listens on expected port
  - Frontend compiles without errors
  - No console errors on initial load
  - API health endpoint responds (if exists)
  
  ACTION IF FAIL: Fix startup errors before proceeding
  
□ CHECK: Environment configuration is complete
  - Database connection string is configured
  - API base URL is correctly set in frontend
  - CORS is properly configured
  - All required environment variables are set
  
  ACTION IF FAIL: Add missing configuration
```

### 1.2 Database Verification

```
□ CHECK: Database exists and is accessible
  - Can connect to database
  - All tables/collections exist
  - Schema matches entity models
  
  ACTION IF FAIL: Run migrations, create database
  
□ CHECK: Seed data is populated
  - EVERY table has at least 5 realistic records
  - Related records have valid foreign keys
  - Enum fields have variety (not all same status)
  - Dates are realistic (past, present, future as appropriate)
  - Text fields have realistic content (not "test" or "asdf")
  
  ACTION IF FAIL: Create comprehensive seed data NOW
  
  SEED DATA REQUIREMENTS:
  - Users: At least 3 (admin, regular user, viewer if roles exist)
  - Each main entity: At least 5-10 records
  - Related entities: Proper relationships (orders belong to customers, etc.)
  - Variety: Different statuses, dates, amounts
  - Realistic: Real-looking names, emails (@example.com), addresses
```

### 1.3 API Endpoint Verification

```
For EVERY endpoint in the application:

□ CHECK: Endpoint is reachable
  - Returns appropriate status code (not 500)
  - Returns expected data shape
  - Handles missing/invalid parameters gracefully
  
Test each endpoint:
| Endpoint | Method | Test Input | Expected Result | Actual Result | Pass? |
|----------|--------|------------|-----------------|---------------|-------|
| /api/[x] | GET    | -          | List of items   | [Document]    | □     |
| /api/[x] | GET    | id=1       | Single item     | [Document]    | □     |
| /api/[x] | POST   | valid body | Created item    | [Document]    | □     |
| /api/[x] | PUT    | id + body  | Updated item    | [Document]    | □     |
| /api/[x] | DELETE | id=1       | Success         | [Document]    | □     |

ACTION IF FAIL: Fix endpoint, do not proceed until working
```

---

## PART 2: AUTHENTICATION & AUTHORIZATION TESTING

### 2.1 Registration Flow (If Applicable)

```
□ TEST: Navigate to registration page
  Expected: Form displays with all fields
  Actual: [Document what you see]
  
□ TEST: Submit empty form
  Expected: Validation errors shown for required fields
  Actual: [Document what happens]
  
□ TEST: Submit with invalid email format
  Expected: Email validation error
  Actual: [Document what happens]
  
□ TEST: Submit with password mismatch (if confirm password exists)
  Expected: Password mismatch error
  Actual: [Document what happens]
  
□ TEST: Submit valid registration
  Expected: Account created, redirected to login or dashboard
  Actual: [Document what happens]
  
□ TEST: Try to register with same email again
  Expected: "Email already exists" error
  Actual: [Document what happens]
```

### 2.2 Login Flow

```
□ TEST: Navigate to login page
  Expected: Form with email/username and password fields
  Actual: [Document what you see]
  
□ TEST: Submit empty form
  Expected: Validation errors
  Actual: [Document what happens]
  
□ TEST: Submit with wrong credentials
  Expected: "Invalid credentials" error (not specific about which field)
  Actual: [Document what happens]
  
□ TEST: Submit with correct credentials
  Using: [seeded user email] / [seeded user password]
  Expected: Redirected to dashboard/home, user info visible
  Actual: [Document what happens]
  
□ TEST: Refresh page after login
  Expected: Still logged in, session persists
  Actual: [Document what happens]
  
□ TEST: Access protected page without login
  Expected: Redirected to login
  Actual: [Document what happens]
```

### 2.3 Logout Flow

```
□ TEST: Click logout
  Expected: Session cleared, redirected to login/home
  Actual: [Document what happens]
  
□ TEST: Try to access protected page after logout
  Expected: Redirected to login
  Actual: [Document what happens]
  
□ TEST: Browser back button after logout
  Expected: Cannot access protected content
  Actual: [Document what happens]
```

### 2.4 Role-Based Access (If Applicable)

```
For each role in the system:

Role: [Admin/User/Viewer/etc.]
□ TEST: Login as this role
□ TEST: Verify menu shows correct items for this role
□ TEST: Verify can access allowed pages
□ TEST: Verify cannot access restricted pages
□ TEST: Verify can perform allowed actions
□ TEST: Verify cannot perform restricted actions
```

---

## PART 3: PAGE-BY-PAGE UI TESTING

### 3.1 Navigation Testing

```
□ TEST: Every menu item is clickable
  Document each menu item:
  | Menu Item | Expected Destination | Actually Goes To | Loads Content? |
  |-----------|---------------------|------------------|----------------|
  | [Item 1]  | /path               | [Actual path]    | □ Yes / □ No   |
  | [Item 2]  | /path               | [Actual path]    | □ Yes / □ No   |
  
□ TEST: Browser back/forward buttons work correctly
□ TEST: Direct URL access works (not just navigation)
□ TEST: 404 page shows for invalid URLs
```

### 3.2 List/Table Pages Testing

For EVERY page that displays a list or table:

```
Page: [Page Name] - [URL]

□ TEST: Page loads without errors
  Expected: Data table/list visible
  Actual: [Document what you see]
  
□ TEST: Data is displayed (NOT EMPTY)
  Expected: Seed data visible in table
  Actual: [Document - if empty, THIS IS A FAILURE]
  
  ⚠️ CRITICAL: If table is empty, STOP and fix:
  - Verify API endpoint returns data
  - Verify frontend calls correct endpoint
  - Verify data mapping is correct
  - Add seed data if missing
  
□ TEST: Table columns match expected data
  | Expected Column | Present? | Shows Correct Data? |
  |-----------------|----------|---------------------|
  | [Column 1]      | □        | □                   |
  | [Column 2]      | □        | □                   |
  
□ TEST: Pagination works (if applicable)
  - Can navigate to page 2
  - Page 2 shows different data
  - Page numbers update correctly
  
□ TEST: Sorting works (if applicable)
  - Click column header
  - Data reorders correctly
  - Sort indicator shows
  
□ TEST: Search/filter works (if applicable)
  - Enter search term that matches seed data
  - Results filter correctly
  - Clear search restores all results
  
□ TEST: Row actions work
  | Action Button | Expected Behavior | Actually Does |
  |---------------|-------------------|---------------|
  | View          | Opens detail page | [Document]    |
  | Edit          | Opens edit form   | [Document]    |
  | Delete        | Confirms & removes| [Document]    |
```

### 3.3 Detail/View Pages Testing

For EVERY detail/view page:

```
Page: [Page Name] - [URL pattern: /entity/:id]

□ TEST: Page loads with valid ID
  Using: ID from seed data
  Expected: All entity fields displayed
  Actual: [Document what you see]
  
□ TEST: All fields show data (not empty, not "undefined")
  | Field Name | Expected Value | Displayed Value | Correct? |
  |------------|----------------|-----------------|----------|
  | [Field 1]  | [From seed]    | [Actual]        | □        |
  | [Field 2]  | [From seed]    | [Actual]        | □        |
  
□ TEST: Related data displays (if applicable)
  - Child records show (e.g., order items on order page)
  - Links to related entities work
  
□ TEST: Page handles invalid ID
  Expected: 404 or "not found" message
  Actual: [Document what happens]
  
□ TEST: Action buttons work
  - Edit button opens edit form
  - Delete button confirms and deletes
  - Back button returns to list
```

### 3.4 Create/Add Form Testing

For EVERY create/add form:

```
Form: Create [Entity Name] - [URL]

□ TEST: Form displays with all fields
  | Field Name | Input Type | Present? | Correct Type? |
  |------------|------------|----------|---------------|
  | [Field 1]  | text       | □        | □             |
  | [Field 2]  | dropdown   | □        | □             |
  | [Field 3]  | date       | □        | □             |
  
□ TEST: Dropdowns are populated (NOT EMPTY)
  | Dropdown | Expected Options | Actually Shows |
  |----------|------------------|----------------|
  | [Drop 1] | [Option list]    | [Actual]       |
  
  ⚠️ CRITICAL: Empty dropdowns are a FAILURE. Fix by:
  - Verify API endpoint for options exists
  - Verify frontend fetches options on load
  - Add lookup/reference data to seed
  
□ TEST: Submit empty form
  Expected: Validation errors for required fields
  Actual: [Document which errors show]
  
□ TEST: Submit with invalid data
  | Invalid Input | Field | Expected Error | Actual Error |
  |---------------|-------|----------------|--------------|
  | "abc" in number field | Amount | "Must be number" | [Actual] |
  | Past date in future-only | Date | "Must be future" | [Actual] |
  
□ TEST: Submit with valid data
  Test data used:
  | Field | Value Entered |
  |-------|---------------|
  | [Field 1] | [Test value] |
  | [Field 2] | [Test value] |
  
  Expected: Record created, redirected to list/detail, success message
  Actual: [Document exactly what happens]
  
□ TEST: Verify record was actually created
  - Check it appears in list
  - Check it exists in database
  - Check all fields saved correctly
  
□ TEST: Cancel button works
  - Returns to previous page
  - No record created
```

### 3.5 Edit Form Testing

For EVERY edit form:

```
Form: Edit [Entity Name] - [URL pattern: /entity/:id/edit]

□ TEST: Form loads with existing data
  Using: Record ID from seed data
  Expected: All fields pre-populated with current values
  
  | Field | Expected Value (from DB) | Form Shows | Correct? |
  |-------|--------------------------|------------|----------|
  | [Field 1] | [DB value] | [Form value] | □ |
  | [Field 2] | [DB value] | [Form value] | □ |
  
  ⚠️ CRITICAL: If form loads empty, THIS IS A FAILURE. Fix by:
  - Verify API endpoint returns record data
  - Verify form fetches data on load
  - Verify data binds to form fields
  
□ TEST: Modify a field and save
  Change: [Field name] from [old value] to [new value]
  Expected: Success message, changes persisted
  Actual: [Document what happens]
  
□ TEST: Verify changes persisted
  - Reload the page - changes still there
  - Check database - value updated
  
□ TEST: Validation still works on edit
  - Clear required field, submit
  - Validation error appears
```

### 3.6 Delete Functionality Testing

```
□ TEST: Delete button shows confirmation
  Expected: "Are you sure?" dialog or confirmation
  Actual: [Document what happens]
  
□ TEST: Cancel delete
  Expected: Record still exists
  Actual: [Document - verify record still in list]
  
□ TEST: Confirm delete
  Expected: Record removed, no longer in list
  Actual: [Document what happens]
  
□ TEST: Verify actual deletion
  - Record gone from list
  - API returns 404 for that ID
  - Database record removed (or soft-deleted)
  
□ TEST: Handle delete of record with dependencies
  Expected: Error message OR cascade delete
  Actual: [Document behavior]
```

---

## PART 4: UI/UX QUALITY CHECKS

### 4.1 Visual Consistency

```
□ CHECK: Consistent styling across all pages
  - Same fonts throughout
  - Same color scheme
  - Same button styles
  - Same spacing/padding patterns
  
□ CHECK: No broken layouts
  - No overlapping elements
  - No elements extending beyond containers
  - No unexpected horizontal scrollbars
  - Content doesn't disappear on resize
  
□ CHECK: Loading states exist
  - Spinner or skeleton while data loads
  - User knows something is happening
  - No flash of empty content
  
□ CHECK: Empty states are handled
  - Empty table shows "No records found" (not just blank)
  - Empty search shows "No results" message
  - New user sees helpful empty state, not broken page
```

### 4.2 Error Handling

```
□ CHECK: Form errors are visible and helpful
  - Error messages appear near relevant fields
  - Messages explain how to fix the problem
  - Error state is visually distinct (red border, icon, etc.)
  
□ CHECK: API errors show user-friendly messages
  - Network error: "Connection problem, please try again"
  - Server error: "Something went wrong, please try again"
  - NOT raw error messages or stack traces
  
□ CHECK: 404 pages are handled
  - Invalid URLs show custom 404 page
  - Invalid IDs show "not found" message
  - User can navigate back to valid pages
```

### 4.3 Responsive Design (If Applicable)

```
□ CHECK: Desktop view (1920px)
  - All elements visible and properly spaced
  
□ CHECK: Tablet view (768px)
  - Layout adjusts appropriately
  - Navigation still accessible
  - Tables scroll or reformat
  
□ CHECK: Mobile view (375px)
  - Content is readable
  - Buttons are tappable
  - Forms are usable
  - Navigation collapses to menu
```

---

## PART 5: DATA INTEGRITY TESTING

### 5.1 CRUD Cycle Verification

For each main entity, complete a full cycle:

```
Entity: [Entity Name]

□ CREATE: Add new record with all fields
  Data used: [Document all field values]
  Result: [Record ID created]
  
□ READ: View the created record
  Verification: All fields display correctly
  
□ UPDATE: Modify the record
  Changed: [Field] from [Old] to [New]
  Verification: Changes persisted
  
□ DELETE: Remove the record
  Verification: Record no longer exists

CYCLE COMPLETE: □ Yes / □ No - Fix issues before proceeding
```

### 5.2 Relationship Integrity

```
□ TEST: Creating child record with parent reference
  - Create order for existing customer
  - Verify customer link works
  - Verify order appears in customer's order list
  
□ TEST: Viewing parent shows children
  - View customer detail
  - Customer's orders are listed
  
□ TEST: Cannot delete parent with children (or cascades properly)
  - Try to delete customer with orders
  - Expected behavior: [Block with error / Cascade delete]
  - Actual behavior: [Document]
```

### 5.3 Validation Consistency

```
□ TEST: Frontend and backend validation match
  - Disable JavaScript
  - Submit invalid form
  - Backend should catch same errors
  
□ TEST: Database constraints match validation
  - Try to insert invalid data via API directly
  - Database should reject if validation missed
```

---

## PART 6: PERFORMANCE & STABILITY

### 6.1 Basic Performance

```
□ CHECK: Pages load in reasonable time (< 3 seconds)
  | Page | Load Time | Acceptable? |
  |------|-----------|-------------|
  | Home | [Time] | □ |
  | List page | [Time] | □ |
  | Form page | [Time] | □ |
  
□ CHECK: No memory leaks on navigation
  - Navigate between pages multiple times
  - Application remains responsive
  
□ CHECK: Large data sets handled
  - If 100+ records, pagination works
  - List doesn't freeze
```

### 6.2 Error Recovery

```
□ TEST: Refresh during form entry
  Expected: Data may be lost (warn user) or persisted
  Actual: [Document behavior]
  
□ TEST: Session timeout handling
  Expected: Graceful redirect to login
  Actual: [Document behavior]
  
□ TEST: Network failure during save
  Expected: Error message, data not lost, can retry
  Actual: [Document behavior]
```

---

## PART 7: FINAL VERIFICATION CHECKLIST

Before declaring the application complete, answer these questions:

### User Experience Verification

```
□ Can a new user register (if applicable)?
□ Can a user log in with seeded credentials?
□ Does the user see data immediately after login (not empty pages)?
□ Can the user navigate to every page via the menu?
□ Does every page show relevant content (not blank)?
□ Can the user create a new record of each type?
□ Can the user view any record's details?
□ Can the user edit any record?
□ Can the user delete a record?
□ Does search/filter actually filter results?
□ Do all dropdowns have options (not empty)?
□ Are all form validations working?
□ Does the user see success/error messages?
□ Can the user log out?
```

### Technical Verification

```
□ Does the application start without errors?
□ Are there any console errors during normal use?
□ Do all API endpoints return expected data?
□ Is the database properly seeded?
□ Are all environment variables configured?
□ Does authentication persist across refresh?
□ Are protected routes actually protected?
□ Do all foreign key relationships work?
```

### Seed Data Verification

```
□ Does EVERY list page show data?
□ Does EVERY dropdown have options?
□ Is there enough variety to demonstrate features?
□ Are dates realistic (not all the same)?
□ Are statuses varied (not all "active")?
□ Are relationships valid (orders have customers, etc.)?
□ Can I log in with the documented credentials?
```

---

## DOCUMENTATION REQUIREMENTS

When you deliver the application, provide:

### 1. Startup Instructions
```
How to start the backend:
[Exact commands]

How to start the frontend:
[Exact commands]

How to run database migrations:
[Exact commands]

How to seed the database:
[Exact commands]
```

### 2. Test Credentials
```
Admin User:
- Email: [email]
- Password: [password]

Regular User:
- Email: [email]
- Password: [password]
```

### 3. Test Report Summary
```
Pages Tested: [Number]
Tests Passed: [Number]
Tests Failed: [Number]
Issues Fixed: [List]
Known Limitations: [List]
```

### 4. Seed Data Summary
```
| Entity | Records Created | Sample Data |
|--------|-----------------|-------------|
| Users | [N] | admin@example.com, user@example.com |
| Customers | [N] | Acme Corp, Sunrise Catering, etc. |
| Orders | [N] | Various statuses and dates |
```

---

## FAILURE PROTOCOL

If ANY of the following are true, DO NOT deliver the application:

❌ Any page shows blank/empty content that should have data
❌ Login does not work with provided credentials
❌ Any CRUD operation fails
❌ Any dropdown is empty when it should have options
❌ Any navigation link is broken
❌ Console shows errors during normal use
❌ Database is not seeded

Instead:
1. Document what failed
2. Fix the issue
3. Re-run affected tests
4. Continue until all tests pass

---

## HOW TO USE THIS PROTOCOL

When I ask you to build an application:

1. **Build the application** following my requirements
2. **Run this ENTIRE protocol** systematically
3. **Document results** in a test report
4. **Fix ALL failures** before proceeding
5. **Re-test** fixed areas
6. **Deliver** only when all critical tests pass

Include in your delivery:
- Complete source code
- Database seed scripts
- Test report summary
- Startup instructions
- Test credentials

**Remember: An application with an empty list page, non-functional login, or missing seed data is NOT COMPLETE.**
````

---

## How to Use This With Your Build Requests

When you ask Claude to build something, append this protocol. Here's a template:

```markdown
## My Request

Build me a [description of your application] with the following features:
- [Feature 1]
- [Feature 2]
- [Feature 3]

## Technology Stack
- Backend: [Your preference or "your choice"]
- Frontend: [Your preference or "your choice"]
- Database: [Your preference or "your choice"]

## MANDATORY REQUIREMENT

Before delivering this application, you MUST complete the "Comprehensive Application Testing & Validation Protocol" included below. 

Do NOT tell me the application is complete until:
1. Every page shows data (no empty tables/lists)
2. Login works with the credentials you provide
3. All CRUD operations function
4. All dropdowns are populated
5. You have documented test results

[Paste the testing protocol here]
```

---

## Quick Reference: Most Common Issues This Catches

| Issue | Protocol Section | Test That Catches It |
|-------|-----------------|---------------------|
| Empty list pages | 3.2 | "Data is displayed (NOT EMPTY)" |
| Login doesn't work | 2.2 | "Submit with correct credentials" |
| Empty dropdowns | 3.4 | "Dropdowns are populated (NOT EMPTY)" |
| Forms don't submit | 3.4 | "Submit with valid data" |
| No seed data | 1.2 | "EVERY table has at least 5 records" |
| Broken navigation | 3.1 | "Every menu item is clickable" |
| Edit form loads empty | 3.5 | "Form loads with existing data" |
| API errors on page load | 1.3 | "Endpoint is reachable" |
| Unstyled/broken UI | 4.1 | "Visual Consistency" checks |

---



PART 2 - DO NOT MISS

# Claude Prompt: Ultimate Application Quality Assurance & UI/UX Mastery Protocol

This is the definitive testing and quality assurance protocol that combines **Senior UI/UX Design Expertise**, **Ruthless Quality Assurance Testing**, and **Technical Validation** into one comprehensive system.

---

## The Prompt

````markdown
# Ultimate Application Quality Assurance & UI/UX Mastery Protocol
## The Definitive Pre-Delivery Checklist for AI-Built Applications

---

## YOUR ROLES

You must wear THREE hats simultaneously when building and validating applications:

### 🎨 HAT 1: Senior UI/UX Designer (15+ Years Experience)
You have designed enterprise applications for Fortune 500 companies. You understand:
- Visual hierarchy and information architecture
- Cognitive load reduction
- Accessibility standards (WCAG 2.1 AA)
- Modern design patterns and anti-patterns
- Typography, spacing, color theory
- User psychology and behavior patterns
- Mobile-first responsive design
- Micro-interactions and feedback loops

**Your standard:** "Would I be proud to show this in my portfolio?"

### 🔬 HAT 2: Senior QA Engineer (Ruthless Critic)
You have broken countless applications. You test:
- Every click, every input, every edge case
- What happens when things go wrong
- What users actually do (not what they should do)
- Performance under stress
- Security vulnerabilities
- Data integrity across operations

**Your standard:** "If there's a way to break it, I will find it."

### ⚙️ HAT 3: Technical Architect
You ensure the application is:
- Properly structured
- Following best practices
- Performant and scalable
- Secure and maintainable

**Your standard:** "This code should pass a senior developer's code review."

---

## CRITICAL INSTRUCTION

**The application is NOT complete until it passes EVERY section of this protocol with a score of 90% or higher.**

You must:
1. Build the application
2. Run this ENTIRE protocol
3. Score each section honestly
4. Fix ALL critical and major issues
5. Re-test until quality standards are met
6. Document everything
7. Only THEN deliver

**NEVER DELIVER an application that:**
- Has empty pages or missing data
- Has broken functionality
- Looks unprofessional or amateur
- Has poor typography or spacing
- Wastes screen space
- Confuses users about what to do
- Has inconsistent design patterns

---

# PART 1: UI/UX DESIGN EVALUATION

## 1.1 First Impressions Audit (The 5-Second Test)

When a user first sees each page, evaluate:

```
Page: [Page Name]

□ CLARITY: Can users understand what this page is for within 5 seconds?
  Score: [ ] Immediately clear / [ ] Somewhat clear / [ ] Confusing
  Issues: [Document any confusion points]

□ HIERARCHY: Is the most important element the most prominent?
  - What SHOULD be most important: [Identify]
  - What IS most prominent: [Observe]
  - Match: [ ] Yes / [ ] No
  
□ ACTION CLARITY: Is it obvious what the user should do?
  - Primary action identified: [ ] Yes / [ ] No
  - Primary action is visually prominent: [ ] Yes / [ ] No
  
□ PROFESSIONAL APPEARANCE: Does it look like a real product?
  - Would pass as commercial software: [ ] Yes / [ ] No
  - Feels amateur or unfinished: [ ] Yes / [ ] No
  
FIRST IMPRESSION SCORE: ___/10
```

## 1.2 Visual Hierarchy & Information Architecture

### 1.2.1 Page Structure Analysis

For EVERY page, evaluate:

```
Page: [Page Name]

LAYOUT STRUCTURE:
□ Uses clear visual zones (header, content, sidebar, footer)
□ Content is logically grouped
□ Related items are visually connected
□ Whitespace creates breathing room
□ No orphaned elements floating randomly

INFORMATION PRIORITY:
□ Most important info is "above the fold"
□ Secondary info is accessible but not competing
□ Tertiary info is available on demand (expandable, tabs, etc.)

VISUAL FLOW:
□ Eye naturally moves in logical pattern (Z or F pattern for Western)
□ No visual "traps" that confuse the eye
□ Clear start and end points

SCREEN REAL ESTATE:
□ Space is used efficiently (not cramped, not wasteful)
□ Content area is appropriately sized
□ Sidebars/panels are justified (not empty decoration)

STRUCTURE SCORE: ___/10

Issues to fix:
- [List specific problems]
```

### 1.2.2 Component Placement Evaluation

```
HEADER/TOP BAR:
□ Contains: Logo/brand, primary navigation, user menu, search (if applicable)
□ Height is proportional (not too tall, not cramped)
□ Stays consistent across all pages
□ Important actions are accessible
Rating: ___/5

NAVIGATION:
□ Location is conventional (top, left sidebar, or both)
□ Current location is clearly indicated
□ Hierarchy is clear (parent > child relationships)
□ Icons have labels (not icon-only unless universal)
□ Collapsed state works well (if applicable)
Rating: ___/5

MAIN CONTENT AREA:
□ Takes appropriate portion of screen (typically 60-80%)
□ Has clear boundaries
□ Content doesn't feel lost in space
□ Maximum line length is readable (50-75 characters)
Rating: ___/5

SIDEBARS/PANELS (if present):
□ Serve a clear purpose
□ Don't compete with main content
□ Can be collapsed if not essential
□ Content is relevant to current context
Rating: ___/5

FOOTER (if present):
□ Contains appropriate content (links, copyright, etc.)
□ Doesn't waste excessive space
□ Accessible but not distracting
Rating: ___/5
```

## 1.3 Typography Evaluation

### 1.3.1 Font Selection & Usage

```
PRIMARY FONT:
- Font family: [Identify]
- Appropriate for application type: [ ] Yes / [ ] No
- Readable at all sizes: [ ] Yes / [ ] No
- Web-safe or properly loaded: [ ] Yes / [ ] No

FONT HIERARCHY:
| Element | Size | Weight | Appropriate? |
|---------|------|--------|--------------|
| H1 (Page titles) | [Size] | [Weight] | [ ] |
| H2 (Section headers) | [Size] | [Weight] | [ ] |
| H3 (Subsections) | [Size] | [Weight] | [ ] |
| Body text | [Size] | [Weight] | [ ] |
| Small/caption text | [Size] | [Weight] | [ ] |
| Button text | [Size] | [Weight] | [ ] |
| Form labels | [Size] | [Weight] | [ ] |
| Input text | [Size] | [Weight] | [ ] |

TYPOGRAPHY RULES CHECK:
□ Clear size progression (each level noticeably different)
□ No more than 3 font weights used
□ Consistent sizing throughout application
□ Body text is minimum 16px (or equivalent)
□ Line height is 1.4-1.6 for body text
□ Letter spacing is appropriate
□ No ALL CAPS for long text
□ Sufficient contrast for readability

COMMON TYPOGRAPHY SINS (check if present):
[ ] Too many font sizes (chaos)
[ ] Text too small to read comfortably
[ ] Lines too long (hard to track)
[ ] Lines too short (choppy reading)
[ ] Insufficient line height (cramped)
[ ] Low contrast text
[ ] Centered body text (hard to read)
[ ] Justified text with rivers of white space

TYPOGRAPHY SCORE: ___/10

Fixes needed:
- [List specific typography issues]
```

### 1.3.2 Text Content Quality

```
□ Headings are descriptive (not "Page 1" or "Section")
□ Labels are clear and concise
□ Instructions are helpful
□ Error messages are human-readable
□ Empty states have helpful text
□ Button labels describe the action
□ No placeholder text left in ("Lorem ipsum", "TODO")
□ Consistent terminology throughout
□ No spelling or grammar errors
□ Appropriate tone (professional, friendly, etc.)

CONTENT QUALITY SCORE: ___/10
```

## 1.4 Color & Visual Design Evaluation

### 1.4.1 Color System Analysis

```
COLOR PALETTE IDENTIFICATION:
- Primary color: [Color + hex]
- Secondary color: [Color + hex]
- Accent color: [Color + hex]
- Success color: [Color + hex]
- Warning color: [Color + hex]
- Error color: [Color + hex]
- Neutral colors: [Range]
- Background color: [Color + hex]
- Text color: [Color + hex]

COLOR USAGE EVALUATION:
□ Primary color used for primary actions/branding
□ Colors are used consistently (same meaning throughout)
□ Not too many colors (max 5-6 plus neutrals)
□ Colors work together harmoniously
□ Sufficient contrast for accessibility
□ Color is not the ONLY indicator (shapes, icons also used)

CONTRAST CHECKS (WCAG AA minimum):
| Element | Foreground | Background | Ratio | Pass? |
|---------|------------|------------|-------|-------|
| Body text | [Color] | [Color] | [Ratio] | [ ] 4.5:1+ |
| Large text | [Color] | [Color] | [Ratio] | [ ] 3:1+ |
| UI components | [Color] | [Color] | [Ratio] | [ ] 3:1+ |
| Form inputs | [Color] | [Color] | [Ratio] | [ ] |
| Links | [Color] | [Color] | [Ratio] | [ ] |

COLOR PSYCHOLOGY CHECK:
□ Colors appropriate for industry/purpose
□ Error states feel urgent (reds/oranges)
□ Success states feel positive (greens)
□ Warnings feel cautionary (yellows/oranges)
□ Primary actions stand out

COLOR SCORE: ___/10
```

### 1.4.2 Visual Polish & Details

```
SHADOWS & DEPTH:
□ Consistent shadow style throughout
□ Shadows indicate elevation logically
□ Not too harsh or too subtle
□ Cards/modals have appropriate depth

BORDERS & DIVIDERS:
□ Used purposefully (not everywhere)
□ Consistent style (color, weight)
□ Subtle, not overwhelming
□ Create visual separation where needed

ICONS:
□ Consistent style (outline, filled, etc.)
□ Consistent size
□ Meaningful and recognizable
□ Have accessible labels/tooltips
□ Not overused

IMAGES (if applicable):
□ Appropriate resolution
□ Consistent style/treatment
□ Properly sized (not stretched/squished)
□ Have alt text for accessibility

MICRO-DETAILS:
□ Rounded corners are consistent
□ Spacing follows a system (4px, 8px, 16px, etc.)
□ Alignment is precise (no 1px off-ness)
□ Interactive elements have hover/focus states

VISUAL POLISH SCORE: ___/10
```

## 1.5 Space Utilization & Layout Efficiency

### 1.5.1 Screen Real Estate Audit

```
DESKTOP VIEW (1920x1080):

□ Content doesn't stretch edge-to-edge uncomfortably
□ Maximum content width is set appropriately (1200-1400px typical)
□ Extra space on large screens is handled gracefully
□ No vast empty areas that feel wasteful
□ No cramped areas that feel overwhelming

MEASUREMENT CHECK:
- Header height: [px] - Appropriate? [ ]
- Sidebar width: [px] - Appropriate? [ ]
- Content area width: [px] - Appropriate? [ ]
- Spacing between elements: Consistent? [ ]

SPACE EFFICIENCY RATING:
[ ] Excellent - Every pixel has purpose
[ ] Good - Minor areas could be optimized
[ ] Fair - Some wasted or cramped space
[ ] Poor - Significant layout issues
```

### 1.5.2 Smart Space Solutions

Evaluate if these patterns are used appropriately:

```
TABS:
□ Used to organize related content that doesn't need simultaneous view
□ Tab labels are clear and concise
□ Active tab is clearly indicated
□ Tab content loads smoothly
□ Not overused (max 5-7 tabs typically)
Where tabs are used: [List pages]
Where tabs SHOULD be used: [Suggestions]

ACCORDIONS/COLLAPSIBLES:
□ Used for secondary content users may not need
□ Clear expand/collapse indicators
□ Smooth animation
□ Remember state where appropriate
Where used: [List]
Where SHOULD be used: [Suggestions]

MODALS/DIALOGS:
□ Used for focused tasks that need attention
□ Not overused (not modal-in-modal)
□ Can be closed easily
□ Don't contain too much content
□ Proper focus management
Where used: [List]
Appropriate usage: [ ] Yes / [ ] No

SPLIT VIEWS/MASTER-DETAIL:
□ Used when viewing list + detail simultaneously helps
□ Proportions are appropriate
□ Resizable if appropriate
Where applicable: [Identify opportunities]

DRAWERS/SLIDE-OUTS:
□ Used for supplementary content
□ Don't block critical information
□ Easy to dismiss
Where used: [List]

CARDS:
□ Used to group related information
□ Consistent card styling
□ Appropriate information density
□ Clear hierarchy within cards
Where used: [List]
Effective usage: [ ] Yes / [ ] No

SPACE UTILIZATION SCORE: ___/10
```

## 1.6 Navigation & Wayfinding

### 1.6.1 Navigation Structure Evaluation

```
PRIMARY NAVIGATION:
□ Contains all main sections
□ Logical grouping of items
□ Order makes sense (most used first, or logical flow)
□ Not too many items (7±2 rule)
□ Icons aid recognition (not just decoration)

CURRENT LOCATION INDICATORS:
□ User always knows where they are
□ Active state is visually distinct
□ Breadcrumbs present for deep hierarchies
□ Page titles match navigation labels

NAVIGATION PATTERNS:
| Pattern | Present? | Effective? |
|---------|----------|------------|
| Top navigation bar | [ ] | [ ] |
| Left sidebar | [ ] | [ ] |
| Breadcrumbs | [ ] | [ ] |
| Tab navigation | [ ] | [ ] |
| Pagination | [ ] | [ ] |
| Back buttons | [ ] | [ ] |
| Search | [ ] | [ ] |

NAVIGATION ACCESSIBILITY:
□ Keyboard navigable
□ Focus states visible
□ Skip links present
□ Mobile menu works

NAVIGATION SCORE: ___/10
```

### 1.6.2 User Journey Flow

```
For each major user task, trace the journey:

TASK: [e.g., "Create a new customer"]
Steps required:
1. [Step] - Obvious? [ ] Yes / [ ] No
2. [Step] - Obvious? [ ] Yes / [ ] No
3. [Step] - Obvious? [ ] Yes / [ ] No

Friction points identified:
- [List any confusing or unnecessary steps]

Suggested improvements:
- [How to streamline]

TASK: [Next major task]
[Repeat analysis]

OVERALL JOURNEY SCORE: ___/10
```

## 1.7 Form Design Excellence

### 1.7.1 Form Layout & Structure

```
For EVERY form in the application:

FORM: [Form Name]

LAYOUT:
□ Single column layout (preferred for most forms)
□ Logical field grouping
□ Related fields are visually connected
□ Sections have clear headers
□ Appropriate form length (or broken into steps)

LABELS:
□ Every field has a label
□ Labels are above fields (preferred) or clearly associated
□ Labels are concise but descriptive
□ Required fields are indicated (asterisk or text)
□ Optional fields are indicated (if minority)

FIELD SIZING:
□ Field width matches expected input
  - Short answers (names): Medium width
  - Long answers (descriptions): Full width
  - Numbers: Narrow width
  - Emails: Medium-wide width
□ All fields align consistently
□ Touch targets are large enough (min 44px)

HELP TEXT:
□ Complex fields have helper text
□ Format requirements are shown (date format, etc.)
□ Character limits shown where relevant
□ Help text doesn't clutter simple fields

FORM LAYOUT SCORE: ___/10
```

### 1.7.2 Form Interaction Design

```
INPUT TYPES:
□ Correct input type for data (email, tel, number, date)
□ Dropdowns for limited choices (< 10 options)
□ Radio buttons for mutually exclusive choices (< 5 options)
□ Checkboxes for multiple selections
□ Text areas for long text
□ Date pickers for dates
□ File upload styled appropriately

INTERACTION STATES:
| State | Visually Distinct? | Example Field |
|-------|-------------------|---------------|
| Default | [ ] | [Check] |
| Focus | [ ] | [Check] |
| Filled | [ ] | [Check] |
| Error | [ ] | [Check] |
| Disabled | [ ] | [Check] |
| Read-only | [ ] | [Check] |

VALIDATION FEEDBACK:
□ Real-time validation (as user types/leaves field)
□ Error messages appear near the field
□ Error messages explain how to fix
□ Success indicators where helpful
□ Form-level error summary for long forms
□ Focus moves to first error on submit

SMART DEFAULTS:
□ Sensible defaults where appropriate
□ Today's date for date fields (if applicable)
□ User's info pre-filled (if known)
□ Most common option pre-selected (if appropriate)

FORM INTERACTION SCORE: ___/10
```

## 1.8 Data Display Excellence

### 1.8.1 Table/List Design

```
For EVERY data table/list:

TABLE: [Table Name] on [Page]

COLUMN DESIGN:
□ Column headers are clear and concise
□ Important columns are first (left side)
□ Column widths match content needs
□ Numeric data is right-aligned
□ Action columns are last (right side)
□ No unnecessary columns

DATA PRESENTATION:
□ Dates are formatted consistently and readably
□ Numbers are formatted (thousands separators, decimals)
□ Currency shows symbol and proper format
□ Long text is truncated with ellipsis (tooltip for full)
□ Status uses visual indicators (badges, colors)
□ Empty cells show dash or "N/A" (not blank)

TABLE FUNCTIONALITY:
□ Sortable columns are indicated
□ Current sort is shown
□ Filterable where appropriate
□ Search works across visible columns
□ Pagination or infinite scroll for large data sets
□ Row selection where appropriate
□ Bulk actions where appropriate

ROW DENSITY:
□ Appropriate padding (not cramped, not wasteful)
□ Row hover state for clickable rows
□ Zebra striping or dividers for readability
□ Consistent row height

RESPONSIVE BEHAVIOR:
□ Horizontal scroll for wide tables (contained)
□ Or columns stack/hide appropriately
□ Key columns always visible

TABLE DESIGN SCORE: ___/10
```

### 1.8.2 Card & Grid Layouts

```
If cards/grids are used:

CARD DESIGN:
□ Clear visual boundaries
□ Consistent card sizes (or intentional variation)
□ Content hierarchy within cards is clear
□ Interactive cards have hover states
□ Cards aren't overloaded with information
□ Actions are appropriately placed

GRID LAYOUT:
□ Consistent gutters between items
□ Responsive columns (4 → 2 → 1 as screen shrinks)
□ Items align properly
□ No orphaned single items on rows

CARD/GRID SCORE: ___/10
```

### 1.8.3 Detail Page Design

```
For EVERY detail/view page:

PAGE: [Entity] Detail Page

INFORMATION ORGANIZATION:
□ Most important info is prominent
□ Related info is grouped logically
□ Clear section headers
□ Appropriate use of tabs (if complex)
□ Related entities are accessible

FIELD DISPLAY:
□ Labels are clearly associated with values
□ Empty fields handled gracefully (show "Not provided" vs hiding)
□ Values are formatted appropriately
□ Long text is readable (proper line length)

ACTIONS:
□ Edit action is easily accessible
□ Delete has confirmation
□ Related actions are grouped
□ Primary action is prominent

DETAIL PAGE SCORE: ___/10
```

## 1.9 Feedback & Communication

### 1.9.1 System Feedback

```
LOADING STATES:
□ Spinner or skeleton while loading
□ Clear indication something is happening
□ Appropriate for duration (spinner < 3s, progress bar > 3s)
□ No empty flashes before content
Loading states present for:
- [ ] Page loads
- [ ] Data fetches
- [ ] Form submissions
- [ ] File uploads

SUCCESS FEEDBACK:
□ Clear confirmation after actions
□ Toast/snackbar for minor successes
□ Success page/message for major completions
□ Appropriate duration (not too fast, not too slow)

ERROR FEEDBACK:
□ Errors are visible and clear
□ Explain what went wrong
□ Suggest how to fix
□ Don't blame the user
□ Persistent until dismissed (not auto-hide errors)

EMPTY STATES:
□ Every possible empty state has a design
□ Explains why it's empty
□ Suggests next action
□ Has visual interest (illustration/icon)

Empty states needed for:
- [ ] No search results
- [ ] Empty list (no records yet)
- [ ] No permissions
- [ ] Error state (failed to load)

FEEDBACK SCORE: ___/10
```

### 1.9.2 Micro-Interactions

```
BUTTON FEEDBACK:
□ Hover state on all buttons
□ Active/pressed state
□ Disabled state when appropriate
□ Loading state for async actions

LINK FEEDBACK:
□ Hover state (underline or color change)
□ Visited state (if applicable)
□ Focus state for keyboard

INPUT FEEDBACK:
□ Focus state clearly visible
□ Character count where relevant
□ Real-time validation feedback
□ Autocomplete where helpful

TRANSITIONS:
□ Smooth transitions (not jarring)
□ Appropriate duration (200-300ms typical)
□ Purposeful (not just decoration)
□ Respect prefers-reduced-motion

MICRO-INTERACTION SCORE: ___/10
```

## 1.10 Responsive Design Mastery

### 1.10.1 Breakpoint Testing

```
Test at these exact widths:

DESKTOP LARGE (1920px):
□ Layout uses space well
□ Content isn't stretched uncomfortably
□ No horizontal scroll
Screenshot observation: [Notes]

DESKTOP (1440px):
□ Primary layout intact
□ All features accessible
Screenshot observation: [Notes]

LAPTOP (1024px):
□ Layout adjusts appropriately
□ Sidebar may collapse
□ Tables may scroll
Screenshot observation: [Notes]

TABLET (768px):
□ Single column layout where appropriate
□ Navigation collapses to hamburger
□ Touch-friendly tap targets
□ Tables adapt or scroll
Screenshot observation: [Notes]

MOBILE (375px):
□ All content accessible
□ No horizontal scroll
□ Readable text (no zooming needed)
□ Buttons are finger-friendly (44px min)
□ Forms are usable
□ Modals work properly
Screenshot observation: [Notes]

RESPONSIVE SCORE: ___/10
```

### 1.10.2 Responsive Component Behavior

```
| Component | Desktop | Tablet | Mobile | Works? |
|-----------|---------|--------|--------|--------|
| Header | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Navigation | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Data tables | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Forms | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Cards/Grid | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Modals | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Images | [Behavior] | [Behavior] | [Behavior] | [ ] |
```

## 1.11 Accessibility Audit

### 1.11.1 WCAG 2.1 AA Compliance Check

```
PERCEIVABLE:
□ All images have alt text
□ Color is not only indicator
□ Contrast ratios meet minimums
□ Text can be resized to 200%
□ No content only in images of text

OPERABLE:
□ All functionality available by keyboard
□ No keyboard traps
□ Skip links present
□ Focus is visible and logical
□ No time limits (or extendable)
□ No content that flashes

UNDERSTANDABLE:
□ Language is specified
□ Navigation is consistent
□ Error identification is clear
□ Labels and instructions provided

ROBUST:
□ Valid HTML
□ Name, role, value available
□ Status messages announced

ACCESSIBILITY SCORE: ___/10
```

### 1.11.2 Keyboard Navigation Test

```
Tab through entire application:

□ Can reach all interactive elements
□ Tab order is logical (follows visual order)
□ Focus indicator is always visible
□ Can activate all buttons/links with Enter
□ Can close modals with Escape
□ Can navigate dropdowns with arrows
□ No keyboard traps

Issues found:
- [List any keyboard issues]
```

---

# PART 2: FUNCTIONAL TESTING (QA ENGINEER HAT)

## 2.1 Foundation Checks

### 2.1.1 Application Startup

```
□ Backend starts without errors
  Command: [Command used]
  Result: [ ] Success / [ ] Errors (list below)
  Errors: [Document any]

□ Frontend starts without errors
  Command: [Command used]
  Result: [ ] Success / [ ] Errors (list below)
  Errors: [Document any]

□ Application loads in browser
  URL: [URL]
  Result: [ ] Loads / [ ] Fails
  Console errors: [ ] None / [ ] Present (list below)
  Errors: [Document any]

□ API is accessible from frontend
  Test endpoint: [Endpoint]
  Result: [ ] Success / [ ] CORS error / [ ] Other error

STARTUP SCORE: ___/10
```

### 2.1.2 Database & Seed Data

```
□ Database connection works
□ All migrations applied
□ Seed data script runs successfully
□ All tables have data

SEED DATA VERIFICATION:
| Table/Collection | Expected Records | Actual Records | Has Variety? |
|------------------|------------------|----------------|--------------|
| Users | 3+ | [Count] | [ ] |
| [Entity 1] | 5+ | [Count] | [ ] |
| [Entity 2] | 5+ | [Count] | [ ] |
| [Entity 3] | 5+ | [Count] | [ ] |
| [Lookup data] | All options | [Count] | [ ] |

SEED DATA QUALITY:
□ Realistic names (not "test", "asdf")
□ Realistic emails (@example.com for safety)
□ Various statuses represented
□ Various dates (past, recent, future where appropriate)
□ Valid relationships (FKs exist)
□ Edge cases included (overdue, expired, etc.)

DATABASE SCORE: ___/10
```

## 2.2 Authentication Testing

### 2.2.1 Registration (If Applicable)

```
□ Navigate to registration: [ ] Works / [ ] Broken
□ All required fields present: [ ] Yes / [ ] Missing: ______
□ Submit empty form: [ ] Validation errors / [ ] Submits anyway
□ Submit invalid email: [ ] Shows error / [ ] Accepts
□ Submit weak password: [ ] Shows error / [ ] Accepts
□ Submit valid data: [ ] Creates account / [ ] Fails
□ Duplicate email: [ ] Shows error / [ ] Creates duplicate

Test credentials created:
- Email: [Email used]
- Password: [Password used]

REGISTRATION SCORE: ___/10
```

### 2.2.2 Login Testing

```
□ Navigate to login: [ ] Works / [ ] Broken
□ Form displays correctly: [ ] Yes / [ ] Issues: ______
□ Empty submission: [ ] Validation / [ ] Error
□ Wrong credentials: [ ] Generic error / [ ] Reveals too much
□ Correct credentials: [ ] Logs in / [ ] Fails

Seeded credentials used:
- Email: [Email]
- Password: [Password]
- Result: [ ] Success / [ ] Failure

Post-login checks:
□ Redirected to correct page
□ User info displayed (name, avatar, etc.)
□ Protected pages now accessible
□ Login page redirects to dashboard if already logged in
□ Session persists on refresh

LOGIN SCORE: ___/10
```

### 2.2.3 Logout Testing

```
□ Logout button accessible: [ ] Yes / [ ] No
□ Click logout: [ ] Logs out / [ ] Nothing happens
□ Redirected appropriately: [ ] Yes / [ ] No
□ Protected pages blocked after logout: [ ] Yes / [ ] No
□ Back button doesn't reveal protected content: [ ] Secure / [ ] Insecure

LOGOUT SCORE: ___/10
```

## 2.3 Page-by-Page Functional Testing

### 2.3.1 List Page Testing Template

For EVERY list/table page:

```
PAGE: [Page Name]
URL: [URL]

LOAD TEST:
□ Page loads without errors
□ Data displays (NOT EMPTY)
□ Correct columns shown
□ Data formatted correctly

Record count: [Number displayed]
Expected: [Should match seed data]

If empty, STOP and fix:
□ Check API endpoint
□ Check frontend API call
□ Check data binding
□ Add seed data if missing

INTERACTION TESTS:
□ Row click/view works
□ Edit button works
□ Delete button works (with confirmation)
□ Sort works (if present)
□ Search works (if present)
□ Filter works (if present)
□ Pagination works (if present)

EDGE CASES:
□ Search with no results shows message
□ Filter with no results shows message
□ Very long content truncates gracefully

PAGE SCORE: ___/10
```

### 2.3.2 Create Form Testing Template

For EVERY create form:

```
FORM: Create [Entity]
URL: [URL]

DISPLAY TEST:
□ Form displays
□ All fields present
□ Labels are correct
□ Required indicators present
□ Dropdowns populated (NOT EMPTY)

DROPDOWN VERIFICATION:
| Dropdown | Options Expected | Options Present | Correct? |
|----------|------------------|-----------------|----------|
| [Drop 1] | [List] | [Actual] | [ ] |
| [Drop 2] | [List] | [Actual] | [ ] |

VALIDATION TESTS:
| Test | Field | Input | Expected | Actual | Pass? |
|------|-------|-------|----------|--------|-------|
| Empty required | [Field] | (empty) | Error | | [ ] |
| Invalid format | Email | "notanemail" | Error | | [ ] |
| Too long | Name | [255+ chars] | Error | | [ ] |
| Invalid number | Amount | "abc" | Error | | [ ] |

SUBMISSION TESTS:
□ Valid data submits successfully
□ Success message shown
□ Redirected appropriately
□ Record appears in list
□ All data saved correctly

Test data used:
| Field | Value |
|-------|-------|
| [Field 1] | [Value] |
| [Field 2] | [Value] |

FORM SCORE: ___/10
```

### 2.3.3 Edit Form Testing Template

For EVERY edit form:

```
FORM: Edit [Entity]
URL Pattern: [URL/:id]
Test Record: [ID from seed data]

LOAD TEST:
□ Form loads with existing data
□ All fields pre-populated
□ Dropdown shows current selection
□ Dates formatted correctly

PRE-POPULATED DATA CHECK:
| Field | Expected (from DB) | Shows in Form | Match? |
|-------|-------------------|---------------|--------|
| [Field 1] | [Value] | [Value] | [ ] |
| [Field 2] | [Value] | [Value] | [ ] |

If form loads empty, STOP and fix:
□ Check API endpoint returns data
□ Check form fetches on load
□ Check data binds to fields

EDIT TESTS:
□ Change field and save
□ Changes persist on reload
□ Validation still works
□ Cancel discards changes

FORM SCORE: ___/10
```

### 2.3.4 Detail Page Testing Template

For EVERY detail/view page:

```
PAGE: [Entity] Detail
URL Pattern: [URL/:id]
Test Record: [ID from seed data]

DISPLAY TEST:
□ Page loads with data
□ All fields display values
□ Related data displays
□ Formatted correctly (dates, currency, etc.)

FIELD VERIFICATION:
| Field | Expected Value | Displayed Value | Correct? |
|-------|----------------|-----------------|----------|
| [Field 1] | [Value] | [Value] | [ ] |
| [Field 2] | [Value] | [Value] | [ ] |

NAVIGATION:
□ Edit button works
□ Delete button works
□ Back/return works
□ Related links work

ERROR HANDLING:
□ Invalid ID shows 404/not found
□ Graceful error message

PAGE SCORE: ___/10
```

## 2.4 CRUD Cycle Verification

For each main entity, complete full cycle:

```
ENTITY: [Entity Name]

CREATE:
□ Navigate to create form
□ Fill in all fields
□ Submit successfully
□ Record created with ID: [ID]

READ:
□ View record in list
□ View record detail page
□ All data displays correctly

UPDATE:
□ Open edit form
□ Data pre-populated correctly
□ Change [Field] to [New Value]
□ Save successfully
□ Change persisted

DELETE:
□ Click delete
□ Confirmation appears
□ Confirm deletion
□ Record removed from list
□ Record not accessible

CRUD CYCLE: [ ] COMPLETE / [ ] FAILED at step: ______
```

## 2.5 Edge Case & Error Testing

### 2.5.1 Error Handling

```
□ 404 page exists and is styled
□ API errors show user-friendly message
□ Network failure handled gracefully
□ Server error (500) shows appropriate message
□ No raw error messages or stack traces exposed

ERROR HANDLING SCORE: ___/10
```

### 2.5.2 Edge Cases

```
□ Empty search returns helpful message
□ Extremely long text input handled
□ Special characters in input handled
□ Rapid clicking doesn't cause issues
□ Double-submit prevention works
□ Concurrent edit handling (if applicable)
□ Browser refresh during operation

EDGE CASE SCORE: ___/10
```

---

# PART 3: TECHNICAL QUALITY AUDIT

## 3.1 Code Quality Check

### 3.1.1 Backend Quality

```
STRUCTURE:
□ Logical folder organization
□ Separation of concerns
□ Consistent naming conventions
□ No commented-out code
□ No unused imports

ERROR HANDLING:
□ Try-catch where appropriate
□ Meaningful error messages
□ Proper HTTP status codes
□ Validation on all inputs

SECURITY:
□ Input sanitization
□ SQL injection prevention (parameterized queries)
□ Authentication on protected routes
□ No sensitive data in responses
□ CORS properly configured

BACKEND SCORE: ___/10
```

### 3.1.2 Frontend Quality

```
STRUCTURE:
□ Component organization logical
□ Reusable components extracted
□ Consistent naming
□ No duplicate code
□ State management appropriate

ERROR HANDLING:
□ API errors caught and displayed
□ Loading states implemented
□ Empty states handled
□ Form validation implemented

PERFORMANCE:
□ No unnecessary re-renders
□ Large lists virtualized/paginated
□ Images optimized
□ No memory leaks on navigation

FRONTEND SCORE: ___/10
```

## 3.2 Console & Network Check

```
BROWSER CONSOLE:
□ No JavaScript errors during normal use
□ No warnings (or acceptable/expected only)
□ No failed network requests

NETWORK TAB:
□ API calls return expected status codes
□ No CORS errors
□ Reasonable response times
□ No excessive calls

CONSOLE SCORE: ___/10
```

---

# PART 4: SCORING & DELIVERY DECISION

## 4.1 Score Summary

```
SECTION SCORES:

UI/UX DESIGN:
- First Impressions: ___/10
- Visual Hierarchy: ___/10
- Typography: ___/10
- Color & Visual: ___/10
- Space Utilization: ___/10
- Navigation: ___/10
- Form Design: ___/10
- Data Display: ___/10
- Feedback: ___/10
- Responsive: ___/10
- Accessibility: ___/10
UI/UX SUBTOTAL: ___/110 = ___%

FUNCTIONAL TESTING:
- Foundation: ___/10
- Authentication: ___/10
- List Pages: ___/10
- Create Forms: ___/10
- Edit Forms: ___/10
- Detail Pages: ___/10
- CRUD Cycles: ___/10
- Error Handling: ___/10
FUNCTIONAL SUBTOTAL: ___/80 = ___%

TECHNICAL QUALITY:
- Backend Quality: ___/10
- Frontend Quality: ___/10
- Console/Network: ___/10
TECHNICAL SUBTOTAL: ___/30 = ___%

═══════════════════════════════
OVERALL SCORE: ___/220 = ___%
═══════════════════════════════
```

## 4.2 Delivery Decision

```
CRITICAL FAILURES (Any = Cannot Deliver):
□ Empty pages with no data
□ Login doesn't work
□ Any CRUD operation fails
□ Empty dropdowns
□ Console errors during normal use
□ Professional appearance failure

MAJOR ISSUES (Must fix before delivery):
- [List issues scoring below 6/10]

MINOR ISSUES (Document, fix if time):
- [List cosmetic or enhancement items]

DELIVERY DECISION:
[ ] ✅ READY TO DELIVER (Score ≥ 90%, no critical failures)
[ ] ⚠️ NEEDS FIXES (Score 70-89%, or critical failures)
[ ] ❌ NOT READY (Score < 70%, multiple critical failures)
```

## 4.3 Required Documentation

When delivering, include:

```markdown
## Application Delivery Package

### 1. Startup Instructions
[Exact commands to run backend, frontend, database]

### 2. Environment Setup
[All env variables needed]

### 3. Test Credentials
| Role | Email | Password |
|------|-------|----------|
| Admin | [email] | [password] |
| User | [email] | [password] |

### 4. Seed Data Summary
[What data was seeded, how to re-seed]

### 5. Quality Report
Overall Score: ___%

Passed Tests: [List]
Known Limitations: [List]
Future Improvements: [List]

### 6. Pages Inventory
| Page | URL | Status |
|------|-----|--------|
| Dashboard | /dashboard | ✅ Working |
| Customers | /customers | ✅ Working |
| ... | ... | ... |
```

---

# HOW TO USE THIS PROTOCOL

## For Every Application Build Request:

1. **Build** the application per requirements
2. **Stop** before declaring it done
3. **Run** this ENTIRE protocol
4. **Score** honestly in each section
5. **Fix** all critical and major issues
6. **Re-test** affected areas
7. **Document** results
8. **Deliver** only when quality standards met

## Red Flags That Mean "Not Done":

🚩 "The data will appear when you add records" - NO, seed the data
🚩 "The styling can be improved later" - NO, meet minimum standards
🚩 "Login should work with your credentials" - NO, test it yourself
🚩 Empty tables, dropdowns, or pages - NEVER acceptable
🚩 Console errors - NEVER acceptable
🚩 "I didn't test on mobile" - Test it

## Quality Standards:

- **Minimum acceptable score: 90%**
- **Zero critical failures**
- **All CRUD operations working**
- **Professional appearance**
- **Seed data in place**
- **Documentation complete**

---

**Remember: An application that looks amateur, has empty pages, or doesn't function completely is NOT FINISHED. The user should be able to use it immediately upon delivery without any additional setup or fixes.**
````

---

## Quick Reference Card

### Most Critical Checks (Never Skip)

| Check | Why It Matters |
|-------|----------------|
| Seed data populated | Empty pages = broken app perception |
| Login works | Can't test anything without access |
| All dropdowns populated | Empty dropdowns = broken forms |
| CRUD operations work | Core functionality |
| No console errors | Indicates broken code |
| Professional appearance | Trust and usability |
| Typography readable | User can't use what they can't read |
| Responsive design | Users have different devices |

### Score Interpretation

| Score | Meaning | Action |
|-------|---------|--------|
| 95-100% | Excellent | Ship it! |
| 90-94% | Good | Minor polish, can ship |
| 80-89% | Needs Work | Fix major issues |
| 70-79% | Significant Issues | Major revision needed |
| <70% | Not Acceptable | Start over or major rework |

---

## How to Use This With Your Build Requests

```markdown
## My Request

Build me a [description] with these features:
- [Feature 1]
- [Feature 2]
- [Feature 3]

## MANDATORY QUALITY REQUIREMENT

You MUST complete the "Ultimate Application Quality Assurance & UI/UX Mastery Protocol" before delivering this application.

Minimum standards:
- Overall score of 90% or higher
- Zero critical failures
- Seed data in every table
- All pages show content
- Login works with provided credentials
- Professional, polished UI
- Responsive design
- No console errors

Do NOT tell me it's complete until you have tested everything yourself and documented the results.

[Paste the protocol here or reference it]
```

---

Would you like me to create specialized versions of this protocol for specific scenarios (like "mobile app testing" or "e-commerce specific testing"), or add any additional evaluation criteria?


# Claude Prompt: Ultimate Application Quality Assurance & UI/UX Mastery Protocol

This is the definitive testing and quality assurance protocol that combines **Senior UI/UX Design Expertise**, **Ruthless Quality Assurance Testing**, and **Technical Validation** into one comprehensive system.

---

## The Prompt

````markdown
# Ultimate Application Quality Assurance & UI/UX Mastery Protocol
## The Definitive Pre-Delivery Checklist for AI-Built Applications

---

## YOUR ROLES

You must wear THREE hats simultaneously when building and validating applications:

### 🎨 HAT 1: Senior UI/UX Designer (15+ Years Experience)
You have designed enterprise applications for Fortune 500 companies. You understand:
- Visual hierarchy and information architecture
- Cognitive load reduction
- Accessibility standards (WCAG 2.1 AA)
- Modern design patterns and anti-patterns
- Typography, spacing, color theory
- User psychology and behavior patterns
- Mobile-first responsive design
- Micro-interactions and feedback loops

**Your standard:** "Would I be proud to show this in my portfolio?"

### 🔬 HAT 2: Senior QA Engineer (Ruthless Critic)
You have broken countless applications. You test:
- Every click, every input, every edge case
- What happens when things go wrong
- What users actually do (not what they should do)
- Performance under stress
- Security vulnerabilities
- Data integrity across operations

**Your standard:** "If there's a way to break it, I will find it."

### ⚙️ HAT 3: Technical Architect
You ensure the application is:
- Properly structured
- Following best practices
- Performant and scalable
- Secure and maintainable

**Your standard:** "This code should pass a senior developer's code review."

---

## CRITICAL INSTRUCTION

**The application is NOT complete until it passes EVERY section of this protocol with a score of 90% or higher.**

You must:
1. Build the application
2. Run this ENTIRE protocol
3. Score each section honestly
4. Fix ALL critical and major issues
5. Re-test until quality standards are met
6. Document everything
7. Only THEN deliver

**NEVER DELIVER an application that:**
- Has empty pages or missing data
- Has broken functionality
- Looks unprofessional or amateur
- Has poor typography or spacing
- Wastes screen space
- Confuses users about what to do
- Has inconsistent design patterns

---

# PART 1: UI/UX DESIGN EVALUATION

## 1.1 First Impressions Audit (The 5-Second Test)

When a user first sees each page, evaluate:

```
Page: [Page Name]

□ CLARITY: Can users understand what this page is for within 5 seconds?
  Score: [ ] Immediately clear / [ ] Somewhat clear / [ ] Confusing
  Issues: [Document any confusion points]

□ HIERARCHY: Is the most important element the most prominent?
  - What SHOULD be most important: [Identify]
  - What IS most prominent: [Observe]
  - Match: [ ] Yes / [ ] No
  
□ ACTION CLARITY: Is it obvious what the user should do?
  - Primary action identified: [ ] Yes / [ ] No
  - Primary action is visually prominent: [ ] Yes / [ ] No
  
□ PROFESSIONAL APPEARANCE: Does it look like a real product?
  - Would pass as commercial software: [ ] Yes / [ ] No
  - Feels amateur or unfinished: [ ] Yes / [ ] No
  
FIRST IMPRESSION SCORE: ___/10
```

## 1.2 Visual Hierarchy & Information Architecture

### 1.2.1 Page Structure Analysis

For EVERY page, evaluate:

```
Page: [Page Name]

LAYOUT STRUCTURE:
□ Uses clear visual zones (header, content, sidebar, footer)
□ Content is logically grouped
□ Related items are visually connected
□ Whitespace creates breathing room
□ No orphaned elements floating randomly

INFORMATION PRIORITY:
□ Most important info is "above the fold"
□ Secondary info is accessible but not competing
□ Tertiary info is available on demand (expandable, tabs, etc.)

VISUAL FLOW:
□ Eye naturally moves in logical pattern (Z or F pattern for Western)
□ No visual "traps" that confuse the eye
□ Clear start and end points

SCREEN REAL ESTATE:
□ Space is used efficiently (not cramped, not wasteful)
□ Content area is appropriately sized
□ Sidebars/panels are justified (not empty decoration)

STRUCTURE SCORE: ___/10

Issues to fix:
- [List specific problems]
```

### 1.2.2 Component Placement Evaluation

```
HEADER/TOP BAR:
□ Contains: Logo/brand, primary navigation, user menu, search (if applicable)
□ Height is proportional (not too tall, not cramped)
□ Stays consistent across all pages
□ Important actions are accessible
Rating: ___/5

NAVIGATION:
□ Location is conventional (top, left sidebar, or both)
□ Current location is clearly indicated
□ Hierarchy is clear (parent > child relationships)
□ Icons have labels (not icon-only unless universal)
□ Collapsed state works well (if applicable)
Rating: ___/5

MAIN CONTENT AREA:
□ Takes appropriate portion of screen (typically 60-80%)
□ Has clear boundaries
□ Content doesn't feel lost in space
□ Maximum line length is readable (50-75 characters)
Rating: ___/5

SIDEBARS/PANELS (if present):
□ Serve a clear purpose
□ Don't compete with main content
□ Can be collapsed if not essential
□ Content is relevant to current context
Rating: ___/5

FOOTER (if present):
□ Contains appropriate content (links, copyright, etc.)
□ Doesn't waste excessive space
□ Accessible but not distracting
Rating: ___/5
```

## 1.3 Typography Evaluation

### 1.3.1 Font Selection & Usage

```
PRIMARY FONT:
- Font family: [Identify]
- Appropriate for application type: [ ] Yes / [ ] No
- Readable at all sizes: [ ] Yes / [ ] No
- Web-safe or properly loaded: [ ] Yes / [ ] No

FONT HIERARCHY:
| Element | Size | Weight | Appropriate? |
|---------|------|--------|--------------|
| H1 (Page titles) | [Size] | [Weight] | [ ] |
| H2 (Section headers) | [Size] | [Weight] | [ ] |
| H3 (Subsections) | [Size] | [Weight] | [ ] |
| Body text | [Size] | [Weight] | [ ] |
| Small/caption text | [Size] | [Weight] | [ ] |
| Button text | [Size] | [Weight] | [ ] |
| Form labels | [Size] | [Weight] | [ ] |
| Input text | [Size] | [Weight] | [ ] |

TYPOGRAPHY RULES CHECK:
□ Clear size progression (each level noticeably different)
□ No more than 3 font weights used
□ Consistent sizing throughout application
□ Body text is minimum 16px (or equivalent)
□ Line height is 1.4-1.6 for body text
□ Letter spacing is appropriate
□ No ALL CAPS for long text
□ Sufficient contrast for readability

COMMON TYPOGRAPHY SINS (check if present):
[ ] Too many font sizes (chaos)
[ ] Text too small to read comfortably
[ ] Lines too long (hard to track)
[ ] Lines too short (choppy reading)
[ ] Insufficient line height (cramped)
[ ] Low contrast text
[ ] Centered body text (hard to read)
[ ] Justified text with rivers of white space

TYPOGRAPHY SCORE: ___/10

Fixes needed:
- [List specific typography issues]
```

### 1.3.2 Text Content Quality

```
□ Headings are descriptive (not "Page 1" or "Section")
□ Labels are clear and concise
□ Instructions are helpful
□ Error messages are human-readable
□ Empty states have helpful text
□ Button labels describe the action
□ No placeholder text left in ("Lorem ipsum", "TODO")
□ Consistent terminology throughout
□ No spelling or grammar errors
□ Appropriate tone (professional, friendly, etc.)

CONTENT QUALITY SCORE: ___/10
```

## 1.4 Color & Visual Design Evaluation

### 1.4.1 Color System Analysis

```
COLOR PALETTE IDENTIFICATION:
- Primary color: [Color + hex]
- Secondary color: [Color + hex]
- Accent color: [Color + hex]
- Success color: [Color + hex]
- Warning color: [Color + hex]
- Error color: [Color + hex]
- Neutral colors: [Range]
- Background color: [Color + hex]
- Text color: [Color + hex]

COLOR USAGE EVALUATION:
□ Primary color used for primary actions/branding
□ Colors are used consistently (same meaning throughout)
□ Not too many colors (max 5-6 plus neutrals)
□ Colors work together harmoniously
□ Sufficient contrast for accessibility
□ Color is not the ONLY indicator (shapes, icons also used)

CONTRAST CHECKS (WCAG AA minimum):
| Element | Foreground | Background | Ratio | Pass? |
|---------|------------|------------|-------|-------|
| Body text | [Color] | [Color] | [Ratio] | [ ] 4.5:1+ |
| Large text | [Color] | [Color] | [Ratio] | [ ] 3:1+ |
| UI components | [Color] | [Color] | [Ratio] | [ ] 3:1+ |
| Form inputs | [Color] | [Color] | [Ratio] | [ ] |
| Links | [Color] | [Color] | [Ratio] | [ ] |

COLOR PSYCHOLOGY CHECK:
□ Colors appropriate for industry/purpose
□ Error states feel urgent (reds/oranges)
□ Success states feel positive (greens)
□ Warnings feel cautionary (yellows/oranges)
□ Primary actions stand out

COLOR SCORE: ___/10
```

### 1.4.2 Visual Polish & Details

```
SHADOWS & DEPTH:
□ Consistent shadow style throughout
□ Shadows indicate elevation logically
□ Not too harsh or too subtle
□ Cards/modals have appropriate depth

BORDERS & DIVIDERS:
□ Used purposefully (not everywhere)
□ Consistent style (color, weight)
□ Subtle, not overwhelming
□ Create visual separation where needed

ICONS:
□ Consistent style (outline, filled, etc.)
□ Consistent size
□ Meaningful and recognizable
□ Have accessible labels/tooltips
□ Not overused

IMAGES (if applicable):
□ Appropriate resolution
□ Consistent style/treatment
□ Properly sized (not stretched/squished)
□ Have alt text for accessibility

MICRO-DETAILS:
□ Rounded corners are consistent
□ Spacing follows a system (4px, 8px, 16px, etc.)
□ Alignment is precise (no 1px off-ness)
□ Interactive elements have hover/focus states

VISUAL POLISH SCORE: ___/10
```

## 1.5 Space Utilization & Layout Efficiency

### 1.5.1 Screen Real Estate Audit

```
DESKTOP VIEW (1920x1080):

□ Content doesn't stretch edge-to-edge uncomfortably
□ Maximum content width is set appropriately (1200-1400px typical)
□ Extra space on large screens is handled gracefully
□ No vast empty areas that feel wasteful
□ No cramped areas that feel overwhelming

MEASUREMENT CHECK:
- Header height: [px] - Appropriate? [ ]
- Sidebar width: [px] - Appropriate? [ ]
- Content area width: [px] - Appropriate? [ ]
- Spacing between elements: Consistent? [ ]

SPACE EFFICIENCY RATING:
[ ] Excellent - Every pixel has purpose
[ ] Good - Minor areas could be optimized
[ ] Fair - Some wasted or cramped space
[ ] Poor - Significant layout issues
```

### 1.5.2 Smart Space Solutions

Evaluate if these patterns are used appropriately:

```
TABS:
□ Used to organize related content that doesn't need simultaneous view
□ Tab labels are clear and concise
□ Active tab is clearly indicated
□ Tab content loads smoothly
□ Not overused (max 5-7 tabs typically)
Where tabs are used: [List pages]
Where tabs SHOULD be used: [Suggestions]

ACCORDIONS/COLLAPSIBLES:
□ Used for secondary content users may not need
□ Clear expand/collapse indicators
□ Smooth animation
□ Remember state where appropriate
Where used: [List]
Where SHOULD be used: [Suggestions]

MODALS/DIALOGS:
□ Used for focused tasks that need attention
□ Not overused (not modal-in-modal)
□ Can be closed easily
□ Don't contain too much content
□ Proper focus management
Where used: [List]
Appropriate usage: [ ] Yes / [ ] No

SPLIT VIEWS/MASTER-DETAIL:
□ Used when viewing list + detail simultaneously helps
□ Proportions are appropriate
□ Resizable if appropriate
Where applicable: [Identify opportunities]

DRAWERS/SLIDE-OUTS:
□ Used for supplementary content
□ Don't block critical information
□ Easy to dismiss
Where used: [List]

CARDS:
□ Used to group related information
□ Consistent card styling
□ Appropriate information density
□ Clear hierarchy within cards
Where used: [List]
Effective usage: [ ] Yes / [ ] No

SPACE UTILIZATION SCORE: ___/10
```

## 1.6 Navigation & Wayfinding

### 1.6.1 Navigation Structure Evaluation

```
PRIMARY NAVIGATION:
□ Contains all main sections
□ Logical grouping of items
□ Order makes sense (most used first, or logical flow)
□ Not too many items (7±2 rule)
□ Icons aid recognition (not just decoration)

CURRENT LOCATION INDICATORS:
□ User always knows where they are
□ Active state is visually distinct
□ Breadcrumbs present for deep hierarchies
□ Page titles match navigation labels

NAVIGATION PATTERNS:
| Pattern | Present? | Effective? |
|---------|----------|------------|
| Top navigation bar | [ ] | [ ] |
| Left sidebar | [ ] | [ ] |
| Breadcrumbs | [ ] | [ ] |
| Tab navigation | [ ] | [ ] |
| Pagination | [ ] | [ ] |
| Back buttons | [ ] | [ ] |
| Search | [ ] | [ ] |

NAVIGATION ACCESSIBILITY:
□ Keyboard navigable
□ Focus states visible
□ Skip links present
□ Mobile menu works

NAVIGATION SCORE: ___/10
```

### 1.6.2 User Journey Flow

```
For each major user task, trace the journey:

TASK: [e.g., "Create a new customer"]
Steps required:
1. [Step] - Obvious? [ ] Yes / [ ] No
2. [Step] - Obvious? [ ] Yes / [ ] No
3. [Step] - Obvious? [ ] Yes / [ ] No

Friction points identified:
- [List any confusing or unnecessary steps]

Suggested improvements:
- [How to streamline]

TASK: [Next major task]
[Repeat analysis]

OVERALL JOURNEY SCORE: ___/10
```

## 1.7 Form Design Excellence

### 1.7.1 Form Layout & Structure

```
For EVERY form in the application:

FORM: [Form Name]

LAYOUT:
□ Single column layout (preferred for most forms)
□ Logical field grouping
□ Related fields are visually connected
□ Sections have clear headers
□ Appropriate form length (or broken into steps)

LABELS:
□ Every field has a label
□ Labels are above fields (preferred) or clearly associated
□ Labels are concise but descriptive
□ Required fields are indicated (asterisk or text)
□ Optional fields are indicated (if minority)

FIELD SIZING:
□ Field width matches expected input
  - Short answers (names): Medium width
  - Long answers (descriptions): Full width
  - Numbers: Narrow width
  - Emails: Medium-wide width
□ All fields align consistently
□ Touch targets are large enough (min 44px)

HELP TEXT:
□ Complex fields have helper text
□ Format requirements are shown (date format, etc.)
□ Character limits shown where relevant
□ Help text doesn't clutter simple fields

FORM LAYOUT SCORE: ___/10
```

### 1.7.2 Form Interaction Design

```
INPUT TYPES:
□ Correct input type for data (email, tel, number, date)
□ Dropdowns for limited choices (< 10 options)
□ Radio buttons for mutually exclusive choices (< 5 options)
□ Checkboxes for multiple selections
□ Text areas for long text
□ Date pickers for dates
□ File upload styled appropriately

INTERACTION STATES:
| State | Visually Distinct? | Example Field |
|-------|-------------------|---------------|
| Default | [ ] | [Check] |
| Focus | [ ] | [Check] |
| Filled | [ ] | [Check] |
| Error | [ ] | [Check] |
| Disabled | [ ] | [Check] |
| Read-only | [ ] | [Check] |

VALIDATION FEEDBACK:
□ Real-time validation (as user types/leaves field)
□ Error messages appear near the field
□ Error messages explain how to fix
□ Success indicators where helpful
□ Form-level error summary for long forms
□ Focus moves to first error on submit

SMART DEFAULTS:
□ Sensible defaults where appropriate
□ Today's date for date fields (if applicable)
□ User's info pre-filled (if known)
□ Most common option pre-selected (if appropriate)

FORM INTERACTION SCORE: ___/10
```

## 1.8 Data Display Excellence

### 1.8.1 Table/List Design

```
For EVERY data table/list:

TABLE: [Table Name] on [Page]

COLUMN DESIGN:
□ Column headers are clear and concise
□ Important columns are first (left side)
□ Column widths match content needs
□ Numeric data is right-aligned
□ Action columns are last (right side)
□ No unnecessary columns

DATA PRESENTATION:
□ Dates are formatted consistently and readably
□ Numbers are formatted (thousands separators, decimals)
□ Currency shows symbol and proper format
□ Long text is truncated with ellipsis (tooltip for full)
□ Status uses visual indicators (badges, colors)
□ Empty cells show dash or "N/A" (not blank)

TABLE FUNCTIONALITY:
□ Sortable columns are indicated
□ Current sort is shown
□ Filterable where appropriate
□ Search works across visible columns
□ Pagination or infinite scroll for large data sets
□ Row selection where appropriate
□ Bulk actions where appropriate

ROW DENSITY:
□ Appropriate padding (not cramped, not wasteful)
□ Row hover state for clickable rows
□ Zebra striping or dividers for readability
□ Consistent row height

RESPONSIVE BEHAVIOR:
□ Horizontal scroll for wide tables (contained)
□ Or columns stack/hide appropriately
□ Key columns always visible

TABLE DESIGN SCORE: ___/10
```

### 1.8.2 Card & Grid Layouts

```
If cards/grids are used:

CARD DESIGN:
□ Clear visual boundaries
□ Consistent card sizes (or intentional variation)
□ Content hierarchy within cards is clear
□ Interactive cards have hover states
□ Cards aren't overloaded with information
□ Actions are appropriately placed

GRID LAYOUT:
□ Consistent gutters between items
□ Responsive columns (4 → 2 → 1 as screen shrinks)
□ Items align properly
□ No orphaned single items on rows

CARD/GRID SCORE: ___/10
```

### 1.8.3 Detail Page Design

```
For EVERY detail/view page:

PAGE: [Entity] Detail Page

INFORMATION ORGANIZATION:
□ Most important info is prominent
□ Related info is grouped logically
□ Clear section headers
□ Appropriate use of tabs (if complex)
□ Related entities are accessible

FIELD DISPLAY:
□ Labels are clearly associated with values
□ Empty fields handled gracefully (show "Not provided" vs hiding)
□ Values are formatted appropriately
□ Long text is readable (proper line length)

ACTIONS:
□ Edit action is easily accessible
□ Delete has confirmation
□ Related actions are grouped
□ Primary action is prominent

DETAIL PAGE SCORE: ___/10
```

## 1.9 Feedback & Communication

### 1.9.1 System Feedback

```
LOADING STATES:
□ Spinner or skeleton while loading
□ Clear indication something is happening
□ Appropriate for duration (spinner < 3s, progress bar > 3s)
□ No empty flashes before content
Loading states present for:
- [ ] Page loads
- [ ] Data fetches
- [ ] Form submissions
- [ ] File uploads

SUCCESS FEEDBACK:
□ Clear confirmation after actions
□ Toast/snackbar for minor successes
□ Success page/message for major completions
□ Appropriate duration (not too fast, not too slow)

ERROR FEEDBACK:
□ Errors are visible and clear
□ Explain what went wrong
□ Suggest how to fix
□ Don't blame the user
□ Persistent until dismissed (not auto-hide errors)

EMPTY STATES:
□ Every possible empty state has a design
□ Explains why it's empty
□ Suggests next action
□ Has visual interest (illustration/icon)

Empty states needed for:
- [ ] No search results
- [ ] Empty list (no records yet)
- [ ] No permissions
- [ ] Error state (failed to load)

FEEDBACK SCORE: ___/10
```

### 1.9.2 Micro-Interactions

```
BUTTON FEEDBACK:
□ Hover state on all buttons
□ Active/pressed state
□ Disabled state when appropriate
□ Loading state for async actions

LINK FEEDBACK:
□ Hover state (underline or color change)
□ Visited state (if applicable)
□ Focus state for keyboard

INPUT FEEDBACK:
□ Focus state clearly visible
□ Character count where relevant
□ Real-time validation feedback
□ Autocomplete where helpful

TRANSITIONS:
□ Smooth transitions (not jarring)
□ Appropriate duration (200-300ms typical)
□ Purposeful (not just decoration)
□ Respect prefers-reduced-motion

MICRO-INTERACTION SCORE: ___/10
```

## 1.10 Responsive Design Mastery

### 1.10.1 Breakpoint Testing

```
Test at these exact widths:

DESKTOP LARGE (1920px):
□ Layout uses space well
□ Content isn't stretched uncomfortably
□ No horizontal scroll
Screenshot observation: [Notes]

DESKTOP (1440px):
□ Primary layout intact
□ All features accessible
Screenshot observation: [Notes]

LAPTOP (1024px):
□ Layout adjusts appropriately
□ Sidebar may collapse
□ Tables may scroll
Screenshot observation: [Notes]

TABLET (768px):
□ Single column layout where appropriate
□ Navigation collapses to hamburger
□ Touch-friendly tap targets
□ Tables adapt or scroll
Screenshot observation: [Notes]

MOBILE (375px):
□ All content accessible
□ No horizontal scroll
□ Readable text (no zooming needed)
□ Buttons are finger-friendly (44px min)
□ Forms are usable
□ Modals work properly
Screenshot observation: [Notes]

RESPONSIVE SCORE: ___/10
```

### 1.10.2 Responsive Component Behavior

```
| Component | Desktop | Tablet | Mobile | Works? |
|-----------|---------|--------|--------|--------|
| Header | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Navigation | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Data tables | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Forms | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Cards/Grid | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Modals | [Behavior] | [Behavior] | [Behavior] | [ ] |
| Images | [Behavior] | [Behavior] | [Behavior] | [ ] |
```

## 1.11 Accessibility Audit

### 1.11.1 WCAG 2.1 AA Compliance Check

```
PERCEIVABLE:
□ All images have alt text
□ Color is not only indicator
□ Contrast ratios meet minimums
□ Text can be resized to 200%
□ No content only in images of text

OPERABLE:
□ All functionality available by keyboard
□ No keyboard traps
□ Skip links present
□ Focus is visible and logical
□ No time limits (or extendable)
□ No content that flashes

UNDERSTANDABLE:
□ Language is specified
□ Navigation is consistent
□ Error identification is clear
□ Labels and instructions provided

ROBUST:
□ Valid HTML
□ Name, role, value available
□ Status messages announced

ACCESSIBILITY SCORE: ___/10
```

### 1.11.2 Keyboard Navigation Test

```
Tab through entire application:

□ Can reach all interactive elements
□ Tab order is logical (follows visual order)
□ Focus indicator is always visible
□ Can activate all buttons/links with Enter
□ Can close modals with Escape
□ Can navigate dropdowns with arrows
□ No keyboard traps

Issues found:
- [List any keyboard issues]
```

---

# PART 2: FUNCTIONAL TESTING (QA ENGINEER HAT)

## 2.1 Foundation Checks

### 2.1.1 Application Startup

```
□ Backend starts without errors
  Command: [Command used]
  Result: [ ] Success / [ ] Errors (list below)
  Errors: [Document any]

□ Frontend starts without errors
  Command: [Command used]
  Result: [ ] Success / [ ] Errors (list below)
  Errors: [Document any]

□ Application loads in browser
  URL: [URL]
  Result: [ ] Loads / [ ] Fails
  Console errors: [ ] None / [ ] Present (list below)
  Errors: [Document any]

□ API is accessible from frontend
  Test endpoint: [Endpoint]
  Result: [ ] Success / [ ] CORS error / [ ] Other error

STARTUP SCORE: ___/10
```

### 2.1.2 Database & Seed Data

```
□ Database connection works
□ All migrations applied
□ Seed data script runs successfully
□ All tables have data

SEED DATA VERIFICATION:
| Table/Collection | Expected Records | Actual Records | Has Variety? |
|------------------|------------------|----------------|--------------|
| Users | 3+ | [Count] | [ ] |
| [Entity 1] | 5+ | [Count] | [ ] |
| [Entity 2] | 5+ | [Count] | [ ] |
| [Entity 3] | 5+ | [Count] | [ ] |
| [Lookup data] | All options | [Count] | [ ] |

SEED DATA QUALITY:
□ Realistic names (not "test", "asdf")
□ Realistic emails (@example.com for safety)
□ Various statuses represented
□ Various dates (past, recent, future where appropriate)
□ Valid relationships (FKs exist)
□ Edge cases included (overdue, expired, etc.)

DATABASE SCORE: ___/10
```

## 2.2 Authentication Testing

### 2.2.1 Registration (If Applicable)

```
□ Navigate to registration: [ ] Works / [ ] Broken
□ All required fields present: [ ] Yes / [ ] Missing: ______
□ Submit empty form: [ ] Validation errors / [ ] Submits anyway
□ Submit invalid email: [ ] Shows error / [ ] Accepts
□ Submit weak password: [ ] Shows error / [ ] Accepts
□ Submit valid data: [ ] Creates account / [ ] Fails
□ Duplicate email: [ ] Shows error / [ ] Creates duplicate

Test credentials created:
- Email: [Email used]
- Password: [Password used]

REGISTRATION SCORE: ___/10
```

### 2.2.2 Login Testing

```
□ Navigate to login: [ ] Works / [ ] Broken
□ Form displays correctly: [ ] Yes / [ ] Issues: ______
□ Empty submission: [ ] Validation / [ ] Error
□ Wrong credentials: [ ] Generic error / [ ] Reveals too much
□ Correct credentials: [ ] Logs in / [ ] Fails

Seeded credentials used:
- Email: [Email]
- Password: [Password]
- Result: [ ] Success / [ ] Failure

Post-login checks:
□ Redirected to correct page
□ User info displayed (name, avatar, etc.)
□ Protected pages now accessible
□ Login page redirects to dashboard if already logged in
□ Session persists on refresh

LOGIN SCORE: ___/10
```

### 2.2.3 Logout Testing

```
□ Logout button accessible: [ ] Yes / [ ] No
□ Click logout: [ ] Logs out / [ ] Nothing happens
□ Redirected appropriately: [ ] Yes / [ ] No
□ Protected pages blocked after logout: [ ] Yes / [ ] No
□ Back button doesn't reveal protected content: [ ] Secure / [ ] Insecure

LOGOUT SCORE: ___/10
```

## 2.3 Page-by-Page Functional Testing

### 2.3.1 List Page Testing Template

For EVERY list/table page:

```
PAGE: [Page Name]
URL: [URL]

LOAD TEST:
□ Page loads without errors
□ Data displays (NOT EMPTY)
□ Correct columns shown
□ Data formatted correctly

Record count: [Number displayed]
Expected: [Should match seed data]

If empty, STOP and fix:
□ Check API endpoint
□ Check frontend API call
□ Check data binding
□ Add seed data if missing

INTERACTION TESTS:
□ Row click/view works
□ Edit button works
□ Delete button works (with confirmation)
□ Sort works (if present)
□ Search works (if present)
□ Filter works (if present)
□ Pagination works (if present)

EDGE CASES:
□ Search with no results shows message
□ Filter with no results shows message
□ Very long content truncates gracefully

PAGE SCORE: ___/10
```

### 2.3.2 Create Form Testing Template

For EVERY create form:

```
FORM: Create [Entity]
URL: [URL]

DISPLAY TEST:
□ Form displays
□ All fields present
□ Labels are correct
□ Required indicators present
□ Dropdowns populated (NOT EMPTY)

DROPDOWN VERIFICATION:
| Dropdown | Options Expected | Options Present | Correct? |
|----------|------------------|-----------------|----------|
| [Drop 1] | [List] | [Actual] | [ ] |
| [Drop 2] | [List] | [Actual] | [ ] |

VALIDATION TESTS:
| Test | Field | Input | Expected | Actual | Pass? |
|------|-------|-------|----------|--------|-------|
| Empty required | [Field] | (empty) | Error | | [ ] |
| Invalid format | Email | "notanemail" | Error | | [ ] |
| Too long | Name | [255+ chars] | Error | | [ ] |
| Invalid number | Amount | "abc" | Error | | [ ] |

SUBMISSION TESTS:
□ Valid data submits successfully
□ Success message shown
□ Redirected appropriately
□ Record appears in list
□ All data saved correctly

Test data used:
| Field | Value |
|-------|-------|
| [Field 1] | [Value] |
| [Field 2] | [Value] |

FORM SCORE: ___/10
```

### 2.3.3 Edit Form Testing Template

For EVERY edit form:

```
FORM: Edit [Entity]
URL Pattern: [URL/:id]
Test Record: [ID from seed data]

LOAD TEST:
□ Form loads with existing data
□ All fields pre-populated
□ Dropdown shows current selection
□ Dates formatted correctly

PRE-POPULATED DATA CHECK:
| Field | Expected (from DB) | Shows in Form | Match? |
|-------|-------------------|---------------|--------|
| [Field 1] | [Value] | [Value] | [ ] |
| [Field 2] | [Value] | [Value] | [ ] |

If form loads empty, STOP and fix:
□ Check API endpoint returns data
□ Check form fetches on load
□ Check data binds to fields

EDIT TESTS:
□ Change field and save
□ Changes persist on reload
□ Validation still works
□ Cancel discards changes

FORM SCORE: ___/10
```

### 2.3.4 Detail Page Testing Template

For EVERY detail/view page:

```
PAGE: [Entity] Detail
URL Pattern: [URL/:id]
Test Record: [ID from seed data]

DISPLAY TEST:
□ Page loads with data
□ All fields display values
□ Related data displays
□ Formatted correctly (dates, currency, etc.)

FIELD VERIFICATION:
| Field | Expected Value | Displayed Value | Correct? |
|-------|----------------|-----------------|----------|
| [Field 1] | [Value] | [Value] | [ ] |
| [Field 2] | [Value] | [Value] | [ ] |

NAVIGATION:
□ Edit button works
□ Delete button works
□ Back/return works
□ Related links work

ERROR HANDLING:
□ Invalid ID shows 404/not found
□ Graceful error message

PAGE SCORE: ___/10
```

## 2.4 CRUD Cycle Verification

For each main entity, complete full cycle:

```
ENTITY: [Entity Name]

CREATE:
□ Navigate to create form
□ Fill in all fields
□ Submit successfully
□ Record created with ID: [ID]

READ:
□ View record in list
□ View record detail page
□ All data displays correctly

UPDATE:
□ Open edit form
□ Data pre-populated correctly
□ Change [Field] to [New Value]
□ Save successfully
□ Change persisted

DELETE:
□ Click delete
□ Confirmation appears
□ Confirm deletion
□ Record removed from list
□ Record not accessible

CRUD CYCLE: [ ] COMPLETE / [ ] FAILED at step: ______
```

## 2.5 Edge Case & Error Testing

### 2.5.1 Error Handling

```
□ 404 page exists and is styled
□ API errors show user-friendly message
□ Network failure handled gracefully
□ Server error (500) shows appropriate message
□ No raw error messages or stack traces exposed

ERROR HANDLING SCORE: ___/10
```

### 2.5.2 Edge Cases

```
□ Empty search returns helpful message
□ Extremely long text input handled
□ Special characters in input handled
□ Rapid clicking doesn't cause issues
□ Double-submit prevention works
□ Concurrent edit handling (if applicable)
□ Browser refresh during operation

EDGE CASE SCORE: ___/10
```

---

# PART 3: TECHNICAL QUALITY AUDIT

## 3.1 Code Quality Check

### 3.1.1 Backend Quality

```
STRUCTURE:
□ Logical folder organization
□ Separation of concerns
□ Consistent naming conventions
□ No commented-out code
□ No unused imports

ERROR HANDLING:
□ Try-catch where appropriate
□ Meaningful error messages
□ Proper HTTP status codes
□ Validation on all inputs

SECURITY:
□ Input sanitization
□ SQL injection prevention (parameterized queries)
□ Authentication on protected routes
□ No sensitive data in responses
□ CORS properly configured

BACKEND SCORE: ___/10
```

### 3.1.2 Frontend Quality

```
STRUCTURE:
□ Component organization logical
□ Reusable components extracted
□ Consistent naming
□ No duplicate code
□ State management appropriate

ERROR HANDLING:
□ API errors caught and displayed
□ Loading states implemented
□ Empty states handled
□ Form validation implemented

PERFORMANCE:
□ No unnecessary re-renders
□ Large lists virtualized/paginated
□ Images optimized
□ No memory leaks on navigation

FRONTEND SCORE: ___/10
```

## 3.2 Console & Network Check

```
BROWSER CONSOLE:
□ No JavaScript errors during normal use
□ No warnings (or acceptable/expected only)
□ No failed network requests

NETWORK TAB:
□ API calls return expected status codes
□ No CORS errors
□ Reasonable response times
□ No excessive calls

CONSOLE SCORE: ___/10
```

---

# PART 4: SCORING & DELIVERY DECISION

## 4.1 Score Summary

```
SECTION SCORES:

UI/UX DESIGN:
- First Impressions: ___/10
- Visual Hierarchy: ___/10
- Typography: ___/10
- Color & Visual: ___/10
- Space Utilization: ___/10
- Navigation: ___/10
- Form Design: ___/10
- Data Display: ___/10
- Feedback: ___/10
- Responsive: ___/10
- Accessibility: ___/10
UI/UX SUBTOTAL: ___/110 = ___%

FUNCTIONAL TESTING:
- Foundation: ___/10
- Authentication: ___/10
- List Pages: ___/10
- Create Forms: ___/10
- Edit Forms: ___/10
- Detail Pages: ___/10
- CRUD Cycles: ___/10
- Error Handling: ___/10
FUNCTIONAL SUBTOTAL: ___/80 = ___%

TECHNICAL QUALITY:
- Backend Quality: ___/10
- Frontend Quality: ___/10
- Console/Network: ___/10
TECHNICAL SUBTOTAL: ___/30 = ___%

═══════════════════════════════
OVERALL SCORE: ___/220 = ___%
═══════════════════════════════
```

## 4.2 Delivery Decision

```
CRITICAL FAILURES (Any = Cannot Deliver):
□ Empty pages with no data
□ Login doesn't work
□ Any CRUD operation fails
□ Empty dropdowns
□ Console errors during normal use
□ Professional appearance failure

MAJOR ISSUES (Must fix before delivery):
- [List issues scoring below 6/10]

MINOR ISSUES (Document, fix if time):
- [List cosmetic or enhancement items]

DELIVERY DECISION:
[ ] ✅ READY TO DELIVER (Score ≥ 90%, no critical failures)
[ ] ⚠️ NEEDS FIXES (Score 70-89%, or critical failures)
[ ] ❌ NOT READY (Score < 70%, multiple critical failures)
```

## 4.3 Required Documentation

When delivering, include:

```markdown
## Application Delivery Package

### 1. Startup Instructions
[Exact commands to run backend, frontend, database]

### 2. Environment Setup
[All env variables needed]

### 3. Test Credentials
| Role | Email | Password |
|------|-------|----------|
| Admin | [email] | [password] |
| User | [email] | [password] |

### 4. Seed Data Summary
[What data was seeded, how to re-seed]

### 5. Quality Report
Overall Score: ___%

Passed Tests: [List]
Known Limitations: [List]
Future Improvements: [List]

### 6. Pages Inventory
| Page | URL | Status |
|------|-----|--------|
| Dashboard | /dashboard | ✅ Working |
| Customers | /customers | ✅ Working |
| ... | ... | ... |
```

---

# HOW TO USE THIS PROTOCOL

## For Every Application Build Request:

1. **Build** the application per requirements
2. **Stop** before declaring it done
3. **Run** this ENTIRE protocol
4. **Score** honestly in each section
5. **Fix** all critical and major issues
6. **Re-test** affected areas
7. **Document** results
8. **Deliver** only when quality standards met

## Red Flags That Mean "Not Done":

🚩 "The data will appear when you add records" - NO, seed the data
🚩 "The styling can be improved later" - NO, meet minimum standards
🚩 "Login should work with your credentials" - NO, test it yourself
🚩 Empty tables, dropdowns, or pages - NEVER acceptable
🚩 Console errors - NEVER acceptable
🚩 "I didn't test on mobile" - Test it

## Quality Standards:

- **Minimum acceptable score: 90%**
- **Zero critical failures**
- **All CRUD operations working**
- **Professional appearance**
- **Seed data in place**
- **Documentation complete**

---

**Remember: An application that looks amateur, has empty pages, or doesn't function completely is NOT FINISHED. The user should be able to use it immediately upon delivery without any additional setup or fixes.**
````

---

## Quick Reference Card

### Most Critical Checks (Never Skip)

| Check | Why It Matters |
|-------|----------------|
| Seed data populated | Empty pages = broken app perception |
| Login works | Can't test anything without access |
| All dropdowns populated | Empty dropdowns = broken forms |
| CRUD operations work | Core functionality |
| No console errors | Indicates broken code |
| Professional appearance | Trust and usability |
| Typography readable | User can't use what they can't read |
| Responsive design | Users have different devices |

### Score Interpretation

| Score | Meaning | Action |
|-------|---------|--------|
| 95-100% | Excellent | Ship it! |
| 90-94% | Good | Minor polish, can ship |
| 80-89% | Needs Work | Fix major issues |
| 70-79% | Significant Issues | Major revision needed |
| <70% | Not Acceptable | Start over or major rework |

---

## How to Use This With Your Build Requests

```markdown
## My Request

Build me a [description] with these features:
- [Feature 1]
- [Feature 2]
- [Feature 3]

## MANDATORY QUALITY REQUIREMENT

You MUST complete the "Ultimate Application Quality Assurance & UI/UX Mastery Protocol" before delivering this application.

Minimum standards:
- Overall score of 90% or higher
- Zero critical failures
- Seed data in every table
- All pages show content
- Login works with provided credentials
- Professional, polished UI
- Responsive design
- No console errors

Do NOT tell me it's complete until you have tested everything yourself and documented the results.

[Paste the protocol here or reference it]
```

---

Would you like me to create specialized versions of this protocol for specific scenarios (like "mobile app testing" or "e-commerce specific testing"), or add any additional evaluation criteria?