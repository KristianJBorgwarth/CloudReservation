using CloudReservation.Service.Models.NotificationModels;

namespace CloudReservation.Service.Services.NotificationCacheService;

public interface INotificationCacheService
{
    bool IsDuplicateNotification(Notification notification);
}