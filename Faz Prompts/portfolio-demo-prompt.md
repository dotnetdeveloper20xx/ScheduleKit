# Portfolio Demo Generation Prompt

Use this prompt when working on a separate project that you want to showcase on the DotNetDeveloper portfolio website. Generate ALL the following artifacts comprehensively so they can be copied into the portfolio's demo folder.

**Important**: Be thorough and detailed. Extract as much information about this project as possible. The more comprehensive the output, the better the portfolio demo will be.

---

## Context

I have a portfolio website that showcases my demo projects. Each project is displayed as a card with:
- A colored banner with an icon
- Project name and tagline
- Description
- Tech stack badges
- Key feature highlights
- A link to view the full demo

When users click "View Project", they're taken to a standalone HTML page that presents the project comprehensively with multiple sections, screenshots, feature lists, and technical details.

**Target Audience**: Potential clients and employers evaluating senior .NET development capabilities.

---

## SECTION 1: Project Identity

Generate comprehensive project identification:

```json
{
  "id": "project-slug",
  "name": "ProjectName",
  "tagline": "Short catchy tagline (5-8 words)",
  "elevatorPitch": "One paragraph (3-4 sentences) that you would use to pitch this project to a non-technical stakeholder. Focus on business value and problem solved.",
  "technicalSummary": "One paragraph (3-4 sentences) for technical audiences explaining the architecture and key technical decisions.",
  "problemStatement": "What specific problem does this application solve? Who experiences this problem?",
  "solution": "How does this application solve the problem? What's the unique approach?",
  "targetUsers": [
    "Primary user type and their goals",
    "Secondary user type and their goals",
    "Admin/operator user type if applicable"
  ],
  "industryDomain": "e.g., FinTech, HealthTech, E-commerce, Sports, etc.",
  "projectType": "e.g., SaaS Platform, Internal Tool, Marketplace, CMS, etc."
}
```

---

## SECTION 2: Complete Tech Stack

Provide exhaustive technology details:

```json
{
  "techStack": {
    "backend": {
      "runtime": ".NET 8",
      "framework": "ASP.NET Core Web API",
      "orm": "Entity Framework Core 8",
      "database": "SQL Server 2022",
      "caching": "e.g., Redis, In-Memory Cache",
      "messaging": "e.g., SignalR, RabbitMQ, Azure Service Bus",
      "authentication": "e.g., JWT, Identity, OAuth2",
      "libraries": ["MediatR", "FluentValidation", "AutoMapper", "Serilog", "etc."]
    },
    "frontend": {
      "framework": "Angular 17+",
      "stateManagement": "e.g., Signals, NgRx, Services",
      "uiLibrary": "e.g., Angular Material, Tailwind CSS, Bootstrap",
      "charts": "e.g., Chart.js, ng2-charts, ApexCharts",
      "forms": "Reactive Forms / Template-driven",
      "libraries": ["RxJS", "etc."]
    },
    "infrastructure": {
      "hosting": "e.g., Azure App Service, Docker, IIS",
      "database": "e.g., Azure SQL, SQL Server",
      "storage": "e.g., Azure Blob Storage, Local file system",
      "ci_cd": "e.g., GitHub Actions, Azure DevOps",
      "monitoring": "e.g., Application Insights, Serilog"
    },
    "architecture": {
      "pattern": "e.g., Clean Architecture, N-Tier, Vertical Slices",
      "principles": ["SOLID", "DDD", "CQRS", "etc."],
      "projectStructure": "Describe the solution structure (e.g., 6 projects: Domain, Application, Infrastructure, API, Web, Tests)"
    }
  }
}
```

---

## SECTION 3: Feature Inventory

List EVERY feature in the application, organized by category:

