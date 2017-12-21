using Newtonsoft.Json;

namespace Spherus.Notifications.Mail.SparkPost
{
    internal class EmailAddress
    {
        [JsonProperty("name")]
        protected internal string Name { get; set; }

        [JsonProperty("email")]
        protected internal string Email { get; set; }

    }
}