using TenantX.Common.Domain.Results;

namespace TenantX.Common.Domain.Shared;

/// <summary>
/// Base class for all Value Objects. 
/// Provides value-based equality via 'record' and validation utilities.
/// </summary>
public abstract record ValueObject
{
    /// <summary>
    /// [The Guardian] 
    /// Internally validates a rule and throws an exception if broken.
    /// USE: Inside private constructors to ensure the object is never in an invalid state.
    /// OPTIONAL: Recommended for "Fail-Fast" safety.
    /// </summary>
    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleValidationException(rule);
        }
    }

    /// <summary>
    /// [The Communicator] 
    /// Validates a rule and returns a Result.
    /// USE: Inside static Factory Methods (Create) to return errors without throwing exceptions.
    /// REQUIRED: Essential for the Result Pattern flow.
    /// </summary>
    protected static Result<Success> Validate(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            return Error.Validation(rule.Code, rule.Message);
        }
        return Result.Success;
    }
}

/// <summary>
/// Defines a business requirement that must be met by a Value Object or Entity.
/// </summary>
public interface IBusinessRule
{
    /// <summary> Returns true if the business rule is violated. (REQUIRED) </summary>
    bool IsBroken();

    /// <summary> Human-readable explanation of the error. (REQUIRED) </summary>
    string Message { get; }

    /// <summary> Machine-readable unique error code (e.g., "Price.Negative"). (REQUIRED) </summary>
    string Code { get; }
}

/// <summary>
/// Exception thrown when a domain invariant is violated.
/// Used as a last line of defense.
/// </summary>
public class BusinessRuleValidationException : Exception
{
    public IBusinessRule BrokenRule { get; }

    public BusinessRuleValidationException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
    }

    public override string ToString() => $"{BrokenRule.Code}: {BrokenRule.Message}";
}