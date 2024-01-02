using CloudReservation.Service.Models.NotificationModels;
using Microsoft.AspNetCore.SignalR;

namespace CloudReservation.Service.Hubs;

public class NotificationHub : Hub
{
    public async Task SubscribeToNotifications(string subscriptionName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, subscriptionName);
    }

    public async Task SendNotification(NotificationEventDto notification)
    {
        await Clients.Groups(notification.Room).SendAsync("ReceiveNotification", notification);
    }
}