# AI Working Journal

## 2026-03-01 — Initial direction

- Asked AI to build the overall full-stack project from the assessment brief after creating the empty .NET 8 API.
- Accepted the recommendation to start with a vertical slice rather than attempting every requirement.
- Chose a modular monolith, REST, in-memory persistence, and a standalone Angular application to fit the time limit.
- Challenged the temptation to add Kafka, authentication, and a database before the core workflow was demonstrable.

## Decisions accepted

- Core MVP entities are claims with claimant details, incident data, estimated loss, status, assignment, and decision reason.
- REST is appropriate for synchronous UI commands and queries. Kafka is documented as a production extension for domain events.
- A single Angular app uses a role switcher for the demo so both personas share the same application shell.
- Seeded claims make the dashboard meaningful immediately after startup.

## Decisions overridden or constrained

- Full identity was not implemented because the assessment timebox does not justify an identity server. The UI role switch is explicitly documented as non-production.
- Database persistence was not added. The service uses an in-memory collection and documents the migration path.
- Angular build verification could not be performed because Node.js/npm were not installed in the environment. The frontend source and package metadata were still added for local installation.

## Sign-off checklist

Before submission, manually review every generated file, install Node.js, run `npm install && npm run build`, run `dotnet build`, test the Swagger endpoints, and create meaningful Git commits showing the progression.
