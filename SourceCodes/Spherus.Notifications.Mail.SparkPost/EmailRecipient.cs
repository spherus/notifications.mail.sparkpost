using Newtonsoft.Json;

namespace Spherus.Notifications.Mail.SparkPost
{
    internal class EmailRecipient
    {
        internal EmailRecipient()
        {
            Address = new EmailAddress();
        }

        [JsonProperty("address")]
        protected internal EmailAddress Address { get; set; }
    }
}