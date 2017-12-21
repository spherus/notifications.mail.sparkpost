using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Spherus.Notifications.Mail.SparkPost
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class TransmissionContent : HttpContent
    {
        public TransmissionContent()
        {
            Content = new EmailContent();
            Recipients = new List<EmailRecipient>();
        }

        [JsonProperty("recipients")]
        protected internal List<EmailRecipient> Recipients { get; set; }

        [JsonProperty("content")]
        protected internal EmailContent Content { get; set; }

        protected internal IDictionary<string, object> ApiCredentials { get; set; }

        protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            var content = GetContent();
            await stream.WriteAsync(content, 0, content.Length);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = GetContent().Length;
            return true;
        }

        protected byte[] GetContent()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}