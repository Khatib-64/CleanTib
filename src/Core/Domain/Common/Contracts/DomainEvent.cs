using CleanTib.Shared.Events;

namespace CleanTib.Domain.Common.Contracts;

public abstract class DomainEvent : IEvent
{
    public DateTime TriggeredOn { get; protected set; } = DateTime.UtcNow;
}