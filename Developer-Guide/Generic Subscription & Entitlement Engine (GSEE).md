# Generic Subscription & Entitlement Engine (GSEE)

**Status:** Final Architectural Specification  
**Role:** Standalone Microservice / SaaS-Ready Platform  
**Version:** 1.2 (Tenant-Centric Multi-Tenancy)

---

## 1. Executive Summary
The **GSEE** is a project-agnostic service designed to manage the commercial lifecycle of users across multiple independent systems, referred to as **Tenants**. By decoupling "Access Logic" (what a user paid for) from "Business Logic" (what the app does), it allows for rapid scaling and a single source of truth for billing and entitlements across any number of external applications.

---

## 2. Core Pillars (The Architecture)

1.  **SaaS Multi-Tenancy (Tenant Isolation):** The system is built to host multiple **Tenants** (e.g., a Fitness App, a Task App, and a Video App) on a single infrastructure. Every piece of data is "Tenant-Aware."
2.  **Identity Decoupling:** GSEE stores no sensitive personal data (passwords/emails). It maps an `External_User_ID` from the Tenant's system to a local `Subscriber` record.
3.  **The Registry Pattern (Entitlements):** Features are not hardcoded columns. They are dynamic "Keys" registered by each Tenant, allowing the engine to support any product model without code changes.
4.  **Payment Agnosticism:** GSEE manages subscription **states** and **activation logic** but delegates the actual credit card processing to the Tenant's system.
5.  **API-First / Standalone:** GSEE lives as a separate REST API, allowing it to be integrated into any technology stack (Node.js, Python, PHP, etc.).

---

## 3. Tenant Management & Isolation

To make GSEE a true SaaS, we distinguish between the **Provider** (you) and the **Tenants** (the systems using your service).

