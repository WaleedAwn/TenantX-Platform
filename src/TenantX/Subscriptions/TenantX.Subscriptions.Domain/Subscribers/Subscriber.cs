using TenantX.Common.Domain.Shared;
using TenantX.Subscriptions.Domain.Subscribers.Events;

namespace TenantX.Subscriptions.Domain.Subscribers;

/// <summary>
/// Represents the identity link between a Tenant's user and the GSEE engine.
/// This aggregate serves as the anchor for all subscription history.
/// </summary>
/// <remarks>
/// <b>Main Use:</b>
/// Mapping an <see cref="ExternalUserId"/> (from the client system) to our internal 
/// <see cref="SubscriberId"/>. It stores no PII (Personally Identifiable Information).
/// </remarks>
public sealed class Subscriber : TenantScopedAggregateRoot<SubscriberId>
{
  /// <summary>
  /// The unique identifier provided by the Tenant's system.
  /// </summary>
  public ExternalUserId ExternalUserId { get; private set; }

  /// <summary>
  /// The timestamp of when this user first interacted with the subscription engine.
  /// </summary>
  public DateTime JoinedAtUtc { get; private set; }

  // Required for EF Core
#pragma warning disable CS8618
  private Subscriber() { }
#pragma warning restore CS8618

  private Subscriber(SubscriberId id, ExternalUserId externalUserId) : base(id)
  {
    ExternalUserId = externalUserId;
    JoinedAtUtc = DateTime.UtcNow;
  }

  /// <summary>
  /// Factory method for creating a new Subscriber record.
  /// Follows the "Always-Valid" principle by accepting a pre-validated ExternalUserId.
  /// </summary>
  /// <param name="externalUserId">The validated ID from the external system.</param>
  /// <returns>A new Subscriber instance.</returns>
  public static Subscriber Create(ExternalUserId externalUserId)
  {
    var subscriber = new Subscriber(SubscriberId.New(), externalUserId);

    subscriber.RaiseDomainEvent(new SubscriberCreatedDomainEvent(subscriber.Id, subscriber.ExternalUserId));

    return subscriber;
  }
}
