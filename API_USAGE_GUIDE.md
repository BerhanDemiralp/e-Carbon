# e-Carbon AI API Documentation

A comprehensive guide for using the e-Carbon API to track and report carbon emissions.

## Table of Contents

1. [Introduction](#introduction)
2. [Getting Started](#getting-started)
3. [Authentication](#authentication)
4. [Workflow Guide](#workflow-guide)
5. [Endpoint Reference](#endpoint-reference)
6. [Error Reference](#error-reference)
7. [Code Examples](#code-examples)

---

## Introduction

e-Carbon AI is a carbon emissions tracking and reporting system that helps organizations:

- Record carbon emission activities (energy, transportation, fuel, etc.)
- Calculate CO₂e emissions using standard emission factors
- Create immutable monthly snapshots
- Generate verifiable PDF reports with SHA-256 hash verification

### Key Concepts

| Concept | Description |
|---------|-------------|
| **Company** | The organization tracking emissions |
| **Facility** | A physical location under a company (factory, office, etc.) |
| **Activity Record** | A single emission activity (e.g., 100L diesel used) |
| **Emission Factor** | CO₂e multiplier per unit (e.g., 2.68 kg CO₂e/L for diesel) |
| **Scope 1** | Direct emissions (fuel combustion, company vehicles) |
| **Scope 2** | Indirect emissions (purchased electricity, heat) |
| **Snapshot** | Frozen monthly record with immutable data |
| **Report** | PDF document with SHA-256 hash for verification |

---

## Getting Started

### Base URL

```
http://localhost:5000
```

### Content Type

All requests and responses use JSON:

```http
Content-Type: application/json
```

### API Endpoints Overview

| Category | Endpoints |
|----------|-----------|
| Companies | Create, Get, List, Update, Delete |
| Facilities | Create, Get, List (by company/all), Update |
| Activity Records | Create, List (by facility/all), Update, Delete |
| Calculations | Preview Monthly Emissions |
| Snapshots | Create, Get, List, Freeze |
| Reports | Generate, Download, Verify, List |
| Audit Logs | List (with filtering) |

---

## Authentication

> **Note:** JWT authentication is coming soon. Currently, all endpoints are open.

Future implementation will require:

```bash
# Get token
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@ecarbon.com","password":"ecarbon123"}'

# Use token
curl -X GET http://localhost:5000/api/companies \
  -H "Authorization: Bearer <your-token>"
```

---

## Workflow Guide

### 1. Creating Your First Company

Start by creating a company entity:

```bash
curl -X POST http://localhost:5000/api/companies \
  -H "Content-Type: application/json" \
  -d '{"name":"Green Energy Corp"}'
```

**Response:**
```json
{
  "id": "83090fcf-458d-473f-b103-22d5d94be2d5",
  "name": "Green Energy Corp",
  "createdAt": "2026-02-07T21:05:13.438857Z",
  "facilityCount": 0
}
```

### 2. Adding Facilities

Create facilities under your company:

```bash
curl -X POST http://localhost:5000/api/companies/83090fcf-458d-473f-b103-22d5d94be2d5/facilities \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Solar Plant A",
    "address": "123 Energy Ave",
    "type": "Manufacturing"
  }'
```

**Response:**
```json
{
  "id": "ae1e276d-7f2c-47bd-a5c2-d7e93ba115e3",
  "companyId": "83090fcf-458d-473f-b103-22d5d94be2d5",
  "name": "Solar Plant A",
  "address": "123 Energy Ave",
  "type": "Manufacturing",
  "createdAt": "2026-02-07T21:05:14.000000Z"
}
```

### 3. Recording Activities

Record emission activities for your facilities:

**Available Activity Types:**
| Type | Unit | Scope |
|------|------|-------|
| Diesel | L | 1 |
| Gasoline | L | 1 |
| NaturalGas | m³ | 1 |
| Propane | L | 1 |
| Coal | kg | 1 |
| Biogas | m³ | 1 |
| Electricity | kWh | 2 |
| Solar | kWh | 2 |
| Wind | kWh | 2 |

**Example: Record diesel consumption**

```bash
curl -X POST http://localhost:5000/api/activity-records \
  -H "Content-Type: application/json" \
  -d '{
    "facilityId": "ae1e276d-7f2c-47bd-a5c2-d7e93ba115e3",
    "activityType": "Diesel",
    "quantity": 50,
    "unit": "L",
    "activityDate": "2026-02-05",
    "scope": 1
  }'
```

**Example: Record electricity usage**

```bash
curl -X POST http://localhost:5000/api/activity-records \
  -H "Content-Type: application/json" \
  -d '{
    "facilityId": "ae1e276d-7f2c-47bd-a5c2-d7e93ba115e3",
    "activityType": "Electricity",
    "quantity": 10000,
    "unit": "kWh",
    "activityDate": "2026-02-01",
    "scope": 2
  }'
```

**Response:**
```json
{
  "id": "107870ac-ff6e-485c-90cc-0c372eb9ea2d",
  "facilityId": "ae1e276d-7f2c-47bd-a5c2-d7e93ba115e3",
  "activityDate": "2026-02-01T10:00:00Z",
  "activityType": "Electricity",
  "scope": 2,
  "quantity": 10000,
  "unit": "kWh",
  "co2eKg": 500,
  "createdAt": "2026-02-07T21:05:14.000000Z"
}
```

### 4. Preview Monthly Emissions

Before creating a snapshot, preview your emissions:

```bash
curl -X GET "http://localhost:5000/api/companies/83090fcf-458d-473f-b103-22d5d94be2d5/months/2026-02/preview"
```

**Response:**
```json
{
  "companyId": "83090fcf-458d-473f-b103-22d5d94be2d5",
  "companyName": "Green Energy Corp",
  "month": "2026-02",
  "scope1TotalKg": 134,
  "scope2TotalKg": 500,
  "totalKg": 634,
  "breakdown": [
    {
      "activityRecordId": "107870ac-ff6e-485c-90cc-0c372eb9ea2d",
      "facilityName": "Solar Plant A",
      "activityDate": "2026-02-01T10:00:00Z",
      "activityType": "Electricity",
      "scope": 2,
      "quantity": 10000,
      "unit": "kWh",
      "factorKgPerUnit": 0.05,
      "co2eKg": 500
    },
    {
      "activityRecordId": "2e774eca-9606-4f2f-a72e-3b0075d36d5d",
      "facilityName": "Solar Plant A",
      "activityDate": "2026-02-05T14:00:00Z",
      "activityType": "Diesel",
      "scope": 1,
      "quantity": 50,
      "unit": "L",
      "factorKgPerUnit": 2.68,
      "co2eKg": 134
    }
  ]
}
```

### 5. Create Monthly Snapshot

Once satisfied with the preview, create an immutable snapshot:

```bash
curl -X POST http://localhost:5000/api/companies/83090fcf-458d-473f-b103-22d5d94be2d5/snapshots \
  -H "Content-Type: application/json" \
  -d '{"month": "2026-02"}'
```

**Response:**
```json
{
  "id": "a783f831-5eda-4d55-8141-87d4541c9e64",
  "companyId": "83090fcf-458d-473f-b103-22d5d94be2d5",
  "month": "2026-02",
  "status": "Draft",
  "scope1TotalKg": 134,
  "scope2TotalKg": 500,
  "totalKg": 634,
  "createdAt": "2026-02-07T21:45:56.238401Z",
  "itemCount": 2
}
```

### 6. Freeze Snapshot

Freeze the snapshot to make it immutable:

```bash
curl -X POST http://localhost:5000/api/snapshots/a783f831-5eda-4d55-8141-87d4541c9e64/freeze
```

**Response:**
```json
{
  "id": "a783f831-5eda-4d55-8141-87d4541c9e64",
  "companyId": "83090fcf-458d-473f-b103-22d5d94be2d5",
  "month": "2026-02",
  "status": "Frozen",
  "scope1TotalKg": 134,
  "scope2TotalKg": 500,
  "totalKg": 634,
  "createdAt": "2026-02-07T21:45:56.238401Z",
  "frozenAt": "2026-02-07T21:55:50.544679Z"
}
```

### 7. Generate PDF Report

Generate an official PDF report with SHA-256 hash:

```bash
curl -X POST http://localhost:5000/api/snapshots/a783f831-5eda-4d55-8141-87d4541c9e64/reports
```

**Response:**
```json
{
  "id": "1b7e3ea7-c7c7-4ba6-850b-4ec00e41a5e9",
  "snapshotId": "a783f831-5eda-4d55-8141-87d4541c9e64",
  "status": "Generated",
  "createdAt": "2026-02-08T19:51:49.3612254Z",
  "hashAlgorithm": "SHA-256",
  "hashValue": "5d299edb326bc3fef80f73658ff1aa814c060d744c611ef0cebf56531db8afb4"
}
```

### 8. Download Report

Download the PDF report:

```bash
curl -o "CarbonReport_Green_Energy_Corp_2026_02.pdf" \
  http://localhost:5000/api/reports/1b7e3ea7-c7c7-4ba6-850b-4ec00e41a5e9/download
```

### 9. Verify Report Integrity

Verify that the PDF hasn't been tampered with:

```bash
curl http://localhost:5000/api/reports/1b7e3ea7-c7c7-4ba6-850b-4ec00e41a5e9/verify
```

**Response:**
```json
{
  "id": "1b7e3ea7-c7c7-4ba6-850b-4ec00e41a5e9",
  "snapshotId": "a783f831-5eda-4d55-8141-87d4541c9e64",
  "hashAlgorithm": "SHA-256",
  "storedHash": "5d299edb326bc3fef80f73658ff1aa814c060d744c611ef0cebf56531db8afb4",
  "computedHash": "5d299edb326bc3fef80f73658ff1aa814c060d744c611ef0cebf56531db8afb4",
  "isValid": true,
  "verifiedAt": "2026-02-08T19:51:55.7387289Z"
}
```

---

## Endpoint Reference

### Companies

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/companies` | Create a new company |
| GET | `/api/companies` | List all companies |
| GET | `/api/companies/{id}` | Get company by ID |
| PUT | `/api/companies/{id}` | Update company |
| DELETE | `/api/companies/{id}` | Soft delete company |

**Create Company Request:**
```json
{
  "name": "Acme Corporation"
}
```

**Create Company Response:**
```json
{
  "id": "83090fcf-458d-473f-b103-22d5d94be2d5",
  "name": "Acme Corporation",
  "createdAt": "2026-02-07T21:05:13.438857Z",
  "facilityCount": 0
}
```

### Facilities

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/companies/{companyId}/facilities` | Create facility |
| GET | `/api/facilities/{id}` | Get facility by ID |
| GET | `/api/companies/{companyId}/facilities` | List facilities by company |
| GET | `/api/facilities` | List all facilities |
| PUT | `/api/facilities/{id}` | Update facility |

**Create Facility Request:**
```json
{
  "name": "Main Office",
  "address": "123 Main St",
  "type": "Office"
}
```

### Activity Records

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/activity-records` | Create activity record |
| GET | `/api/facilities/{facilityId}/activities` | List activities by facility |
| GET | `/api/activity-records` | List all activities |
| PUT | `/api/activity-records/{id}` | Update activity record |
| DELETE | `/api/activity-records/{id}` | Soft delete activity record |

**Create Activity Record Request:**
```json
{
  "facilityId": "ae1e276d-7f2c-47bd-a5c2-d7e93ba115e3",
  "activityType": "Electricity",
  "quantity": 5000,
  "unit": "kWh",
  "activityDate": "2026-02-15",
  "scope": 2
}
```

### Calculations

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/companies/{companyId}/months/{month}/preview` | Preview monthly emissions |

### Snapshots

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/companies/{companyId}/snapshots` | Create snapshot |
| GET | `/api/snapshots` | List all snapshots |
| GET | `/api/snapshots/{id}` | Get snapshot by ID |
| POST | `/api/snapshots/{id}/freeze` | Freeze snapshot |

### Reports

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/snapshots/{snapshotId}/reports` | Generate report |
| GET | `/api/reports` | List all reports |
| GET | `/api/reports/{id}/download` | Download PDF |
| GET | `/api/reports/{id}/verify` | Verify report hash |

### Audit Logs

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/audit-logs` | List audit logs |

**Query Parameters:**
- `entityType` - Filter by entity type (Company, Facility, etc.)
- `fromDate` - Filter from date (ISO 8601)
- `toDate` - Filter to date (ISO 8601)
- `page` - Page number (default: 1)
- `pageSize` - Page size (default: 50)

```bash
curl "http://localhost:5000/api/audit-logs?entityType=MonthlySnapshot&fromDate=2026-02-01&page=1&pageSize=10"
```

---

## Error Reference

### HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 400 | Bad Request / Validation Error |
| 404 | Resource Not Found |
| 500 | Internal Server Error |

### Error Response Formats

**Validation Error (400):**
```json
{
  "errors": {
    "Name": ["Company name is required"],
    "Email": ["Invalid email format"]
  }
}
```

**Not Found Error (404):**
```json
{
  "error": "Company with id '83090fcf-458d-473f-b103-22d5d94be2d5' was not found."
}
```

**Invalid Operation Error (400):**
```json
{
  "error": "Snapshot is already frozen"
}
```

**Unexpected Error (500):**
```json
{
  "error": "An unexpected error occurred."
}
```

### Common Error Messages

| Message | Cause |
|---------|-------|
| "Company with id '...' was not found." | Invalid company ID |
| "Facility with id '...' was not found." | Invalid facility ID |
| "Snapshot with id '...' was not found." | Invalid snapshot ID |
| "Report with id '...' was not found." | Invalid report ID |
| "Snapshot already exists for company in month YYYY-MM" | Duplicate snapshot |
| "Snapshot must be frozen before generating a report" | Report before freeze |
| "Snapshot is already frozen" | Double freeze attempt |
| "No activity records found for month YYYY-MM" | Empty month snapshot |
| "Invalid month format. Use YYYY-MM." | Incorrect date format |
| "Cannot create snapshot for a future month." | Future month snapshot |
| "Company has no facilities." | Snapshot without facilities |

---

## Code Examples

### JavaScript / Node.js

```javascript
// Create a company
async function createCompany(name) {
  const response = await fetch('http://localhost:5000/api/companies', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name })
  });
  return response.json();
}

// Preview monthly emissions
async function previewEmissions(companyId, month) {
  const response = await fetch(
    `http://localhost:5000/api/companies/${companyId}/months/${month}/preview`
  );
  return response.json();
}

// Create and freeze snapshot
async function createSnapshot(companyId, month) {
  // Create
  const create = await fetch(
    `http://localhost:5000/api/companies/${companyId}/snapshots`,
    {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ month })
    }
  );
  const snapshot = await create.json();
  
  // Freeze
  await fetch(
    `http://localhost:5000/api/snapshots/${snapshot.id}/freeze`,
    { method: 'POST' }
  );
  
  return snapshot;
}
```

### Python

```python
import requests

BASE_URL = "http://localhost:5000/api"

def create_company(name):
    return requests.post(
        f"{BASE_URL}/companies",
        json={"name": name}
    ).json()

def preview_emissions(company_id, month):
    return requests.get(
        f"{BASE_URL}/companies/{company_id}/months/{month}/preview"
    ).json()

def create_and_freeze_snapshot(company_id, month):
    # Create
    create = requests.post(
        f"{BASE_URL}/companies/{company_id}/snapshots",
        json={"month": month}
    )
    snapshot = create.json()
    
    # Freeze
    requests.post(
        f"{BASE_URL}/snapshots/{snapshot['id']}/freeze"
    )
    
    return snapshot
```

### cURL

```bash
# Full workflow
COMPANY_ID=$(curl -s -X POST http://localhost:5000/api/companies \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Corp"}' | jq -r '.id')

FACILITY_ID=$(curl -s -X POST "http://localhost:5000/api/companies/${COMPANY_ID}/facilities" \
  -H "Content-Type: application/json" \
  -d '{"name":"Factory","address":"123 St","type":"Manufacturing"}' | jq -r '.id')

# Create activities
curl -s -X POST http://localhost:5000/api/activity-records \
  -H "Content-Type: application/json" \
  -d "{\"facilityId\":\"${FACILITY_ID}\",\"activityType\":\"Diesel\",\"quantity\":100,\"unit\":\"L\",\"activityDate\":\"2026-02-15\",\"scope\":1}"

# Preview
curl -s "http://localhost:5000/api/companies/${COMPANY_ID}/months/2026-02/preview"

# Create snapshot
SNAPSHOT_ID=$(curl -s -X POST "http://localhost:5000/api/companies/${COMPANY_ID}/snapshots" \
  -H "Content-Type: application/json" \
  -d '{"month":"2026-02"}' | jq -r '.id')

# Freeze
curl -s -X POST "http://localhost:5000/api/snapshots/${SNAPSHOT_ID}/freeze"

# Generate report
REPORT_ID=$(curl -s -X POST "http://localhost:5000/api/snapshots/${SNAPSHOT_ID}/reports" | jq -r '.id')

# Verify
curl -s "http://localhost:5000/api/reports/${REPORT_ID}/verify"
```

---

## Best Practices

### 1. Validation
- Always validate user input before sending to API
- Handle validation errors gracefully in your UI

### 2. Pagination
- Use pagination for list endpoints with large datasets
- Default page size is 50, but can be customized

### 3. Idempotency
- Creating the same snapshot twice will fail (unique constraint)
- Use the preview endpoint before creating snapshots

### 4. Security
- Keep PDF files secure - they contain sensitive emissions data
- Verify reports after download to ensure integrity
- Implement proper authentication when available

### 5. Data Retention
- Snapshots are immutable once frozen
- Use audit logs to track all changes
- Reports can be re-generated if needed

---

## Support

For issues or questions:
- Check the [GitHub Repository](https://github.com/BerhanDemiralp/e-Carbon)
- Review error messages for troubleshooting
- Use audit logs to trace operations

---

## Changelog

| Version | Date | Changes |
|---------|------|---------|
| 1.0.0 | 2026-02-08 | Initial API release |

---

**Generated by e-Carbon AI** - Track your carbon footprint with confidence.