### A. The Tenant Entity (`sub_projects`)
Each system using GSEE is a **Tenant**. They are managed via the `sub_projects` table:
*   **Tenant ID (`ProjectId`):** A unique UUID assigned to each system.
*   **API Key:** A secure secret used by the Tenant to authenticate their API calls.
*   **Webhook URL:** The endpoint where GSEE sends notifications (e.g., "Subscription Expired") back to the Tenant.
*   **Status:** `ACTIVE` or `SUSPENDED` (allowing the Provider to shut down a Tenant's access).

### B. The Aggregate Root Pattern
To ensure absolute data isolation, all core entities (Tiers, Features, Subscriptions) inherit from a base **Aggregate Root** containing a `ProjectId`.
*   **Safety Rule:** Every database query **must** include a filter: `WHERE project_id = {authenticated_tenant_id}`.
*   **Security:** This ensures that Tenant A (Fitness App) can never accidentally or maliciously access the tiers or users of Tenant B (Task App).

### C. The Tenant Handshake (API Authentication)
Tenants interact with GSEE by providing their credentials in every request header:
1.  **Middleware** validates the `X-GSEE-API-Key`.
2.  **Middleware** resolves the `ProjectId`.
3.  **Context** is set for the duration of the request, ensuring all database operations are scoped to that Tenant.

---

## 4. Data Model (Entities)

| Entity | Level | Description |
| :--- | :--- | :--- |
| **Project (Tenant)** | Global | The system using GSEE. Stores API Keys and configuration. |
| **Feature** | Tenant | Dictionary of keys defined by the Tenant (e.g., `max_workspaces`). |
| **Tier (Plan)** | Tenant | The Tenant's commercial packages (e.g., "Gold Plan"). |
| **Tier_Feature** | Tenant | The specific value (limit) for a feature within a tier. |
| **Subscriber** | Tenant | A local record representing a user belonging to a specific Tenant. |
| **Subscription** | Tenant | The contract status (Pending, Active, Expired, Deactivated). |
| **Transaction** | Tenant | The audit log linking a payment intent to a gateway reference. |

---

## 5. Detailed Operational Scenarios

### Scenario A: Tenant Onboarding (Setup)
*Target: A new system, "FitnessPro," wants to use GSEE.*
1.  **Registration:** The Provider creates a record for "FitnessPro" in `sub_projects`.
2.  **API Key:** GSEE generates a unique key. FitnessPro saves this in their environment variables.
3.  **Feature Registry:** FitnessPro calls GSEE to register their metrics:
    *   `max_workouts` (Numeric)
    *   `pro_videos` (Boolean)
4.  **Tier Definition:** FitnessPro defines their "Elite Plan" at $29/mo with `max_workouts=999` and `pro_videos=true`.

### Scenario B: Payment & Atomic Upgrade
*Target: A user in FitnessPro upgrades from "Free" to "Elite".*
1.  **Intent:** User selects "Elite." FitnessPro calls GSEE to create a `PENDING` Subscription.
2.  **Payment:** FitnessPro charges the user's card via Stripe. Stripe returns `txn_abc123`.
3.  **Confirmation:** FitnessPro calls `confirmPayment(transaction_id, ref: "txn_abc123")`.
4.  **Atomic Switch:** Inside a single database transaction, GSEE:
    *   Locates any currently `ACTIVE` subscription for this user.
    *   Marks the old subscription as `DEACTIVATED`.
    *   Marks the "Elite" subscription as `ACTIVE`.
    *   Sets `EndsAt` to current time + interval.

### Scenario C: Entitlement Enforcement
*Target: User tries to watch a Pro Video in FitnessPro.*
1.  **The Question:** FitnessPro calls GSEE: `GET /entitlements/{userId}/pro_videos`.
2.  **The Logic:** GSEE finds the user's active "Elite" plan and looks up the value for `pro_videos`.
3.  **The Answer:** GSEE returns `true`.
4.  **Action:** FitnessPro allows the video to play.

---

## 6. System Operations (API Requirements)

### **Administrative Ops (Tenant Configuration)**
*   `POST /features`: Tenant registers a new limit key.
*   `POST /tiers`: Tenant defines a new plan.
*   `POST /tiers/{id}/features`: Tenant sets the values for that plan.

### **Subscriber & Payment Ops**
*   `POST /subscribers/sync`: Link a Tenant's user ID to GSEE.
*   `POST /subscriptions/subscribe`: Create a `PENDING` plan.
*   `POST /payments/confirm`: The critical trigger that activates a plan and kills old ones.
*   `GET /subscribers/{id}/entitlements`: Returns a JSON map of all limits for the current user.

---

## 7. Communication Strategy

### A. API (Pull)
Tenants call GSEE endpoints to perform actions or check statuses. To solve latency, Tenants should cache `/entitlements` results in their own Redis/Session storage.

### B. Webhooks (Push)
GSEE acts as the master clock. When a subscription naturally reaches its `EndsAt` date, GSEE pings the Tenant's `webhook_url`:
*   **Event `subscription.expired`:** Tells the Tenant to revoke user access immediately.
*   **Event `subscription.upgraded`:** Confirms the atomic switch was successful.

---

## 8. Technical Best Practices

1.  **Tenant Isolation:** No entity should ever be created without a `ProjectId`.
2.  **Immutability:** Tiers are immutable. If a Tenant wants to change a price, they must create a new Tier. This protects historical billing data.
3.  **Atomic Swapping:** The upgrade/downgrade logic must be wrapped in a database transaction. If the deactivation of the old plan fails, the activation of the new plan must also fail.
4.  **UTC Only:** All timestamps are stored and calculated in UTC to ensure consistency across Tenants in different time zones.
5.  **Idempotent Activation:** If a Tenant calls `confirmPayment` twice with the same transaction ID, GSEE must return the same success result without creating duplicate active subscriptions.

---

## 9. Future Expansion
The architecture is designed to support:
*   **Grace Periods:** Allowing users 3 days of access after a missed payment.
*   **Metered Billing:** Tracking usage counts (e.g., "10 out of 50 tasks used").
*   **Tenant Dashboards:** A UI where Tenants can log in and manage their own Tiers and Features.

---
**End of Source of Truth Document.**


