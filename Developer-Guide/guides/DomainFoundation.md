# 📘 Domain Foundation & DDD Guide

## 1. The ID System (Strongly-Typed IDs)

Instead of using raw `Guid` values, we use **Strongly-Typed IDs**. This prevents "primitive obsession" and accidental ID swapping (e.g., passing a `UserId` into a method expecting a `TenantId`).

- **Structure:** All IDs inherit from `EntityId<T>`.
- **Purpose:** Ensures type safety at compile time.
- **When to use:** Every entity must have a corresponding ID record (e.g., `TenantId`, `WorkspaceId`).
- **Usage:**
  ```csharp
  var id = TenantId.New(); // Create new
  var existingId = TenantId.From(someGuid); // From existing
  ```

---

## 2. Entity & Aggregate Architecture

We categorize objects based on their scope and lifecycle.

### A. BaseEntity<TId>

The foundation of every object with an identity. It handles equality logic based on the ID.

### B. AggregateRoot<TId> (Global Scope)

Used for top-level objects that are **not** restricted by a tenant (Global entities).

- **Purpose:** Manages domain events and auditing.
- **When to use:** Use for `Tenant`, `User`, and `ApiClient`.

### C. TenantScopedAggregateRoot<TId> (Multi-Tenancy)

The heart of our data isolation strategy.

- **Purpose:** Automatically enforces the `TenantId` requirement.
- **When to use:** Use for **everything** inside a shop (Workspaces, Products, Accounts, Sales).
- **Note:** The `TenantId` is usually populated by the Infrastructure layer during the save process.

### D. AuditableEntity

- **Purpose:** Automatically tracks _who_ created/modified a record and _when_.
- **Requirement:** All aggregates should be auditable to maintain a financial and operational audit trail.

---

## 3. Value Objects (The Building Blocks)

Value Objects represent concepts without an identity (e.g., `Email`, `Currency`, `TenantName`).

### The Two Pillars of Validation:

1.  **The Guardian (CheckRule):** Used inside **private constructors**. It throws a `BusinessRuleValidationException` if a rule is broken. This is our "Last Line of Defense" to ensure an object is never invalid in memory.
2.  **The Communicator (Validate):** Used inside **public static Factory Methods** (`Create`). It returns a `Result<T>` instead of throwing an exception.

**When to use:** Use for any property that has business logic or validation rules. Do not use raw strings for names, emails, or descriptions.

---

## 4. The Result Pattern (Railway Programming)

We do not use `try-catch` for business logic flow. We use the `Result<T>` pattern to handle success and failure gracefully.

- **Result.Success:** Returns the expected value.
- **Result.Failure:** Returns an `Error` object containing a `Code`, `Description`, and `ErrorKind` (Validation, Conflict, NotFound, etc.).
- **Purpose:** Makes business flow explicit and prevents "Exception Pollution."

---

## 5. Business Rules (IBusinessRule)

Business rules are encapsulated into small, reusable classes.

- **IsBroken():** The logic that determines if the rule failed.
- **Message & Code:** The user-friendly and system-friendly error details.
- **When to use:** Every time you have a constraint (e.g., "Email must be valid format," "Price cannot be negative").

---

## 6. Security & Multi-Tenant Context (ICurrentUserContext)

This interface is the bridge between the logged-in user and the Domain layer.

- **UserId:** Who is doing the action.
- **TenantId:** Which company they belong to.
- **WorkspaceId:** Which specific branch they are currently working in.
- **When to use:** Use this in Application Services and Infrastructure to filter data and populate audit fields.

---

## 7. Constants and Centralized Errors

To keep the system maintainable, we never hardcode strings or error messages.

- **ValidationConstants:** Centralized lengths and limits (e.g., `Names.MaxLength`).
- **DomainErrors:** A hierarchical static class containing all error codes.
  - _Example:_ `DomainErrors.Currency.Unsupported`.
  - **Rule:** When creating a new Module, always add its errors here.

---

## 8. Summary Checklist for New Features:

1.  **Define the ID:** Create a record inheriting from `EntityId`.
2.  **Choose the Base:** Is it `AggregateRoot` (Global) or `TenantScopedAggregateRoot` (Tenant-specific)?
3.  **Encapsulate Properties:** Use `ValueObjects` for all properties.
4.  **Create via Factory:** Only allow object creation via `public static Result<T> Create(...)`.
5.  **Use Private Setters:** Never allow external code to change the state of an entity without a descriptive method.

---

**This architecture is designed to protect the system's data integrity. By following these rules, we ensure that the system remains stable, auditable, and ready for scaling to thousands of tenants.**
