# **e-Carbon AI – Technical Design and Implementation Report**

## **1) Project Purpose**

### 1.1 Problem Definition

Companies need to calculate, freeze (snapshot), report, and store their **carbon emissions (CO₂e)** on a monthly basis in a traceable and auditable way.

This project:

- Is a **simulation** of a real corporate carbon reporting system
- But is designed at a **production-level architecture**
- Aims to deliver not just a demo, but a **portfolio-grade backend**

### 1.2 Core Objectives

The system must satisfy the following principles:

1. **Deterministic Calculation**
   - Same input → always the same output
   - Even if emission factors change in the future, past reports remain unchanged

2. **Copy-based Monthly Snapshot (Freeze)**
   - All activity records of a given month are copied into a snapshot
   - Original records may change later, but snapshots remain immutable

3. **Immutable Audit Log**
   - All critical actions (create, update, delete, freeze, generate report) are logged
   - Logs cannot be modified or deleted

4. **PDF Report + Hash Integrity**
   - Each snapshot generates a PDF report
   - A SHA-256 hash of the PDF is stored
   - The report can be verified later against the stored hash

5. **API-first Architecture**
   - All operations are performed via REST APIs
   - Frontend is optional

---

## **2) Architecture Overview**

### 2.1 High-Level Architecture

```
Client (Swagger / Postman / Frontend)
        |
        v
ASP.NET Core Minimal API
        |
        v
Application Layer (Handlers + Validators)
        |
        v
Domain + Calculation Engine
        |
        v
Infrastructure Layer
  - PostgreSQL (EF Core)
  - PDF Generator
  - Hash Service
  - Audit Writer
```

### 2.2 Recommended Technology Stack

| Layer      | Technology               |
| ---------- | ------------------------ |
| API        | ASP.NET Core Minimal API |
| Language   | C#                       |
| Runtime    | .NET 10                  |
| Database   | PostgreSQL               |
| ORM        | Entity Framework Core    |
| Validation | FluentValidation         |
| PDF        | QuestPDF / iText7        |
| Hash       | SHA-256                  |
| Testing    | xUnit + Testcontainers   |

---

## **3) Data Model (Tables)**

### 3.1 `companies`

Stores company information.

| Column     | Type      | Description      |
| ---------- | --------- | ---------------- |
| id         | UUID      | Primary Key      |
| name       | text      | Company name     |
| created_at | timestamp | Creation time    |
| updated_at | timestamp | Last update time |

**Index**

- `UX_companies_name` (unique)

---

### 3.2 `facilities`

Represents company facilities.

| Column     | Type                  |
| ---------- | --------------------- |
| id         | UUID                  |
| company_id | UUID (FK → companies) |
| name       | text                  |
| location   | text                  |
| created_at | timestamp             |
| updated_at | timestamp             |

**Index**

- `IX_facilities_company_id`
- `UX_facilities_company_id_name`

---

### 3.3 `emission_factors`

Stores carbon emission factors.

| Column           | Type                                   |
| ---------------- | -------------------------------------- |
| id               | UUID                                   |
| activity_type    | text (Diesel, NaturalGas, Electricity) |
| unit             | text (L, m³, kWh)                      |
| kg_co2e_per_unit | decimal                                |
| source           | text                                   |
| year             | int                                    |
| region           | text                                   |
| is_active        | bool                                   |
| created_at       | timestamp                              |

**Index**

- `UX_factors_activity_type_unit_year_region`

---

### 3.4 `activity_records`

Stores actual consumption data.

| Column        | Type           |
| ------------- | -------------- |
| id            | UUID           |
| facility_id   | UUID           |
| activity_date | date           |
| scope         | smallint (1/2) |
| activity_type | text           |
| quantity      | decimal        |
| unit          | text           |
| created_at    | timestamp      |
| updated_at    | timestamp      |

**Index**

- `IX_activity_facility_date`
- `IX_activity_date`

---

### 3.5 `monthly_snapshots`

Stores frozen monthly carbon data.

