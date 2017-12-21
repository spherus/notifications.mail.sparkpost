namespace Spherus.Notifications.Mail.SparkPost
{
    public static class SparkPostEmailProvider
    {
        public static NotificationProvider UseSparkPostEmailProvider(this NotificationProvider provider, MailNotificationModel model)
        {
            provider.NotificationService = new SparkPostNotificationService(model);
            return provider;
        }

        public static NotificationProvider UseSparkPostEmailProvider(this NotificationProvider provider)
        {
            provider.NotificationService = new SparkPostNotificationService();
            return provider;
        }
    }
}