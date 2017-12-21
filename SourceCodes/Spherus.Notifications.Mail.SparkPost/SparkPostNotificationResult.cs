using Spherus.Notifications.Interfaces;

namespace Spherus.Notifications.Mail.SparkPost
{
    public class SparkPostNotificationResult : INotificationResult
    {
        public object NotificationResponse { get; set; }
    }
}
