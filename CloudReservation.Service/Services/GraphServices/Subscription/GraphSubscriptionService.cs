using CloudReservation.Service.Services.GraphServices.Builder;
using CloudReservation.Service.Wrappers.ConfigurationWrapper;
using Microsoft.Graph;

namespace CloudReservation.Service.Services.GraphServices.Subscription;

public class GraphSubscriptionService : BackgroundService
{
    private readonly GraphServiceClient _graphService;
    private readonly string[] _rooms;
    private readonly List<Microsoft.Graph.Models.Subscription> _subscriptions = new();

    public GraphSubscriptionService(IGraphBuilder graphBuilder, IConfigurationWrapper configurationWrapper)
    {
        _rooms = configurationWrapper.GetMsGraphConfiguration<string[]>(ConfigurationType.Rooms);
        _graphService = graphBuilder.Build();
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await CancelSubscriptions();
        await SubscribeToEvents();

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(25));
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await RenewSubscriptions();
            }
        }
        catch (TaskCanceledException)
        {
            // Do nothing
        }
    }

    private async Task SubscribeToEvents()
    {
        foreach(var room in _rooms)
        {
            var subscription = CreateSubscription(room);

            Console.WriteLine(subscription.Resource);
            var result = await _graphService.Subscriptions.PostAsync(subscription);

            if (result == null) continue;

            _subscriptions.Add(result);
        }
    }

    private async Task RenewSubscriptions()
    {
        var subTemps = new List<Microsoft.Graph.Models.Subscription>();

        foreach (var sub in _subscriptions)
        {
            var subscription = CreateSubscription(sub.Id!);

            var result = await _graphService.Subscriptions[sub.Id].PatchAsync(subscription);

            if (result == null) continue;

            subTemps.Add(result);
        }

        _subscriptions.Clear();
        _subscriptions.AddRange(subTemps);
    }

    private async Task CancelSubscriptions()
    {
        var subs = await _graphService.Subscriptions.GetAsync();

        if (subs?.Value is null || subs.Value.Count == 0) return;

        foreach (var subId in subs.Value)
        {
            Console.WriteLine($"Deleting subscription: {subId.Id}");
            await _graphService.Subscriptions[subId.Id].DeleteAsync();
        }
    }

    private static Microsoft.Graph.Models.Subscription CreateSubscription(string room)
    {
        return new Microsoft.Graph.Models.Subscription
        {
            ChangeType = "updated, created, deleted",
            NotificationUrl = "https://2e69-77-213-122-76.ngrok.io/api/notification/listen-second",
            Resource = $"/users/{room}/events",
            ExpirationDateTime = DateTime.UtcNow.AddMinutes(15),
            ClientState = Guid.NewGuid().ToString()
        };
    }
}