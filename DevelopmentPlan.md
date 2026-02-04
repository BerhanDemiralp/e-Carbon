# **e-Carbon AI – Development Plan**
## *Vertical Slice Architecture Implementation*

---

## **1) Architecture Overview**

### 1.1 Why Vertical Slice Architecture?

- **Feature-focused**: Each feature is self-contained and independently deployable
- **Reduced coupling**: Changes to one feature don't cascade to others
- **Easier maintenance**: All code for a feature lives in one place
- **Scalable teams**: Multiple developers can work on different features simultaneously

### 1.2 Technology Stack

| Component     | Technology                    |
|---------------|-------------------------------|
| API Framework | ASP.NET Core Minimal API      |
| Language      | C# 12 (.NET 9)                |
| Pattern       | Vertical Slice Architecture   |
| Mediator      | MediatR                       |
| Validation    | FluentValidation              |
| ORM           | Entity Framework Core 9       |
| Database      | PostgreSQL 16                 |
| PDF           | QuestPDF                      |
| Testing       | xUnit + Testcontainers        |
| Auth          | JWT Bearer                    |
| Documentation | Swagger/OpenAPI               |

---

## **2) Project Structure**

```
eCarbon/
│
├── src/
│   └── eCarbon.Api/
│       ├── Program.cs
│       ├── appsettings.json
│       │
│       ├── Common/                          # Cross-cutting concerns only
│       │   ├── Auth/
│       │   │   ├── ICurrentUserService.cs
│       │   │   └── JwtExtensions.cs
│       │   ├── Behaviors/
│       │   │   ├── ValidationBehavior.cs
│       │   │   └── AuditingBehavior.cs
│       │   ├── Exceptions/
│       │   │   ├── NotFoundException.cs
│       │   │   └── ValidationException.cs
│       │   ├── Middleware/
│       │   │   ├── ErrorHandlingMiddleware.cs
│       │   │   └── RequestTimingMiddleware.cs
│       │   └── Persistence/
│       │       ├── AppDbContext.cs
│       │       ├── Configurations/
│       │       └── Migrations/
│       │
│       ├── Domain/                          # Shared domain entities
│       │   ├── Entities/
│       │   │   ├── Company.cs
│       │   │   ├── Facility.cs
│       │   │   ├── ActivityRecord.cs
│       │   │   ├── EmissionFactor.cs
│       │   │   ├── MonthlySnapshot.cs
│       │   │   ├── SnapshotItem.cs
│       │   │   ├── Report.cs
│       │   │   └── AuditLog.cs
│       │   └── Enums/
│       │       ├── ActivityType.cs
│       │       └── ScopeType.cs
│       │
│       └── Features/                        # VERTICAL SLICES
│           │
│           ├── Auth/
│           │   └── Login/
│           │       ├── LoginEndpoint.cs
│           │       ├── LoginCommand.cs
│           │       ├── LoginHandler.cs
│           │       ├── LoginRequest.cs
│           │       ├── LoginResponse.cs
│           │       └── LoginValidator.cs
│           │
│           ├── Companies/
│           │   ├── CreateCompany/
│           │   │   ├── CreateCompanyEndpoint.cs
│           │   │   ├── CreateCompanyCommand.cs
│           │   │   ├── CreateCompanyHandler.cs
│           │   │   ├── CreateCompanyRequest.cs
│           │   │   ├── CreateCompanyResponse.cs
│           │   │   └── CreateCompanyValidator.cs
│           │   ├── GetCompany/
│           │   ├── ListCompanies/
│           │   ├── UpdateCompany/
│           │   └── DeleteCompany/
│           │
│           ├── Facilities/
│           │   ├── CreateFacility/
│           │   ├── ListFacilitiesByCompany/
│           │   └── GetFacility/
│           │
│           ├── ActivityRecords/
│           │   ├── CreateActivityRecord/
│           │   ├── ListActivityRecordsByFacility/
│           │   ├── UpdateActivityRecord/
│           │   └── DeleteActivityRecord/
│           │
│           ├── Calculations/
│           │   └── PreviewMonthlyEmissions/
│           │       ├── PreviewEndpoint.cs
│           │       ├── PreviewQuery.cs
│           │       ├── PreviewHandler.cs
│           │       ├── EmissionsCalculator.cs
│           │       ├── PreviewResponse.cs
│           │       └── EmissionsBreakdownDto.cs
│           │
│           ├── Snapshots/
│           │   ├── CreateSnapshot/
│           │   │   ├── CreateSnapshotEndpoint.cs
│           │   │   ├── CreateSnapshotCommand.cs
│           │   │   ├── CreateSnapshotHandler.cs
│           │   │   ├── SnapshotBuilder.cs
│           │   │   └── SnapshotCreatedResponse.cs
│           │   ├── GetSnapshot/
│           │   └── FreezeSnapshot/
│           │
│           ├── Reports/
│           │   ├── GenerateReport/
│           │   │   ├── GenerateReportEndpoint.cs
│           │   │   ├── GenerateReportCommand.cs
│           │   │   ├── GenerateReportHandler.cs
│           │   │   ├── ReportBuilder.cs
│           │   │   ├── PdfGenerator.cs
│           │   │   ├── HashService.cs
│           │   │   └── ReportResponse.cs
│           │   ├── DownloadReport/
│           │   └── VerifyReport/
│           │
│           └── AuditLogs/
│               └── ListAuditLogs/
│                   ├── ListAuditLogsEndpoint.cs
│                   ├── ListAuditLogsQuery.cs
│                   ├── ListAuditLogsHandler.cs
│                   └── AuditLogDto.cs
│
└── tests/
    ├── eCarbon.UnitTests/
    │   └── Features/
    │       ├── Companies/
    │       │   └── CreateCompany/
    │       │       ├── CreateCompanyHandlerTests.cs
    │       │       └── CreateCompanyValidatorTests.cs
    │       ├── Calculations/
    │       └── Reports/
    │
    └── eCarbon.IntegrationTests/
        ├── TestBase.cs
        ├── WebApplicationFactory.cs
        ├── EndToEndWorkflowTests.cs
        └── Features/
            └── SnapshotWorkflowTests.cs

```

