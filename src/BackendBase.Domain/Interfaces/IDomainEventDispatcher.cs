using BackendBase.Domain.Abstract;

namespace BackendBase.Domain.Interfaces;
public interface IDomainEventDispatcher
{
    Task DispatchAndClearEvents(IEnumerable<DomainEntity> entitiesWithEvents);
}
