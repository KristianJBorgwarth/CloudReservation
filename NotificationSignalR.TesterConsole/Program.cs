using CloudReservation.Service.Models.NotificationModels;
using Microsoft.AspNetCore.SignalR.Client;

var hubConnection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5010/notificationHub") // Replace with your actual hub URL
    .Build();

hubConnection.On<NotificationEventDto>("ReceiveNotification", (notification) =>
{
    Console.WriteLine($"Notification received:");
    Console.WriteLine($"Room: {notification.Room}");
    Console.WriteLine($"Subject: {notification.Subject}");
    Console.WriteLine($"Start: {notification.Start}");
    Console.WriteLine($"End: {notification.End}");
});

try
{
    await hubConnection.StartAsync();
    Console.WriteLine("Connected to hub");

    await hubConnection.InvokeAsync("SubscribeToNotifications", "Leonardo");
    await hubConnection.InvokeAsync("SubscribeToNotifications", "Donatello");
    await hubConnection.InvokeAsync("SubscribeToNotifications", "Raphael");
    await hubConnection.InvokeAsync("SubscribeToNotifications", "Michelangelo");

    Console.WriteLine("Subscribed to notifications. Press any key to exit");
    Console.ReadKey();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    await hubConnection.StopAsync();
}