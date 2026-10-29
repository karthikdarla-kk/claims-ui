# Claims Hub MVP

A time-boxed full-stack claims workflow prototype for the Chubb APAC take-home assessment.

## What is included

- .NET 8 REST API in `Claims.Api`
  - Submit and list claims
  - Filter claimant claims by email or status
  - Assign claims to an officer
  - Progress claims through review, settlement, or rejection
  - Dashboard counts and outstanding open exposure
  - Swagger/OpenAPI in development
  - Seed data for an immediate walkthrough
- Angular application in `claims-ui`
  - Claimant view: report and track claims
  - Officer view: workload metrics and queue actions
  - Single application with a demo role switch

## Run locally

### API

```powershell
dotnet run --project Claims.Api\Claims.Api.csproj
```

The API uses the HTTPS URL shown by ASP.NET Core, normally `https://localhost:7185`. Swagger is available at `/swagger`.

### Angular UI

Install Node.js 18+ and npm, then:

```powershell
cd claims-ui
npm install
npm start
```

Open the URL printed by Angular, normally `http://localhost:4200`.

The current development environment did not have Node.js/npm installed, so the Angular build could not be run here.

## Walkthrough

1. Open claimant view and submit a motor or property claim.
2. Switch to officer view; the new claim appears in the queue and increases open exposure.
3. Assign it to the demo officer, then settle or reject it.
4. Return to claimant view and use the matching email to track the status.
5. Open Swagger to show the REST contract and repeat operations independently.

## Architecture and trade-offs

The API is a modular monolith with a claim workflow service. This is intentionally smaller than separate claim, identity, notification, and reporting services for a 2–3 hour exercise. The claim service boundary can later be extracted without changing the HTTP contract.

REST is used for user-driven commands and queries because the UI needs immediate validation and responses. In a production system, domain events such as `ClaimSubmitted`, `ClaimAssigned`, and `ClaimDecided` would be published to Kafka for notifications, audit, analytics, and downstream integrations. Kafka is not included in the MVP to avoid infrastructure obscuring the core workflow.

The API uses SQL Server persistence with EF Core. `EnsureCreated` keeps the assessment setup simple; a production deployment should use reviewed migrations, optimistic concurrency, an outbox, immutable audit history, proper authentication/authorization, document storage, and a background notification process.

The role switch is a demonstration substitute, not security. Production access would use OIDC/JWT claims and server-side authorization policies.

## Priorities and next steps

The highest-value workflow was implemented first: submission, visibility, officer action, and exposure. With more time I would add authentication, document upload, additional-information requests, audit history, tests, event publishing, accessibility review, and deployment automation.
