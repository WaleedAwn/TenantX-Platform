using Bogus;

using TenantX.Common.Domain;
using TenantX.Common.Domain.Shared;

namespace TenantX.Common.UnitTests.Abstractions;

public abstract class BaseTest
{

	protected static readonly Faker Faker = new();
	/// <summary>
	/// Changed 'Entity' to 'IHasDomainEvents' to support all aggregate types.
	/// </summary>
	public static T AssertDomainEventWasPublished<T>(IHasDomainEvents entity) where T : IDomainEvent
	{
		T? domainEvent = entity.DomainEvents.OfType<T>().SingleOrDefault();
		if (domainEvent is null)
		{
			throw new Exception($"Expected domain event of type {typeof(T).Name} was not published.");
		}
		return domainEvent;
	}

}