---

## **3) Development Phases**

### **Phase 1: Foundation & Core Setup** (Week 1)

#### Day 1-2: Project Bootstrap
- [x] Initialize .NET 9 Minimal API project (updated from .NET 10)
- [x] Configure MediatR, FluentValidation, EF Core
- [x] Setup PostgreSQL with Docker Compose
- [x] Create base entity classes in `Domain/Entities/`
- [x] Configure DbContext with all entity mappings

#### Day 3-4: Infrastructure & Cross-Cutting
- [x] Implement `ValidationBehavior` and `AuditingBehavior` (MediatR pipelines)
- [x] Create `ErrorHandlingMiddleware` and `RequestTimingMiddleware`
- [ ] Setup JWT authentication in `Common/Auth/` (pending - will implement with first slice)
- [x] Create initial EF Core migration

#### Day 5-7: First Vertical Slices - Companies & Auth
**Implement 5 slices:**

1. [ ] **Auth/Login/** - JWT token generation
2. [ ] **Companies/CreateCompany/** - Full CRUD with validation
3. [ ] **Companies/GetCompany/**
4. [ ] **Companies/ListCompanies/**
5. [ ] **Companies/UpdateCompany/** (soft delete approach)

**Each slice includes:**
- [ ] Endpoint registration in Program.cs
- [ ] Request/Response DTOs
- [ ] Command/Query + Handler
- [ ] FluentValidation validator
- [ ] Integration with audit logging

---

### **Phase 2: Facilities, Activities & Calculation** (Week 2)

#### Day 8-9: Facilities
**Implement 3 slices:**
- [ ] **Facilities/CreateFacility/**
- [ ] **Facilities/ListFacilitiesByCompany/**
- [ ] **Facilities/GetFacility/**

#### Day 10-11: Activity Records
**Implement 4 slices:**
- [ ] **ActivityRecords/CreateActivityRecord/**
- [ ] **ActivityRecords/ListActivityRecordsByFacility/**
- [ ] **ActivityRecords/UpdateActivityRecord/**
- [ ] **ActivityRecords/DeleteActivityRecord/**

#### Day 12-14: Calculation Engine
**Implement 1 complex slice:**
- [ ] **Calculations/PreviewMonthlyEmissions/**
  - [ ] Query handler that:
    - [ ] Fetches all activities for company + month
    - [ ] Matches with emission factors (activity type + unit + year + region)
    - [ ] Calculates CO₂e per activity
    - [ ] Aggregates by Scope (1 vs 2)
    - [ ] Returns preview totals

- [ ] **Seed emission factors** with realistic data (Diesel, NaturalGas, Electricity)

---

### **Phase 3: Snapshot System** (Week 3)

#### Day 15-17: Snapshot Creation
**Implement 3 slices:**
- [ ] **Snapshots/CreateSnapshot/**
  - [ ] Handler copies all activity records for month
  - [ ] Creates snapshot_items with frozen factor values
  - [ ] Calculates totals (scope1, scope2, total)
  - [ ] Stores in monthly_snapshots table
  - [ ] Logs action to audit_logs

- [ ] **Snapshots/GetSnapshot/**
  - [ ] Returns snapshot with all items
  - [ ] Read-only view

- [ ] **Snapshots/FreezeSnapshot/**
  - [ ] Changes status from Draft → Frozen
  - [ ] Makes immutable (adds business rule validation)

#### Day 18-19: Constraints & Validation
- [ ] Add unique constraint: one snapshot per company per month
- [ ] Prevent recalculation of frozen snapshots
- [ ] Add validation: can't snapshot future months
- [ ] Add validation: can't snapshot months with no activities

#### Day 20: Integration Testing
- [ ] End-to-end test: Create activities → Preview → Create Snapshot → Freeze

---

### **Phase 4: Reporting & Verification** (Week 4)

#### Day 21-22: PDF Generation
**Implement slice:**
- [ ] **Reports/GenerateReport/**
  - [ ] Handler:
    - [ ] Fetches frozen snapshot
    - [ ] Generates PDF using QuestPDF
    - [ ] Saves to filesystem (or storage)
    - [ ] Calculates SHA-256 hash
    - [ ] Stores report metadata in database
    - [ ] Logs to audit

**PDF Contents:**
- [ ] Company header
- [ ] Report period
- [ ] Scope 1 total (kg CO₂e)
- [ ] Scope 2 total (kg CO₂e)
- [ ] Grand total
- [ ] Activity breakdown table
- [ ] Generation timestamp
- [ ] Report ID and hash

#### Day 23: Download & Verification
**Implement 2 slices:**
- [ ] **Reports/DownloadReport/** - Stream PDF file
- [ ] **Reports/VerifyReport/**
  - [ ] Re-reads PDF from storage
  - [ ] Re-calculates SHA-256
  - [ ] Compares with stored hash
  - [ ] Returns match/no-match result

#### Day 24-25: Audit Logs & Polish
**Implement slice:**
- [ ] **AuditLogs/ListAuditLogs/**
  - [ ] Query with filtering (by entity, date range, actor)
  - [ ] Pagination support

**Final tasks:**
- [ ] Add Swagger documentation
- [ ] Create seed data script
- [ ] Write README with API examples

#### Day 26-28: Testing
- [ ] Unit tests for calculation logic
- [ ] Unit tests for validators
- [ ] Integration tests with Testcontainers
- [ ] End-to-end workflow verification

---

## **4) Implementation Patterns**

### 4.1 Standard Slice Template

Each feature follows this pattern:

```csharp
// FeatureNameEndpoint.cs
public static class FeatureNameEndpoint
{
    public static IEndpointRouteBuilder MapFeatureName(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/resource", async (
            FeatureNameRequest request,
            IMediator mediator,
            CancellationToken ct) =>
        {
            var command = request.ToCommand();
            var result = await mediator.Send(command, ct);
            return Results.Created($"/api/resource/{result.Id}", result);
        });
        return app;
    }
}

// FeatureNameCommand.cs
public record FeatureNameCommand(string Property1, int Property2) : IRequest<FeatureNameResponse>;

// FeatureNameHandler.cs
public class FeatureNameHandler : IRequestHandler<FeatureNameCommand, FeatureNameResponse>
{
    private readonly AppDbContext _db;
    private readonly IAuditService _audit;
    
    public async Task<FeatureNameResponse> Handle(
        FeatureNameCommand request, 
        CancellationToken ct)
    {
        // Business logic here
    }
}

// FeatureNameValidator.cs
public class FeatureNameValidator : AbstractValidator<FeatureNameCommand>
{
    public FeatureNameValidator()
    {
        RuleFor(x => x.Property1).NotEmpty();
    }
}
```

### 4.2 Registration in Program.cs

```csharp
// Features are registered by scanning assembly
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuditingBehavior<,>));
});

// Register all endpoints
app.MapAuth();
app.MapCompanies();
app.MapFacilities();
app.MapActivityRecords();
app.MapCalculations();
app.MapSnapshots();
app.MapReports();
app.MapAuditLogs();
```

---

## **5) Key Technical Decisions**

| Decision | Choice | Rationale |
|----------|--------|-----------|
| **PDF Library** | QuestPDF | Modern, C#-native, better API than iText7 |
| **Authentication** | JWT Bearer | Stateless, works well with Minimal APIs |
| **Audit Strategy** | MediatR Pipeline Behavior | Automatic, cross-cutting, no scatter |
| **Soft Deletes** | Yes (IsDeleted flag) | Audit compliance, data recovery |
| **File Storage** | Local filesystem + path in DB | Simple for MVP, easily migratable |
| **Emission Factors** | Pre-seeded JSON | ~20 realistic factors covering common activities |
| **Validation** | FluentValidation + Pipeline | Clean separation, reusable rules |

---

## **6) MVP Scope (Minimum Viable Product)**

To deliver a functional demo quickly:

**Core Flow:**
1. [ ] Create Company → Create Facility
2. [ ] Add Activity Records (diesel, electricity, gas)
3. [ ] Preview monthly emissions (calculation)
4. [ ] Create & Freeze Snapshot
5. [ ] Generate PDF Report with hash
6. [ ] Verify report integrity

**Out of MVP Scope (Future):**
- [ ] User management (multi-tenant)
- [ ] Role-based permissions
- [ ] Complex emission factor versioning
- [ ] Cloud storage for PDFs
- [ ] Advanced reporting charts
- [ ] Email notifications

---

## **7) Progress Tracking**

### **Current Phase:** Phase 1 - Foundation & Core Setup
### **Current Task:** Implement first vertical slices (Auth & Companies)

### **Completed Tasks:**
- [x] Created development plan (this document)
- [x] Initialized .NET 9 project (updated from .NET 10)
- [x] Added all required NuGet packages (MediatR, FluentValidation, EF Core, PostgreSQL, JWT, QuestPDF, Swagger)
- [x] Created Docker Compose configuration for PostgreSQL and pgAdmin
- [x] Created complete folder structure for Vertical Slice Architecture
- [x] Created all domain entities (8 entities)
- [x] Created all entity configurations with indexes and constraints
- [x] Configured DbContext with entity mappings
- [x] Created initial EF Core migration
- [x] Implemented ValidationBehavior and AuditingBehavior (MediatR pipelines)
- [x] Implemented ErrorHandlingMiddleware and RequestTimingMiddleware
- [x] Configured Program.cs with all services and middleware
- [x] Created comprehensive README.md

### **In Progress:**
- Implementing first vertical slices (Auth/Login and Companies CRUD)

### **Next Steps:**
1. Create Auth/Login slice with JWT authentication
2. Implement Companies/CreateCompany slice
3. Implement Companies/GetCompany, ListCompanies, UpdateCompany, DeleteCompany slices
4. Test API endpoints with Swagger
5. Seed initial data (emission factors)

---

## **8) Deliverables**

By end of Week 4, you will have:

- ✅ Fully functional REST API
- ✅ Swagger documentation
- ✅ PostgreSQL database with migrations
- ✅ PDF report generation with SHA-256 hashing
- ✅ Immutable audit trail
- ✅ Unit & Integration tests
- ✅ Docker Compose setup
- ✅ README with usage examples
