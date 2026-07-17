# 🧪 Unit Testing Foundation Guide

## 1. The `BaseTest` Abstraction

All unit tests for Domain Entities and Value Objects must inherit from `BaseTest`.

### A. Data Generation (Bogus)

- **Purpose:** We use the `Faker` instance to generate realistic test data (names, emails, dates).
- **Why:** This prevents "Magic Strings" in tests and ensures that our business rules are tested against a wide variety of inputs, not just one hardcoded example.
- **Usage:**
  ```csharp
  var name = Faker.Company.CompanyName();
  var email = Faker.Internet.Email();
  ```

### B. Domain Event Assertion

This is the most critical part of testing DDD aggregates. Since aggregates encapsulate logic, we often verify the _result_ of an action by checking if the correct **Domain Event** was raised.

- **Method:** `AssertDomainEventWasPublished<T>(entity)`
- **Purpose:** It searches the `DomainEvents` collection of an aggregate, verifies exactly one event of type `T` exists, and returns it for further inspection.
- **Rule:** Every state-changing method in our aggregates (like `UpgradeSubscription` or `Deactivate`) must have a test that uses this assertion.

---

## 2. Unit Testing Strategy

### A. Value Object Tests

Test that Value Objects correctly "Guard" their invariants.

- **Success Path:** Verify `Create` returns `IsSuccess`.
- **Failure Path:** Verify `Create` returns the correct `DomainError` when given invalid input (e.g., an empty string).

### B. Aggregate Root Tests

Test the business logic inside the entity.

- **Scenario:** A user tries to upgrade a subscription.
- **Logic:**
  1. Arrange: Create a Tenant.
  2. Act: Call `UpgradeSubscription`.
  3. Assert:
     - Check if the `Tier` property changed.
     - Use `AssertDomainEventWasPublished<TenantSubscriptionUpgradedEvent>` to verify the side effect.

---

## 3. Example of a "Gold Standard" Unit Test

Following your patterns, here is how a test for the `Tenant` aggregate would look:

```csharp
public class TenantTests : BaseTest
{
    [Fact]
    public void UpdateName_ShouldRaiseEvent_WhenNameIsChanged()
    {
        // Arrange
        var tenant = Tenant.Create(new TenantName("Old Name"), UserId.New()).Value;
        var newName = new TenantName("New Name");

        // Act
        var result = tenant.UpdateName(newName, UserId.New());

        // Assert
        result.IsSuccess.Should().BeTrue();
        tenant.Name.Should().Be(newName);

        // Use your helper to verify the event
        var domainEvent = AssertDomainEventWasPublished<TenantNameUpdatedEvent>(tenant);
        domainEvent.NewName.Should().Be(newName);
    }
}
```