```markdown
## Complete Feature List

### Authentication & Authorization
- [ ] User registration with email verification
- [ ] Login with JWT tokens
- [ ] Password reset flow
- [ ] Remember me functionality
- [ ] Role-based access control (list all roles)
- [ ] Permission-based authorization
- [ ] Session management
- [ ] Account lockout after failed attempts
- [ ] Two-factor authentication (if applicable)

### User Management
- [ ] User profile management
- [ ] Avatar/profile picture upload
- [ ] Account settings
- [ ] Notification preferences
- [ ] Email preferences
- [ ] Delete account / GDPR compliance

### [Core Domain Feature 1] (e.g., "Auction Management")
- [ ] Feature detail 1
- [ ] Feature detail 2
- [ ] Feature detail 3
(List every sub-feature exhaustively)

### [Core Domain Feature 2]
(Continue for all feature areas)

### Search & Filtering
- [ ] Global search
- [ ] Advanced filters
- [ ] Saved searches
- [ ] Search suggestions/autocomplete
- [ ] Recent searches

### Data Management
- [ ] CRUD operations for each entity
- [ ] Bulk operations
- [ ] Import functionality (CSV, Excel)
- [ ] Export functionality (PDF, Excel, CSV)
- [ ] Data validation rules
- [ ] Duplicate detection

### Notifications & Communications
- [ ] In-app notifications
- [ ] Email notifications (list all email types)
- [ ] Push notifications (if applicable)
- [ ] SMS notifications (if applicable)
- [ ] Notification preferences

### Reporting & Analytics
- [ ] Dashboard with KPIs
- [ ] Charts and visualizations
- [ ] Report generation
- [ ] Date range filtering
- [ ] Export reports
- [ ] Scheduled reports (if applicable)

### Admin Features
- [ ] User management
- [ ] System configuration
- [ ] Audit logs
- [ ] Content management
- [ ] Email template management
- [ ] System health monitoring

### UI/UX Features
- [ ] Responsive design (mobile, tablet, desktop)
- [ ] Dark mode / theme switching
- [ ] Accessibility features (WCAG compliance level)
- [ ] Loading states and skeletons
- [ ] Error handling and user feedback
- [ ] Form validation with clear messages
- [ ] Confirmation dialogs for destructive actions
- [ ] Keyboard navigation
- [ ] Breadcrumbs / navigation aids

### Performance Features
- [ ] Pagination
- [ ] Infinite scroll
- [ ] Lazy loading
- [ ] Caching strategy
- [ ] Optimistic updates
- [ ] Background processing
```

---

## SECTION 4: User Roles & Permissions

Detail every user role and their exact capabilities:

```markdown
## User Roles

### Role 1: [e.g., "Administrator"]
**Description**: [Who is this user?]
**Access Level**: Full system access

**Can Do:**
- List every permission/capability
- Be specific about CRUD operations per entity
- Include admin-specific features

**Cannot Do:**
- Any restrictions

**Dashboard View:**
- What do they see on their dashboard?

---

### Role 2: [e.g., "Standard User"]
(Repeat the same detailed structure)

---

### Role 3: [e.g., "Guest/Anonymous"]
(Repeat for all roles)
```

---

## SECTION 5: User Journeys & Workflows

Document key user flows step-by-step:

```markdown
## Key User Journeys

### Journey 1: [e.g., "New User Registration to First Purchase"]

**Actor**: New visitor
**Goal**: Complete registration and make first purchase
**Preconditions**: None

**Steps**:
1. User lands on homepage
2. Clicks "Sign Up" button
3. Fills registration form (fields: email, password, name, etc.)
4. Receives verification email
5. Clicks verification link
6. Redirected to onboarding wizard
7. Completes profile setup
8. Browses products
9. Adds item to cart
10. Proceeds to checkout
11. Enters payment details
12. Receives confirmation email
13. Views order in "My Orders"

**Success Criteria**: User has verified account and completed order
**Error Handling**: What happens if email fails? Payment fails? etc.

---

### Journey 2: [Next critical workflow]
(Document 5-10 key journeys)
```

---

## SECTION 6: Data Model Overview

Describe key entities and relationships:

```markdown
## Core Entities

### Entity 1: [e.g., "User"]
**Purpose**: [What does this entity represent?]
**Key Fields**:
- Id (GUID)
- Email (string, unique)
- PasswordHash (string)
- FirstName (string)
- LastName (string)
- Role (enum: Admin, User, etc.)
- CreatedAt (DateTime)
- IsActive (bool)
- EmailVerified (bool)

**Relationships**:
- Has many Orders
- Has many Reviews
- Has one Profile

**Business Rules**:
- Email must be unique
- Password must meet complexity requirements
- Cannot delete user with active orders

---

### Entity 2: [Next entity]
(Document all major entities)

## Key Relationships Diagram (describe in text)
- User (1) → (*) Orders
- Order (1) → (*) OrderItems
- OrderItem (*) → (1) Product
- etc.
```

---

## SECTION 7: API Endpoints

List all API endpoints:

```markdown
## API Endpoints

### Authentication
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | /api/auth/register | Register new user | No |
| POST | /api/auth/login | User login | No |
| POST | /api/auth/refresh | Refresh JWT token | Yes |
| POST | /api/auth/forgot-password | Request password reset | No |
| POST | /api/auth/reset-password | Reset password with token | No |

### Users
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | /api/users/me | Get current user profile | Yes |
| PUT | /api/users/me | Update current user | Yes |
| GET | /api/users | List all users (admin) | Admin |
| GET | /api/users/{id} | Get user by ID | Admin |

### [Domain Resource 1]
(Continue for all API areas)

**Total Endpoint Count**: X endpoints
```

