# Best‑practice architecture for .NET Core projects

---

## Recommended Architecture

Clean Architecture, Hexagonal Architecture, and Onion Architecture are modern, domain‑driven approaches that promote separation of concerns, testability, and long‑term maintainability.

---

## Layers & Responsibilities

### Presentation Layer
- ASP.NET Core Web API / MVC / Razor Pages / Blazor
- Handles HTTP requests, validation, authentication
- Communicates only with the Application layer

### Application Layer
- Business use cases and application logic
- CQRS (Commands & Queries) using MediatR
- Defines interfaces for persistence and external services

### Domain Layer
- Entities, Value Objects, Aggregates, Domain Events
- Pure business logic with no framework dependencies

### Infrastructure Layer
- EF Core, repositories, external services
- Implements interfaces defined in the Application layer
- No business rules

---

## Folder Structure Example

```
src/
├── MyProject.Api
├── MyProject.Application
├── MyProject.Domain
├── MyProject.Infrastructure
└── MyProject.Tests
```

---

## Best Practices

- Dependency Inversion Principle
- CQRS + MediatR
- FluentValidation
- AutoMapper
- Structured logging (Serilog / NLog)
- EF Core Migrations
- Swagger / OpenAPI
- Unit & Integration testing

---

## Architecture Patterns

### Clean Architecture
- Inward dependencies
- Framework‑independent domain
- Highly testable

### Hexagonal Architecture
- Ports & Adapters
- Core isolated from infrastructure

### Onion Architecture
- Domain at the center
- Infrastructure on the outside

---

## When to Use

Best for medium to large applications, microservices, and long‑lived systems.

## Reference
Source: https://www.c-sharpcorner.com/article/best-practice-architecture-for-net-core-projects/
