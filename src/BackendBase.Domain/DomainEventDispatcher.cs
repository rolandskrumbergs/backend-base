using System.Threading.Channels;
using BackendBase.Domain.Abstract;
using BackendBase.Domain.Interfaces;

namespace BackendBase.Domain;
public class DomainEventDispatcher(ChannelWriter<DomainEvent> mediator) : IDomainEventDispatcher
{
    private readonly ChannelWriter<DomainEvent> _channelWriter = mediator;

    public async Task DispatchAndClearEvents(IEnumerable<DomainEntity> entitiesWithEvents)
    {
        ArgumentNullException.ThrowIfNull(entitiesWithEvents);

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var domainEvent in events)
            {
                await _channelWriter.WriteAsync(domainEvent).ConfigureAwait(false);
            }
        }
    }
}