---

## SECTION 8: Business Rules & Validation

Document all business logic:

```markdown
## Business Rules

### [Domain Area 1: e.g., "Auction Rules"]

1. **Rule Name**: Minimum Bid Increment
   - **Description**: Each new bid must be at least X% higher than current bid
   - **Implementation**: Validated in domain service
   - **Error Message**: "Bid must be at least £X higher than current bid"

2. **Rule Name**: Anti-Sniping Protection
   - **Description**: If bid placed within last 2 minutes, auction extends by 2 minutes
   - **Implementation**: Domain event triggers extension
   - **Max Extensions**: 5

(Continue for all business rules)

### Validation Rules

| Field | Entity | Rules | Error Message |
|-------|--------|-------|---------------|
| Email | User | Required, Valid email format, Max 256 chars | "Please enter a valid email address" |
| Password | User | Required, Min 8 chars, 1 uppercase, 1 number | "Password must be at least 8 characters with 1 uppercase and 1 number" |
| Price | Product | Required, > 0, Max 2 decimal places | "Price must be greater than zero" |

(Continue for all validated fields)
```

---

## SECTION 9: Email Templates

List all automated emails:

```markdown
## Email Communications

| Email Type | Trigger | Recipient | Key Content |
|------------|---------|-----------|-------------|
| Welcome Email | User registration | New user | Welcome message, getting started guide |
| Email Verification | Registration | New user | Verification link (expires in 24h) |
| Password Reset | Forgot password request | User | Reset link (expires in 1h) |
| Order Confirmation | Order placed | Customer | Order details, expected delivery |
| Shipping Notification | Order shipped | Customer | Tracking number, carrier info |
| [Continue for all emails] | | | |

**Total Email Templates**: X templates
```

---

## SECTION 10: Screenshots Specification

Detailed screenshot requirements:

```markdown
## Screenshots Required

### PRIMARY (Card Thumbnail - Most Important)
**Filename**: 1.png
**Screen**: Main Dashboard / Home
**What to Capture**:
- Show the primary interface users see after login
- Ensure there's meaningful data displayed
- Highlight key metrics or primary actions
**Resolution**: 1920x1080
**Browser**: Chrome (no bookmarks bar)
**Data State**: Populated with realistic demo data

---

### SECONDARY (Demo Page Gallery)

**Filename**: 2.png
**Screen**: [Screen Name]
**What to Capture**: [Specific instructions]
**Why Important**: [What skill/feature this demonstrates]

**Filename**: 3.png
**Screen**: [Screen Name]
**What to Capture**: [Specific instructions]
**Why Important**: [What skill/feature this demonstrates]

(Continue for 8-12 screenshots)

---

### Screenshot Checklist
- [ ] All screenshots use realistic data (not "test123" or Lorem Ipsum)
- [ ] No browser dev tools visible
- [ ] No console errors
- [ ] Consistent window size (1920x1080)
- [ ] User is logged in with appropriate role
- [ ] Data represents a "successful" state
- [ ] Forms show validation states where relevant
- [ ] Loading states captured where impressive
```

---

## SECTION 11: Security Features

Document security implementation:

```markdown
## Security Implementation

### Authentication
- [ ] Password hashing algorithm: [e.g., BCrypt with cost factor 12]
- [ ] JWT token expiration: [e.g., 15 minutes access, 7 days refresh]
- [ ] Secure cookie settings: [HttpOnly, Secure, SameSite]

### Authorization
- [ ] Role-based access control
- [ ] Permission-based authorization
- [ ] Resource-based authorization (users can only access their own data)

### Data Protection
- [ ] Input validation (server-side)
- [ ] SQL injection prevention (parameterized queries via EF Core)
- [ ] XSS prevention (output encoding)
- [ ] CSRF protection
- [ ] Sensitive data encryption at rest
- [ ] HTTPS enforcement

### API Security
- [ ] Rate limiting
- [ ] Request size limits
- [ ] CORS configuration
- [ ] API versioning

### Audit & Compliance
- [ ] Audit logging (what actions are logged)
- [ ] Data retention policies
- [ ] GDPR compliance features (data export, deletion)
```

---

## SECTION 12: Performance Characteristics

Document performance considerations:

