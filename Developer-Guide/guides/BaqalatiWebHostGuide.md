# 🚀 TenantX.Api Web Host Guide

## 1. Module Registration Flow

To keep the system modular, the host does not "know" the details of each module. When adding a new module (e.g., **SaaS**), follow these three steps in `Program.cs`:

1.  **Configuration:** Load the module's JSON settings.
    ```csharp
    builder.Configuration.AddModuleConfiguration(["users", "saas"]);
    ```
2.  **Services:** Register the module's internal dependencies.
    ```csharp
    builder.Services.AddUsersModule(builder.Configuration);
    builder.Services.AddSaaSModule(builder.Configuration); // Example
    ```
3.  **Endpoints:** (Automatic) The host calls `app.MapApiEndpoints()`, which scans the module assemblies for `IApiEndpoint` implementations.

---

## 2. Configuration Management

Instead of one giant `appsettings.json`, we use **Per-Module Configuration**.

- **Location:** Create files named `modules.[module_name].json` in the root of the Api project.
- **Environment Specifics:** Use `modules.[module_name].Development.json` for local overrides.
- **Mechanism:** `ConfigurationExtensions` loads these files automatically, ensuring settings for "Inventory" don't clutter settings for "Users."

---

## 3. The Middleware Pipeline (Order Matters)

The pipeline is configured in `DependencyInjection.UseCoreMiddlewares`. The order is critical for security and performance:

1.  **GlobalExceptionHandler:** Catches every crash in the system and returns a standard `ProblemDetails` JSON.
2.  **RateLimiter:** Protects the system from Brute Force/DDoS _before_ wasting CPU on authentication.
3.  **Authentication:** Identifies _who_ the user is.
4.  **Authorization:** Decides _if_ the user can access the route.
5.  **OutputCache:** Caches the final response (configured to cache for 60 seconds by default).

---

## 4. API Versioning & Documentation

The system uses **URL Segment Versioning** (e.g., `https://api.baqalati.com/v1/users`).

- **OpenAPI (Swagger):** Configured via `AddApiDocumentation`.
- **Security Transformers:** Automatically adds the "Authorize" button to Swagger. It detects the `[Authorize]` attribute on your endpoints and requires a JWT Bearer token.
- **VersionInfoTransformer:** Automatically updates the Swagger UI header based on the version (v1, v2).

---

## 5. Resilience & Performance

### A. Rate Limiting

- **Policy:** `SlidingWindow` (100 requests per minute).
- **Rejection:** Returns `429 Too Many Requests`.

### B. Output Caching

- **Global Policy:** 60-second expiration.
- **Resource Limit:** 100MB maximum cache size to prevent memory exhaustion.

### C. Global Exception Handling

- **File:** `GlobalExceptionHandler.cs`.
- **Behavior:** Converts raw C# Exceptions into **RFC 7807 Problem Details**.
- **Developer Tip:** In Development mode, `app.UseDeveloperExceptionPage()` is used for rich debugging, but in Production, the `GlobalExceptionHandler` hides sensitive stack traces.

---

## 6. Observability (OpenTelemetry & Serilog)

The system is built for cloud monitoring.

- **Logging:** Serilog is configured to read from `appsettings.json`. It captures structured logs, making it easy to search by `UserId` or `TenantId`.
- **Tracing:** OpenTelemetry tracks requests across modules. It instruments:
  - **AspNetCore:** Incoming HTTP requests.
  - **Npgsql:** Database query performance.
  - **HttpClient:** Outgoing calls.
- **Metrics:** Provides a `/metrics` endpoint for Prometheus to track runtime health (CPU, Memory, Request Count).

---

## 7. Developer Cheat-Sheet: Adding a New Module

| Action                | File to Modify            | Code to Add                                                   |
| :-------------------- | :------------------------ | :------------------------------------------------------------ |
| **Load Config**       | `Program.cs`              | `builder.Configuration.AddModuleConfiguration(["newmodule"])` |
| **Register Services** | `Program.cs`              | `builder.Services.AddNewModule(builder.Configuration)`        |
| **Add DB Migration**  | `MigrationsExtensions.cs` | `ApplyMigrations<NewModuleDbContext>(scope);`                 |
| **Add Telemetry**     | `Program.cs`              | Add module name to `AddCoreWebApiInfrastructure("newmodule")` |