| Column          | Type                |
| --------------- | ------------------- |
| id              | UUID                |
| company_id      | UUID                |
| month           | char(7) (YYYY-MM)   |
| status          | text (Draft/Frozen) |
| scope1_total_kg | decimal             |
| scope2_total_kg | decimal             |
| total_kg        | decimal             |
| created_at      | timestamp           |
| frozen_at       | timestamp           |

**Index**

- `UX_snapshot_company_month` (critical)

---

### 3.6 `snapshot_items`

Stores copied records inside a snapshot.

| Column                    | Type      |
| ------------------------- | --------- |
| id                        | UUID      |
| snapshot_id               | UUID      |
| facility_id               | UUID      |
| activity_date             | date      |
| scope                     | smallint  |
| activity_type             | text      |
| quantity                  | decimal   |
| unit                      | text      |
| factor_kg_per_unit        | decimal   |
| co2e_kg                   | decimal   |
| source_activity_record_id | UUID      |
| created_at                | timestamp |

**Index**

- `IX_snapshot_items_snapshot_id`

---

### 3.7 `reports`

Stores generated PDF reports.

| Column         | Type      |
| -------------- | --------- |
| id             | UUID      |
| snapshot_id    | UUID      |
| pdf_path       | text      |
| hash_algorithm | text      |
| hash_value     | text      |
| created_at     | timestamp |

**Index**

- `UX_reports_snapshot_id`

---

### 3.8 `audit_logs`

Immutable audit trail.

| Column      | Type      |
| ----------- | --------- |
| id          | UUID      |
| actor       | text      |
| action      | text      |
| entity_type | text      |
| entity_id   | UUID      |
| summary     | text      |
| created_at  | timestamp |

**Index**

- `IX_audit_entity`

---

## **4) API Design (Endpoint Map)**

### Auth

- `POST /auth/login` → Returns JWT token

### Companies

- `POST /companies`
- `GET /companies`
- `GET /companies/{id}`
- `PUT /companies/{id}`
- `DELETE /companies/{id}`

### Facilities

- `POST /companies/{companyId}/facilities`
- `GET /companies/{companyId}/facilities`

### Activity Records

- `POST /activity-records`
- `GET /facilities/{facilityId}/activity-records`

### Calculation

- `GET /companies/{companyId}/months/{YYYY-MM}/preview`

### Snapshots

- `POST /companies/{companyId}/snapshots`
- `GET /snapshots/{snapshotId}`

### Reports

- `POST /snapshots/{snapshotId}/reports`
- `GET /reports/{reportId}/download`
- `POST /reports/{reportId}/verify`

### Audit

- `GET /audit-logs`

---

## **5) Folder Structure**

```
eCarbon.Api/
│
├── Program.cs
├── appsettings.json
│
├── Common/
│   ├── Auth/
│   ├── Audit/
│   └── Errors/
│
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   └── ValueObjects/
│
├── Infrastructure/
│   ├── Persistence/
│   ├── Reporting/
│   └── Calculation/
│
├── Features/
│   ├── Auth/
│   ├── Companies/
│   ├── Facilities/
│   ├── ActivityRecords/
│   ├── Snapshots/
│   ├── Reports/
│   └── AuditLogs/
│
└── Tests/
    ├── Unit/
    └── Integration/
```

---

## **6) Development Plan (Week-by-Week)**

### **Week 1 – Core Setup**

- Set up .NET 10 + Minimal API
- Configure PostgreSQL with Docker
- Implement Company & Facility CRUD
- Add JWT Authentication
- Build Audit infrastructure

### **Week 2 – Activity & Calculation**

- Implement ActivityRecord CRUD
- Seed emission factors
- Implement calculation engine
- Add preview endpoint

### **Week 3 – Snapshot**

- Implement snapshot creation
- Copy activity records into snapshot_items
- Add unique constraint
- Log snapshot actions in audit table

### **Week 4 – PDF & Verification**

- Integrate PDF generator
- Implement SHA-256 hashing
- Add verification endpoint
- Write README and demo workflow
