using CleanTib.Application.Common.Events;
using CleanTib.Shared.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanTib.Infrastructure.Common.Services;

public class EventPublisher : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger;
    private readonly IPublisher _mediator;

    public EventPublisher(ILogger<EventPublisher> logger, IPublisher mediator) =>
        (_logger, _mediator) = (logger, mediator);

    public Task PublishAsync(IEvent @event)
    {
        _logger.LogInformation("Publishing Event : {event}", @event.GetType().Name);
        return _mediator.Publish(CreateEventNotification(@event));
    }

    private static INotification CreateEventNotification(IEvent @event) =>
        (INotification)Activator.CreateInstance(
            typeof(EventNotification<>).MakeGenericType(@event.GetType()), @event)!;
}