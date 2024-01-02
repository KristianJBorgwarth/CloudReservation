using CloudReservation.Service.Models.NotificationModels;
using Microsoft.Extensions.Caching.Memory;

namespace CloudReservation.Service.Services.NotificationCacheService;

public class NotificationCacheService : INotificationCacheService
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());
    
    public bool IsDuplicateNotification(Notification notification)
    {
        var cacheKey = GetCacheKey(notification);
        if (_cache.TryGetValue(cacheKey, out _))
        {
            return true;
        }

        _cache.Set(cacheKey, notification, TimeSpan.FromSeconds(50));
        return false;
    }
    
    private static string GetCacheKey(Notification notification)
    {
        return $"{notification.SubscriptionId}-{notification.ResourceData.Id}";
    }
}