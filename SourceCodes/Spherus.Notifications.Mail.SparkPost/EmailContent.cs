using Newtonsoft.Json;
using System.Collections.Generic;

namespace Spherus.Notifications.Mail.SparkPost
{
    internal class EmailContent
    {
        public EmailContent()
        {
            From = new EmailAddress();
            EmailAttachments = new List<EmailAttachment>();
            EmailInlineImages = new List<EmailAttachment>();
        }

        [JsonProperty("html")]
        protected internal string Html { get; set; }

        [JsonProperty("subject")]
        protected internal string Subject { get; set; }

        [JsonProperty("from")]
        protected internal EmailAddress From { get; set; }

        [JsonProperty("reply_to")]
        protected internal string ReplyTo { get; set; }

        [JsonProperty("headers")]
        protected internal Dictionary<string, string> Headers { get; set; }

        [JsonProperty("attachments")]
        protected internal List<EmailAttachment> EmailAttachments { get; set; }

        [JsonProperty("inline_images")]
        protected internal List<EmailAttachment> EmailInlineImages { get; set; }
    }
}