# 🏗️ Infrastructure Layer Foundation Guide

## 1. Data Access Strategy (Hybrid Approach)

The system uses a high-performance approach for PostgreSQL.

- **EF Core (Command Path):** Used for complex business logic and maintaining Aggregate integrity.
- **Dapper/Raw SQL (Query Path):** Uses `IDbConnectionFactory` to open high-speed connections for read-only operations, allowing for complex `JOINs` and optimized performance.
- **Npgsql Integration:** Built specifically for PostgreSQL, taking advantage of features like `jsonb` for Outbox/Inbox storage.

---

## 2. The "Invisible" Interceptors (Automation)

The Infrastructure layer uses EF Core Interceptors to enforce business rules automatically. **Developers do not need to set these fields manually.**

### A. Multi-Tenancy Enforcement (`ITenantEntity`)

- **How it works:** When saving an entity that implements `ITenantEntity`, the `AuditableEntityInterceptor` automatically injects the `TenantId` from the `ICurrentUserContext`.
- **Security:** This ensures that a user can **never** accidentally save data into another company's tenant.

### B. Automated Auditing (`IAuditableEntity`)

- **How it works:** The interceptor detects `Added` or `Modified` states.
- **Fields:** It populates `CreatedAt`, `CreatedBy`, `ModifiedAt`, and `ModifiedBy` using the `IDateTimeProvider` and `ICurrentUserContext`.
- **Value Object Support:** The `HasChangedOwnedEntities` extension ensures that if a Value Object (like `PersonName`) changes, the parent Aggregate's `ModifiedAt` timestamp is updated.

---

## 3. Reliability Patterns (Outbox & Inbox)

To ensure the system is "Production-Scale," we use the Outbox/Inbox pattern to guarantee **Eventual Consistency**.

### A. The Outbox Pattern (Internal Events)

- **Purpose:** Ensures that a Domain Event is never lost if the system crashes after a database save.
- **Implementation:** The `InsertOutboxMessagesInterceptor` scrapes domain events from entities and saves them into the `outbox_messages` table in the **same database transaction** as the business change.
- **Serialization:** Events are stored as `jsonb` for flexibility.

### B. The Inbox Pattern (External Events)

- **Purpose:** Ensures that incoming Integration Events are processed exactly once and provides a retry mechanism if a handler fails.

---

## 4. Background Processing & Messaging

- **Quartz.NET:** Used to trigger the Outbox/Inbox background workers. Each module has its own configuration to avoid conflicts.
- **MassTransit:** Acts as the abstraction for the `IEventBus`. In the current setup, it uses an `InMemory` transport, which is perfect for Modular Monolith communication.
- **Factory Pattern:** `DomainEventHandlersFactory` and `IntegrationEventHandlersFactory` use reflection and caching (ConcurrentDictionary) to dynamically locate and trigger the correct handlers in each module.

---

## 5. Global Security Context

### ICurrentUserContext

- This is the "Identity Bridge." It extracts the `UserId`, `TenantId`, and `WorkspaceId` from the `HttpContext` (JWT Claims).
- It is injected into the Interceptors to provide the "Who" for every database operation.

---

## 6. Infrastructure Developer Rules

1.  **Never Manually Set Audit Fields:** Do not set `CreatedAt` or `CreatedBy` in your handlers; let the interceptor do its job.
2.  **Schema Isolation:** When configuring EF Core for a new module, always use the `.ToTable("name", "schema")` pattern to keep the PostgreSQL database organized.
3.  **JSON Compatibility:** Ensure your Domain Events are serializable (use simple types or Value Objects that the `Newtonsoft.Json` serializer can handle).
4.  **Use IDbConnectionFactory for Reads:** For high-performance reports or dashboards, bypass EF Core and use the connection factory with Dapper.

---
