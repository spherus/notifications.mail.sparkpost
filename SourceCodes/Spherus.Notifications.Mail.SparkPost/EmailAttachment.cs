using Newtonsoft.Json;

namespace Spherus.Notifications.Mail.SparkPost
{
    internal class EmailAttachment
    {
        [JsonProperty("type")]
        protected internal string Type { get; set; }

        [JsonProperty("name")]
        protected internal string Name { get; set; }

        [JsonProperty("data")]
        protected internal byte[] Data { get; set; }
    }
}