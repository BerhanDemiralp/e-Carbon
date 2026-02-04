# e-Carbon AI

A production-level carbon emissions tracking and reporting system built with Vertical Slice Architecture.

## 🎯 Project Overview

e-Carbon AI is a corporate carbon emissions (CO₂e) calculation, snapshot, reporting, and storage system designed for monthly reporting in a traceable and auditable way.

### Key Features

- **Deterministic Calculation** - Same input always produces the same output
- **Monthly Snapshots** - Immutable freeze of monthly carbon data
- **Audit Trail** - Immutable logging of all critical actions
- **PDF Reports with Hash Integrity** - SHA-256 verification for tamper-proof reports
- **Vertical Slice Architecture** - Feature-based, maintainable codebase

## 🏗️ Architecture

This project follows **Vertical Slice Architecture (VSA)** where each feature is self-contained with its own:
- Endpoints
- Commands/Queries
- Handlers
- Validators
- Response models

### Technology Stack

- **.NET 9** - Web API
- **PostgreSQL 16** - Database
- **Entity Framework Core 9** - ORM
- **MediatR** - CQRS & Mediator pattern
- **FluentValidation** - Input validation
- **QuestPDF** - PDF generation
- **JWT** - Authentication
- **Swagger/OpenAPI** - API documentation

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### 1. Clone and Setup

```bash
git clone <repository-url>
cd eCarbon
```

### 2. Start PostgreSQL Database

```bash
docker-compose up -d
```

This will start:
- PostgreSQL on port 5432
- pgAdmin on port 5050 (http://localhost:5050)
  - Email: admin@ecarbon.com
  - Password: admin123

### 3. Run the Application

```bash
cd src/eCarbon.Api
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: http://localhost:5000/swagger

### 4. Database Migrations

Migrations are automatically applied when the application starts. To create a new migration:

```bash
cd src/eCarbon.Api
dotnet ef migrations add MigrationName --output-dir Common/Persistence/Migrations
```

## 📁 Project Structure

```
eCarbon/
├── src/
│   └── eCarbon.Api/                  # Main API project
│       ├── Common/                   # Cross-cutting concerns
│       │   ├── Auth/
│       │   ├── Behaviors/           # MediatR pipeline behaviors
│       │   ├── Exceptions/
│       │   ├── Middleware/
│       │   └── Persistence/         # DbContext & configurations
│       ├── Domain/                   # Domain entities & enums
│       │   ├── Entities/
│       │   └── Enums/
│       └── Features/                 # Vertical Slices
│           ├── Auth/
│           ├── Companies/
│           ├── Facilities/
│           ├── ActivityRecords/
│           ├── Calculations/
│           ├── Snapshots/
│           ├── Reports/
│           └── AuditLogs/
├── docker-compose.yml
└── README.md
```

## 🔌 API Endpoints

### Authentication
- `POST /api/auth/login` - Login and get JWT token

### Companies
- `POST /api/companies` - Create company
- `GET /api/companies` - List all companies
- `GET /api/companies/{id}` - Get company by ID
- `PUT /api/companies/{id}` - Update company
- `DELETE /api/companies/{id}` - Delete company

### Facilities
- `POST /api/companies/{companyId}/facilities` - Create facility
- `GET /api/companies/{companyId}/facilities` - List facilities by company

### Activity Records
- `POST /api/activity-records` - Create activity record
- `GET /api/facilities/{facilityId}/activity-records` - List records by facility

### Calculations
- `GET /api/companies/{companyId}/months/{month}/preview` - Preview monthly emissions

### Snapshots
- `POST /api/companies/{companyId}/snapshots` - Create monthly snapshot
- `GET /api/snapshots/{snapshotId}` - Get snapshot
- `POST /api/snapshots/{snapshotId}/freeze` - Freeze snapshot (make immutable)

### Reports
- `POST /api/snapshots/{snapshotId}/reports` - Generate PDF report
- `GET /api/reports/{reportId}/download` - Download PDF report
- `POST /api/reports/{reportId}/verify` - Verify report hash integrity

### Audit Logs
- `GET /api/audit-logs` - List audit logs

## 🧪 Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run with verbosity
dotnet test --verbosity normal
```

### Database Connection

Default connection string (configured in `appsettings.json`):
```
Host=localhost;Port=5432;Database=ecarbon_db;Username=ecarbon;Password=ecarbon123
```

### Environment Variables

You can override configuration using environment variables:

```bash
export ConnectionStrings__DefaultConnection="your-connection-string"
export Jwt__SecretKey="your-secret-key"
```

## 📊 Data Model

### Core Entities

1. **Company** - Organization entity
2. **Facility** - Company locations
3. **EmissionFactor** - CO₂e factors by activity type, unit, year, and region
4. **ActivityRecord** - Raw consumption data
5. **MonthlySnapshot** - Frozen monthly carbon totals
6. **SnapshotItem** - Copied records inside snapshots
7. **Report** - Generated PDF with hash
8. **AuditLog** - Immutable action history

### Calculation Flow

1. **Record Activities** - Log consumption data (diesel, electricity, etc.)
2. **Preview** - Calculate emissions using current factors
3. **Create Snapshot** - Copy records with frozen factors
4. **Freeze** - Make snapshot immutable
5. **Generate Report** - Create PDF with SHA-256 hash
6. **Verify** - Confirm report integrity anytime

## 🔐 Security

- JWT Bearer authentication
- Password: `ecarbon123` (for demo purposes)
- CORS enabled for development
- Input validation with FluentValidation
- Soft deletes for audit compliance

## 📝 Development Plan

See [DevelopmentPlan.md](./DevelopmentPlan.md) for detailed phase-by-phase implementation plan.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Built as a portfolio-grade backend demonstrating enterprise patterns
- Uses industry-standard libraries and best practices
- Designed for extensibility and maintainability

---

**Note**: This is a simulation/demo project for educational and portfolio purposes.