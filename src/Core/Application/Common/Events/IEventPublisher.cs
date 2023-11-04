using CleanTib.Shared.Events;

namespace CleanTib.Application.Common.Events;

public interface IEventPublisher : ITransientService
{
    Task PublishAsync(IEvent @event);
}