```markdown
## Performance

### Database
- Indexes on: [list indexed columns]
- Query optimization: [any specific optimizations]
- Connection pooling: [configuration]

### Caching
- What is cached: [list cached data]
- Cache duration: [TTL values]
- Cache invalidation strategy: [approach]

### Frontend
- Lazy loaded modules: [list]
- Image optimization: [approach]
- Bundle size: [approximate]

### API
- Pagination: [page size defaults]
- Response compression: [enabled/disabled]
- Async operations: [list background jobs]
```

---

## SECTION 13: Demo Credentials & Test Data

Provide demo access details:

```markdown
## Demo Access

### Admin Account
- **Email**: admin@example.com
- **Password**: Demo123!
- **What to explore**: User management, system settings, reports

### Standard User Account
- **Email**: user@example.com
- **Password**: Demo123!
- **What to explore**: Core features, profile management

### Test Data Highlights
- X users in the system
- X [main entities] created
- Date range of data: [e.g., "Last 6 months of simulated activity"]
- Notable test scenarios:
  - User with many orders to demonstrate history
  - Edge case examples (cancelled orders, refunds, etc.)
```

---

## SECTION 14: Competitive Differentiators

What makes this project impressive?

```markdown
## Why This Project Stands Out

### Technical Excellence
1. [Specific technical achievement] - [Why it's impressive]
2. [Specific technical achievement] - [Why it's impressive]
3. [Specific technical achievement] - [Why it's impressive]

### Enterprise Patterns Demonstrated
1. [Pattern] - [How it's implemented]
2. [Pattern] - [How it's implemented]

### Problem-Solving Examples
1. [Challenge faced] - [How it was solved]
2. [Challenge faced] - [How it was solved]

### Code Quality Indicators
- Unit test coverage: X%
- Integration tests: X tests
- Code documentation: [approach]
- Error handling: [approach]
```

---

## SECTION 15: Future Roadmap

Ideas for future development (shows forward thinking):

```markdown
## Potential Enhancements

### Short-term (Nice to have)
- [ ] Feature idea 1
- [ ] Feature idea 2

### Medium-term (Valuable additions)
- [ ] Feature idea 1
- [ ] Feature idea 2

### Long-term (Vision)
- [ ] Feature idea 1
- [ ] Feature idea 2
```

---

## SECTION 16: Color & Icon Recommendation

```markdown
## Visual Identity for Portfolio

### Banner Color
**Recommended**: [blue/teal/purple/orange/green]
**Reason**: [Why this fits the project domain]

### Icon Suggestion
**Primary Icon**: [Describe ideal icon, e.g., "A gavel for auctions"]
**Alternative Icons**: [2-3 alternatives]
**SVG Concept**: [Describe the icon in enough detail to create an SVG]

### Color Reference
- **blue** (#0078d4) - Technology, enterprise, professional services
- **teal** (#008272) - Finance, healthcare, trust-focused
- **purple** (#5c2d91) - Creative, premium, unique offerings
- **orange** (#d83b01) - Energy, action, urgency
- **green** (#2e7d32) - Nature, growth, sports, sustainability
```

---

## OUTPUT FORMAT

Generate all sections in order, using clear separators:

```
================================================================================
SECTION 1: PROJECT IDENTITY
================================================================================
[Content]

================================================================================
SECTION 2: COMPLETE TECH STACK
================================================================================
[Content]

[Continue for all 16 sections]
```

---

## QUALITY CHECKLIST

Before finalizing, verify:

- [ ] All sections are completed thoroughly
- [ ] Technical details are accurate and specific
- [ ] Business value is clearly articulated
- [ ] Features demonstrate senior-level capabilities
- [ ] No placeholder text or TODOs remain
- [ ] Screenshots list covers all impressive features
- [ ] Security section shows enterprise awareness
- [ ] The content would impress a technical hiring manager
- [ ] The content would convince a potential client

---

## EXAMPLE OUTPUT REFERENCE

For reference, here's the level of detail expected. This is from the CarAuctions project:

**Tagline**: "Real-Time Vehicle Auction Platform"

**Highlights** (notice the specificity):
- Real-time bidding with SignalR WebSockets
- Anti-sniping automatic auction extension
- Clean Architecture with 6 projects
- Domain-Driven Design with rich models
- CQRS with pipeline behaviors
- Role-based authorization (RBAC)

Each highlight mentions a **specific technology or pattern**, not vague claims like "well-architected" or "scalable".

---

**Remember**: The goal is to extract MAXIMUM information about this project. Be exhaustive. The portfolio demo should comprehensively showcase every impressive aspect of the work.
