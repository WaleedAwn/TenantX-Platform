# 🌐 API Layer Foundation Guide

## 1. Decentralized Routing (`IApiEndpoint`)

We avoid the "Giant Program.cs" problem. Each feature or module defines its own routes by implementing `IApiEndpoint`.

- **Purpose:** Allows modules to be truly independent. A developer adding a new module doesn't need to modify the main API project; they simply implement this interface in their module.
- **Usage:**
  ```csharp
  public class CreateTenantEndpoint : IApiEndpoint
  {
      public void MapEndpoint(WebApplication app)
      {
          app.MapPost("api/tenants", async (CreateTenantRequest request, ISender sender) =>
          {
              // Logic here
          }).WithTags("Tenants");
      }
  }
  ```

---

## 2. Automatic Discovery (Reflection-Based Registration)

The system uses two powerful extension methods to wire up the API automatically:

- **`AddEndpoints(params Assembly[])`**: Scans the provided assemblies for any classes that implement `IApiEndpoint` and registers them in the Dependency Injection (DI) container.
- **`MapApiEndpoints()`**: Retrieves all registered endpoints from DI and executes their `MapEndpoint` logic.
- **Rule:** When creating a new module, ensure its assembly is passed to `AddEndpoints` in the main `Program.cs`.

---

## 3. Standardized Error Handling (`ToProblem`)

This is the bridge between our **Domain Result Pattern** and the **HTTP Protocol**. It follows the **RFC 7807 (Problem Details for HTTP APIs)** standard.

### Automatic Status Code Mapping:

- **`ErrorKind.Validation`** $\rightarrow$ `400 Bad Request`.
- **`ErrorKind.NotFound`** $\rightarrow$ `404 Not Found`.
- **`ErrorKind.Conflict`** $\rightarrow$ `409 Conflict`.
- **`ErrorKind.Unauthorized`** $\rightarrow$ `403 Forbidden`.
- **Others** $\rightarrow$ `500 Internal Server Error`.

### Validation Problem Details:

If the `Result` contains validation errors, the system generates a `ValidationProblemDetails` object. This is essential for frontend developers (React/Flutter) because it provides a structured dictionary of errors mapped to specific fields, allowing for real-time form validation UI.

---

## 4. API Developer Rules

1.  **Tag Your Endpoints:** Always use `.WithTags("ModuleName")` to keep the Swagger documentation organized.
2.  **Use the Result Bridge:** Never manually return `Results.BadRequest()` or `Results.NotFound()`. Always call `.ToProblem()` on your `Result.Errors` to ensure consistent error responses across the entire system.
3.  **Minimal APIs:** We use **Minimal APIs** for performance and simplicity. Do not use Controller-based APIs unless there is a legacy requirement.
4.  **Keep Endpoints Lean:** The endpoint should only handle:
    - Extracting the request.
    - Sending a command/query via MediatR.
    - Returning the Result via `ToProblem`.
    - _No business logic allowed in the Endpoint class._

---

## 5. Summary of the Full Stack Flow

Now that I have all your layers, here is how a single request flows through the system:

1.  **API Layer:** `IApiEndpoint` receives the request.
2.  **Application Pipeline:** `LoggingBehavior` $\rightarrow$ `ValidationBehavior` $\rightarrow$ `UnhandledExceptionBehavior`.
3.  **Application Handler:** Fetches aggregate from DB using `EF Core`.
4.  **Domain Layer:** Aggregate executes business logic and raises `DomainEvents`.
5.  **Infrastructure Layer:** `AuditableEntityInterceptor` sets timestamps; `InsertOutboxMessagesInterceptor` saves events to the Outbox.
6.  **Database:** Transaction completes in PostgreSQL.
7.  **Result Return:** `Result<T>` flows back to the API, which calls `.ToProblem()` if there are errors, or returns `200/201` if successful.

---
