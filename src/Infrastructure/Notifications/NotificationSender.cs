using CleanTib.Application.Common.Interfaces;
using CleanTib.Shared.Notifications;
using Microsoft.AspNetCore.SignalR;
using static CleanTib.Shared.Notifications.NotificationConstants;

namespace CleanTib.Infrastructure.Notifications;

public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _notificationHubContext;

    public NotificationSender(IHubContext<NotificationHub> notificationHubContext) =>
        _notificationHubContext = notificationHubContext;

    public Task BroadcastAsync(INotificationMessage notification, CancellationToken cancellationToken) =>
        _notificationHubContext.Clients.All
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);

    public Task BroadcastAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken) =>
        _notificationHubContext.Clients.AllExcept(excludedConnectionIds)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);

    public Task SendToAllAsync(INotificationMessage notification, CancellationToken cancellationToken)
    {
        return Task.FromResult("SendToAllAsync");
    }

    public Task SendToAllAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken)
    {
        return Task.FromResult("SendToAllAsync");
    }

    //public Task SendToAllAsync(INotificationMessage notification, CancellationToken cancellationTo3ken) =>
    //    _notificationHubContext.Clients.Group($"GroupTenant-{_currentTenant.Id}")
    //        .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);

    //public Task SendToAllAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken) =>
    //    _notificationHubContext.Clients.GroupExcept($"GroupTenant-{_currentTenant.Id}", excludedConnectionIds)
    //        .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);

    public Task SendToGroupAsync(INotificationMessage notification, string group, CancellationToken cancellationToken) =>
        _notificationHubContext.Clients.Group(group)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);

    public Task SendToGroupAsync(INotificationMessage notification, string group, IEnumerable<string> excludedConnectionIds, CancellationToken cancellationToken) =>
        _notificationHubContext.Clients.GroupExcept(group, excludedConnectionIds)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);

    public Task SendToGroupsAsync(INotificationMessage notification, IEnumerable<string> groupNames, CancellationToken cancellationToken) =>
        _notificationHubContext.Clients.Groups(groupNames)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);

    public Task SendToUserAsync(INotificationMessage notification, string userId, CancellationToken cancellationToken) =>
        _notificationHubContext.Clients.User(userId)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);

    public Task SendToUsersAsync(INotificationMessage notification, IEnumerable<string> userIds, CancellationToken cancellationToken) =>
        _notificationHubContext.Clients.Users(userIds)
            .SendAsync(NotificationFromServer, notification.GetType().FullName, notification, cancellationToken);
}