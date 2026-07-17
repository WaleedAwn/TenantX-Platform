using MassTransit;

using TenantX.Common.Application.EventBus;

namespace TenantX.Common.Infrastructure.EventBuses;

internal sealed class EventBus(IBus bus) : IEventBus
{
	public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IIntegrationEvent
	{
		await bus.Publish(integrationEvent, cancellationToken);
	}
}
