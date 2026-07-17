This is the updated and professionally organized **System Development Manifesto (v3.0)**. It serves as the single source of truth for all developers working on the Supermarket SaaS Ecosystem.

---

# 📜 System Development Manifesto (v3.0)

## 1. Architectural Philosophy

We follow **Clean Architecture** and **Domain-Driven Design (DDD)** within a **Modular Monolith** structure.

- **The Domain is King:** The Domain layer contains business logic and invariants. It has **zero** dependencies.
- **Railway-Oriented Programming:** We do not use exceptions for business flow. We use the **Result Pattern**.
- **Fail-Fast:** Invariants are checked at the moment of object creation via the "Guardian" pattern.
- **Semantic Success:** We use semantic types (`Created`, `Updated`, `Deleted`) to signal the nature of successful operations.

---

## 2. The "Common" Foundation (Shared DNA)

Every module must reference the `Common` ecosystem. Below are the key components and implementation references:

### A. [The Domain Layer](guides/DomainFoundation.md)

- **Identity:** All entities use [Strongly-typed IDs](../src/common/Modules.Common.Domain/Shared/EntityId.cs) to prevent ID swapping.
- **Base Types:**
  - [AggregateRoot<TId>](../src/common/Modules.Common.Domain/Shared/AggregateRoot.cs): For global entities (Tenant, User).
  - [TenantScopedAggregateRoot<TId>](../src/common/Modules.Common.Domain/Shared/TenantScopedAggregateRoot.cs): For shop-specific data.
- **Validation:** Every property with logic must be a [ValueObject](../src/common/Modules.Common.Domain/Shared/ValueObject.cs) using [IBusinessRule](../src/common/Modules.Common.Domain/Shared/IBusinessRule.cs).
- **Error Handling:** All error codes must be registered in the centralized [DomainErrors](../src/common/Modules.Common.Domain/Constants/DomainErrors.cs).

### B. [The Application Layer](guides/ApplicationFoundation.md)

- **Orchestration:** We use **MediatR** for all Commands and Queries.
- **The Pipeline:** Every request automatically passes through:
  1.  [LoggingBehavior](../src/common/Modules.Common.Application/Behaviors/LoggingBehavior.cs): Structured logging.
  2.  [ValidationBehavior](../src/common/Modules.Common.Application/Behaviors/ValidationBehavior.cs): Gatekeeping via FluentValidation.
  3.  [UnhandledExceptionBehavior](../src/common/Modules.Common.Application/Behaviors/UnhandledExceptionBehavior.cs): Safety net.
- **Time & Security:** Always use [IDateTimeProvider](../src/common/Modules.Common.Application/Clock/IDateTimeProvider.cs) and [ICurrentUserContext](../src/common/Modules.Common.Domain/Shared/ICurrentUserContext.cs).

### C. [The Infrastructure Layer](guides/InfrastructureFoundation.md)

- **Hybrid Data:** Use **EF Core** for Writes and [IDbConnectionFactory](../src/common/Modules.Common.Infrastructure/Data/IDbConnectionFactory.cs) (Dapper) for Reads.
- **Interceptors (Automation):**
  - [AuditableEntityInterceptor](../src/common/Modules.Common.Infrastructure/Interceptors//AuditableEntityInterceptor.cs): Auto-sets `CreatedBy`, `ModifiedAt`, and `TenantId`.
  - [InsertOutboxMessagesInterceptor](../src/common/Modules.Common.Infrastructure/Outbox/InsertOutboxMessagesInterceptor.cs): Auto-saves Domain Events to the Outbox.
- **Reliability:** Implements [Outbox](../src/common/Modules.Common.Infrastructure/Outbox/) and [Inbox](../src/common/Modules.Common.Infrastructure/Inbox/) patterns for eventual consistency.

### D. [The API Layer](guides/ApiFoundation.md)

- **Decentralized Routes:** Modules implement [IApiEndpoint](../src/common/Modules.Common.Api/Abstractions/IApiEndpoint.cs).
- **Result Bridge:** Use the [ToProblem](../src/common/Modules.Common.Api/Extensions/ProblemExtensions.cs) extension to convert `Result.Errors` into RFC 7807 Problem Details.

---

## 3. Developer Implementation Rules

### Rule 1: Aggregate Construction

Aggregates **must** have a private constructor and a `public static Result<T> Create(...)` factory.

```csharp
// Use the 'Guardian' in the private constructor
private Tenant(TenantId id, TenantName name) : base(id) {
    CheckRule(new NameRequiredRule(name));
}
```

### Rule 2: Multi-Tenancy Isolation

If an entity belongs to a shop, it **must** inherit from `TenantScopedAggregateRoot`. Never manually set the `TenantId`; let the `AuditableEntityInterceptor` handle it from the security context.

### Rule 3: Commands vs. Queries

- **Commands:** Use MediatR `IRequest<Result<T>>`. Persist changes via Repositories.
- **Queries:** Use Dapper for high-performance SQL. Return DTOs, not Entities.

### Rule 4: Messaging

- **Internal:** Raise `DomainEvent` for logic inside the same module.
- **External:** Use `IIntegrationEvent` via `IEventBus` for cross-module communication.

---

## 4. [Testing Standards](guides/TestingFoundation.md)

- **Unit Tests:** Inherit from [BaseTest](../src/common/Modules.Common.UnitTests/Abstractions/BaseTest.cs).
- **Data:** Use `Faker` (Bogus) for all test data generation.
- **Assertions:** Always use `AssertDomainEventWasPublished<T>` to verify side effects in aggregates.

---

## 5. [Web Host Configuration (Baqalati.Api)](guides/BaqalatiWebHostGuide.md)

When adding a new module:

1.  Add `modules.[name].json` configuration.
2.  Register via `builder.Services.Add[Name]Module()`.
3.  Add the DB context to `ApplyMigrations` in [MigrationsExtensions](../src/API/Baqalati.Api/Extensions/MigrationsExtensions.cs).
4.  Update [AddAppOpenTelemetry](../API/Baqalati.Api/DependencyInjection.cs) with the module's activity source.

---

## 🤖 AI Instruction (Copy-Paste for AI Coding)

> "This system is a Modular Monolith using DDD and Clean Architecture.
> **Tech Stack:** ASP.NET Core, EF Core (Write), Dapper (Read), MediatR, Result Pattern with Semantic Success (Created, Updated), Quartz (Outbox), Bogus (Testing).
>
> **Core Constraints:**
>
> 1. Use **Strongly-typed IDs** (EntityId<T>).
> 2. Aggregates use private setters, private constructors with **CheckRule**, and **public static Result<T> Create** factories.
> 3. Implement **TenantScopedAggregateRoot** for multi-tenant isolation.
> 4. Use **ICurrentUserContext** for security and **IDateTimeProvider** for clock.
> 5. Endpoints must implement **IApiEndpoint** and use **.ToProblem()** for error mapping.
> 6. Use **Primary Constructors**, **File-scoped namespaces**, and **explicit curly braces**.
> 7. All state changes must return **Result<Updated>** or **Result<Created>**."

---
