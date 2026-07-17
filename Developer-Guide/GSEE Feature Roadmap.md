# 🗺️ GSEE Feature Roadmap: The Source of Truth

## 1. Actors Definition

- **Tenant Owner (Admin):** The business owner of the client system (e.g., FitnessApp Admin). They configure the "Menu" and manage the business.
- **End User (Subscriber):** The customer using the client system. They interact with the "Lifecycle" (buying, paying, using features).
- **The System (Background):** Automated jobs that manage time-based rules.

---

## 2. Feature Aggregate (The Registry)

_Goal: Define the metrics that can be limited._

### 🛠️ Tenant Owner Operations

| Type        | Name                    | Pattern | Description                                             |
| :---------- | :---------------------- | :------ | :------------------------------------------------------ |
| **Command** | `CreateFeature`         | EF Core | Registers a new key (e.g., `max_tasks`) and its type.   |
| **Command** | `UpdateFeatureMetadata` | EF Core | Updates the Display Name or Description.                |
| **Command** | `ArchiveFeature`        | EF Core | Retires a feature so it cannot be used in new plans.    |
| **Query**   | `GetFeaturesList`       | Dapper  | Returns all active/archived features for the dashboard. |

---

## 3. Plan Aggregate (The Menu)

_Goal: Define commercial packages and prices._

### 🛠️ Tenant Owner Operations

| Type        | Name                  | Pattern | Description                                         |
| :---------- | :-------------------- | :------ | :-------------------------------------------------- |
| **Command** | `CreatePlan`          | EF Core | Sets name, price (Money VO), and billing period.    |
| **Command** | `SetPlanEntitlements` | EF Core | Links features to the plan with specific values.    |
| **Command** | `ArchivePlan`         | EF Core | Retires the plan from the public menu.              |
| **Query**   | `GetPlanAdminDetails` | Dapper  | Detailed view of a plan including its entitlements. |

### 👤 End User Operations

| Type      | Name                | Pattern | Description                                         |
| :-------- | :------------------ | :------ | :-------------------------------------------------- |
| **Query** | `GetAvailablePlans` | Dapper  | List of non-archived plans for the "Pricing Table." |

---

## 4. Subscriber Aggregate (The Identity)

_Goal: Map external users to GSEE._

### 🛠️ Tenant Owner Operations

| Type      | Name                   | Pattern | Description                                         |
| :-------- | :--------------------- | :------ | :-------------------------------------------------- |
| **Query** | `SearchSubscribers`    | Dapper  | Find a subscriber by their External ID or GSEE ID.  |
| **Query** | `GetSubscriberHistory` | Dapper  | Full timeline of every plan this user has ever had. |

### 👤 End User Operations

| Type        | Name             | Pattern | Description                                                |
| :---------- | :--------------- | :------ | :--------------------------------------------------------- |
| **Command** | `SyncSubscriber` | EF Core | Ensures a GSEE record exists when a user joins the Tenant. |

---

## 5. Subscription Aggregate (The Contract)

_Goal: Manage the active lifecycle, payments, and access._

### 👤 End User Operations (Primary Lifecycle)

| Type        | Name                      | Pattern | Description                                             |
| :---------- | :------------------------ | :------ | :------------------------------------------------------ |
| **Command** | `InitiateSubscription`    | EF Core | Generates a `PaymentCode` and `Pending` record.         |
| **Command** | `ConfirmPayment`          | EF Core | **The Atomic Switch:** Activates new, deactivates old.  |
| **Command** | `CancelRenewal`           | EF Core | Flips status to `Canceled` (Access stays until end).    |
| **Command** | `ReactivateRenewal`       | EF Core | Flips `Canceled` back to `Active` before it expires.    |
| **Query**   | `GetActiveEntitlements`   | Dapper  | **The Most Used Query:** Returns user's current limits. |
| **Query**   | `GetMySubscriptionStatus` | Dapper  | Shows user if they are Active, Pending, or Expiring.    |

### 🛠️ Tenant Owner Operations

| Type        | Name                    | Pattern | Description                                           |
| :---------- | :---------------------- | :------ | :---------------------------------------------------- |
| **Command** | `ManualDeactivate`      | EF Core | Forcefully kills a subscription (e.g., for fraud).    |
| **Query**   | `GetExpiringSoonReport` | Dapper  | List of users whose access ends in the next 48 hours. |

---

## 6. System & Background Services (Automation)

_Goal: Enforce rules of time and cleanup._

### 🕒 Quartz.NET Background Jobs

| Job Name                   | Frequency     | Responsibility                                                                                                        |
| :------------------------- | :------------ | :-------------------------------------------------------------------------------------------------------------------- |
| **SubscriptionJanitorJob** | Every 1 Hour  | 1. Finds `Active/Canceled` where `EndDate + GracePeriod < Now`. <br> 2. Calls `Expire()`. <br> 3. Dispatches Webhook. |
| **PaymentCodeCleanupJob**  | Every 6 Hours | 1. Finds `Pending` where `PaymentCodeExpiresAt < Now`. <br> 2. Calls `FailPayment()`.                                 |

---

## 7. The Core Business Orchestration (Phase 5)

### **The Atomic Switch Logic (Inside `SubscriptionService`)**

This is the specialized logic that executes during the `ConfirmPayment` command. It is the only place allowed to touch two subscriptions at once.

1.  **Identity Check:** Ensure the New and Old subscriptions belong to the same Subscriber.
2.  **State Handover:**
    - Deactivate the **Old Aggregate**.
    - Activate the **New Aggregate**.
3.  **Grace Period Injection:** Pull the `GracePeriod` from Tenant Settings and inject it into the `SubscriptionPeriod` Value Object of the new record.
