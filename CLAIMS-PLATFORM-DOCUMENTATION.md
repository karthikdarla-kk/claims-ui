# Chubb APAC Claims Platform

## Take-Home Assessment Submission

### Candidate Project
Claims Hub MVP

---

## 1. Executive Summary

Claims Hub is a full-stack claims workflow platform for motor and property claims. It supports the two main user journeys in one Angular application:

- Claimants submit incidents and track progress.
- Claims officers review incoming work, manage assignments, make decisions, and monitor outstanding exposure.

The implementation prioritises the core workflow and a demonstrable local application within the assessment timebox.

## 2. Implemented Scope

### Claimant capabilities

- Submit a motor or property claim.
- Provide claimant contact details, incident date, description, and estimated loss.
- View submitted claims by claimant email.
- Track the current claim status and assigned officer.

### Claims officer capabilities

- View the claims queue.
- View total claims and open claims.
- View outstanding liability exposure.
- Assign a claim to an officer.
- Settle or reject a claim with a decision reason.

### Workflow statuses

1. Submitted
2. In Review
3. Awaiting Information
4. Settled
5. Rejected

## 3. Architecture

```text
Angular SPA
	|
	| REST/HTTPS
	v
ASP.NET Core .NET 8 API
	|
	| Entity Framework Core
	v
SQL Server - ClaimsDb
```

The backend is currently a modular monolith. This is appropriate for the MVP because it keeps the deployment and development model simple while allowing clear separation between the HTTP layer, claim workflow service, domain model, and persistence layer.

### Main components

- `Claims.Api/Domain/Claim.cs` - claim entity and workflow enums.
- `Claims.Api/Features/Claims/ClaimsController.cs` - claim REST endpoints.
- `Claims.Api/Features/Claims/DashboardController.cs` - workload and exposure endpoint.
- `Claims.Api/Features/Claims/ClaimService.cs` - validation and claim workflow operations.
- `Claims.Api/Infrastructure/ClaimsDbContext.cs` - EF Core SQL Server mapping.
- `claims-ui/src/app/app.component.html` - Angular claimant and officer screens.
- `claims-ui/src/app/claim.service.ts` - frontend API client.

## 4. Technology Decisions

### Backend

ASP.NET Core .NET 8 was selected because it is already the project foundation and provides strong support for REST APIs, dependency injection, asynchronous I/O, OpenAPI, and Entity Framework Core.

### Frontend

Angular is required by the assessment and is used as a single application for both user types. A demo role switch changes the screen between claimant and officer workflows. In production, the role would come from authenticated identity claims rather than a client-side switch.

### Database

SQL Server is used with Entity Framework Core. The development configuration connects to the local SQL Server instance using Windows Integrated Security and the `ClaimsDb` database.

### Communication

REST is used for synchronous user actions and queries because the claimant and officer need an immediate response to commands such as submitting, assigning, or deciding a claim.

In production, Kafka would be appropriate for asynchronous domain events such as:

- `ClaimSubmitted`
- `ClaimAssigned`
- `ClaimDecisionMade`
- `AdditionalInformationRequested`

Those events could drive notifications, audit processing, analytics, and integration with downstream systems without coupling those processes to the request-response path.

## 5. API Contract

| Method | Endpoint | Purpose |
|---|---|---|
| GET | `/api/claims` | List claims, optionally filtered by email or status |
| GET | `/api/claims/{id}` | Retrieve a claim |
| POST | `/api/claims` | Submit a new claim |
| PATCH | `/api/claims/{id}` | Assign or progress a claim |
| GET | `/api/dashboard` | Return workload and liability metrics |

### Example claim submission

```json
{
  "claimantName": "Jordan Lee",
  "claimantEmail": "jordan@example.com",
  "type": "Motor",
  "incidentDate": "2026-03-01",
  "description": "Vehicle damaged in a car park.",
  "estimatedLoss": 2500
}
```

### Example claim update

```json
{
  "status": "Settled",
  "assignedOfficer": "Demo Officer",
  "decisionReason": "Damage assessment accepted."
}
```

## 6. Liability Exposure

Outstanding exposure is calculated from claims that are not settled or rejected. The dashboard returns:

- Total claims.
- Open claims.
- Sum of estimated losses for open claims.
- Claim counts grouped by status.

This provides managers with an immediate view of workload and potential liability.

## 7. Running the Application

### Start the backend

```powershell
dotnet run --project Claims.Api\Claims.Api.csproj
```

The API normally runs at `https://localhost:7185`. Swagger is available at:

```text
https://localhost:7185/swagger
```

At startup, the API creates the `ClaimsDb` database if it does not exist and inserts demo claims when the database is empty.

### Start the Angular application

Install Node.js 18 or later, then run:

```powershell
cd claims-ui
npm install
npm start
```

Open `http://localhost:4200`.

## 8. Suggested Walkthrough

1. Open the claimant view.
2. Submit a new motor claim.
3. Switch to officer view.
4. Show the increase in open claims and outstanding exposure.
5. Assign the claim to the demo officer.
6. Settle or reject the claim.
7. Return to claimant view and show the updated status.
8. Open Swagger and demonstrate the REST contract.

## 9. Security and Production Considerations

The role switch is only a demonstration mechanism. A production solution would use OIDC or OAuth2 with JWT access tokens and server-side authorization policies.

Additional production concerns include:

- Input validation and anti-fraud checks.
- Malware scanning for uploaded documents.
- Blob storage for claim evidence.
- Immutable audit history.
- Optimistic concurrency handling.
- EF Core migrations managed through deployment pipelines.
- Secrets stored in a managed secret store.
- Structured logging, tracing, and alerting.
- Kafka event publishing through an outbox pattern.

## 10. Prioritisation and Trade-offs

The most important workflow was implemented first: claim submission, visibility, officer action, and exposure reporting.

The following items were intentionally left for future work to stay within the assessment timebox:

- Full authentication and authorization.
- Document upload and storage.
- Additional-information request workflow.
- Notifications.
- Kafka integration.
- Full audit history.
- Automated API and UI test coverage.
- Production deployment infrastructure.

## 11. Future Roadmap

1. Add authenticated claimant and officer accounts.
2. Add document upload with secure object storage.
3. Add an audit timeline to each claim.
4. Add information requests and claimant responses.
5. Publish workflow events through Kafka.
6. Add automated tests and contract testing.
7. Add observability and deployment automation.

## 12. AI Collaboration

The AI working process is documented in `AI-WORKING-JOURNAL.md`. It records accepted decisions, constraints, overridden recommendations, and the reasoning behind the implementation choices.
