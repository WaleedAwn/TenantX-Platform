# 📗 Application Layer Foundation Guide

## 1. The MediatR Pipeline (Cross-Cutting Concerns)

We use **IPipelineBehavior** to ensure that every request (Command or Query) is processed through a consistent set of "filters" before reaching the handler.

### A. LoggingBehavior

- **Purpose:** Automatically logs the start and completion of every request using high-performance `LoggerMessage` source generators.
- **Rule:** Every request is tracked. Do not add manual "Request Started" logs in handlers.

### B. ValidationBehavior

- **Purpose:** The "Gatekeeper." It uses **FluentValidation** to check the request before it hits the handler.
- **Integration with Result Pattern:** If validation fails, it converts FluentValidation errors into our `Error.Validation` type and returns them through the `Result<T>` pattern.
- **Usage:** Define a class inheriting from `AbstractValidator<TRequest>` for your commands.

### C. UnhandledExceptionBehavior

- **Purpose:** The safety net. It catches any unexpected code crashes, logs the full stack trace, and wraps it in a **`BaqalatiException`**.
- **Rule:** This prevents raw database or system errors from leaking to the API client.

---

## 2. Exception Handling (BaqalatiException)

- **Purpose:** A custom, centralized exception type for the entire ecosystem.
- **When to use:** Use this for truly exceptional system failures. For business logic errors (e.g., "Insufficient Funds"), **always** use the `Result` pattern instead.

---

## 3. Event-Driven Architecture (The Messaging DNA)

We distinguish between events that stay "inside" a module and events that go "outside."

### A. Domain Events (`IDomainEvent`)

- **Scope:** Internal to the module.
- **Logic:** Used for side effects within the same business boundary (e.g., "Tenant Created" $\rightarrow$ "Assign Default Owner Role").
- **Handler:** Use `DomainEventHandler<TDomainEvent>`.

### B. Integration Events (`IIntegrationEvent`)

- **Scope:** External (Cross-module or Cross-system).
- **Logic:** Published via the **`IEventBus`**. Used for communication between modules (e.g., "SaaS Module created a Tenant" $\rightarrow$ "Accounting Module creates a default Ledger").
- **Handler:** Use `IntegrationEventHandler<TIntegrationEvent>`.

---

## 4. Infrastructure Abstractions

We never use `DateTime.Now` directly in the application layer to ensure the system is testable.

### IDateTimeProvider

- **Purpose:** Provides a mockable `UtcNow`.
- **Rule:** Always inject `IDateTimeProvider` when you need the current time. This is critical for calculating subscription expiry and transaction timestamps accurately during tests.

---

## 5. Developer Rules for the Application Layer

1.  **Immutability:** All Commands and Queries must be `public record` to ensure they cannot be modified once they enter the pipeline.
2.  **No Logic in Handlers:** Application Handlers should only:
    - Fetch an Aggregate from a Repository.
    - Call a method on the Aggregate.
    - Save the Aggregate.
    - Return a `Result`.
3.  **Validation First:** Every Command must have a corresponding Validator. If a field is required, it must be caught by the `ValidationBehavior` before reaching the Domain.
4.  **Semantic Returns:** Handlers should return semantic results:
    - `Result<Created>` for creations.
    - `Result<Updated>` for modifications.
    - `Result<TValue>` for queries.

---

-
