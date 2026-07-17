namespace TenantX.Common.Domain;

public interface IDomainEvent
{
	Guid Id { get; }
	DateTime OccurredOnUtc { get; }
}

/// <summary>
/// Base record for all domain events. 
/// Using 'record' ensures immutability and value-based equality.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    // Default constructor for new events
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOnUtc = DateTime.UtcNow;
    }

    // Constructor for rehydrating events (e.g., from the Outbox)
    protected DomainEvent(Guid id, DateTime occurredOnUtc)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
    }

    public Guid Id { get; init; }
    public DateTime OccurredOnUtc { get; init; }
}
