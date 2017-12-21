using Spherus.Notifications.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spherus.Notifications.Mail.SparkPost
{
    internal class SparkPostNotificationService : IMailNotificationService
    {

        #region Constructors

        public SparkPostNotificationService()
        {

        }

        public SparkPostNotificationService(MailNotificationModel model)
        {
            mailNotificationModel = model;
        }

        #endregion

        #region Properties

        private MailNotificationModel mailNotificationModel;

        #endregion

        #region Interfaces implementation

        public async Task<ServiceResponse<INotificationResult>> NotifyAsync()
        {
            ServiceResponse<INotificationResult> result = new ServiceResponse<INotificationResult>();

            if (null == mailNotificationModel)
            {
                result.ObjectResult = null;
                result.Status = new ServiceStatus { Code = 2, Message = "MissingNotificationModel" };

                return result;
            }

            SparkPostClient client = new SparkPostClient();
            TransmissionContent transmissionContent = new TransmissionContent
            {
                ApiCredentials = mailNotificationModel.Credentials
            };

            Dictionary<string, string> headers = new Dictionary<string, string>();
            ParseDestinations(headers, DestinationType.CC);
            ParseDestinations(headers, DestinationType.BCC);

            var inlineImages = ParseAttachments(AttachmentType.Image);
            if (inlineImages.Status.Code == 2)
            {
                result.ObjectResult = null;
                result.Status = inlineImages.Status;

                return result;
            }

            var attachments = ParseAttachments(AttachmentType.Attachment);
            if (attachments.Status.Code == 2)
            {
                result.ObjectResult = null;
                result.Status = attachments.Status;

                return result;
            }

            transmissionContent.Content = new EmailContent
            {
                From = new EmailAddress
                {
                    Email = mailNotificationModel.From.Email.Trim(),
                    Name = mailNotificationModel.From.Name.Trim(),
                },
                Subject = mailNotificationModel.Subject.Trim(),
                Html = mailNotificationModel.Text,
                EmailInlineImages = inlineImages.ObjectResult,
                EmailAttachments = attachments.ObjectResult,
                Headers = headers
            };

            ParseReplyTo(transmissionContent);

            var recipients = ParseTo();
            if (recipients.Status.Code == 0)
            {
                transmissionContent.Recipients = recipients.ObjectResult;
            }
            else
            {
                result.ObjectResult = null;
                result.Status = recipients.Status;

                return result;
            }

            var notificationResult = client.SendNotification(transmissionContent);
            notificationResult.Wait();

            result.Status = notificationResult.Result.Status;
            result.ObjectResult = new SparkPostNotificationResult();
            (result.ObjectResult as SparkPostNotificationResult).NotificationResponse = notificationResult.Result;

            return result;
        }

        #endregion

        #region Private Methods

        private void ParseReplyTo(TransmissionContent transmissionContent)
        {
            if (null != mailNotificationModel.ReplyTo)
            {
                if (!string.IsNullOrEmpty(mailNotificationModel.ReplyTo.Email.Trim())
                    && !string.IsNullOrEmpty(mailNotificationModel.ReplyTo.Name.Trim()))
                {
                    transmissionContent.Content.ReplyTo = $"{ mailNotificationModel.ReplyTo.Name.Trim()}<{mailNotificationModel.ReplyTo.Email.Trim()}>";
                }
                else if (!string.IsNullOrEmpty(mailNotificationModel.ReplyTo.Name.Trim()))
                {
                    transmissionContent.Content.ReplyTo = mailNotificationModel.ReplyTo.Email.Trim();
                }
            }
        }

        private void ParseDestinations(Dictionary<string, string> headers, DestinationType destinationType)
        {
            var items = mailNotificationModel
                            .To
                            .Where(t => t.DestinationType == destinationType)
                            .ToList();
            if (items.Count > 0)
            {
                foreach (Mail.Address item in items)
                {
                    headers.Add(destinationType.ToString(), item.Email);
                }
            }
        }

        private ServiceResponse<List<EmailRecipient>> ParseTo()
        {
            ServiceResponse<List<EmailRecipient>> result = new ServiceResponse<List<EmailRecipient>>();

            try
            {
                var items = mailNotificationModel.To
                                .Where(t => t.DestinationType == DestinationType.To)
                                .ToList();
                if (items.Count > 0)
                {
                    result.ObjectResult = new List<EmailRecipient>();
                    foreach (Mail.Address item in items)
                    {
                        result.ObjectResult.Add
                        (
                            new EmailRecipient
                            {
                                Address = new EmailAddress
                                {
                                    Email = item.Email.Trim(),
                                    Name = item.Name.Trim()
                                }
                            }
                        );
                    }
                    result.Status = new ServiceStatus { Code = 0, Message = "Success" };

                    return result;
                }

                result.ObjectResult = null;
                result.Status = new ServiceStatus { Code = 0, Message = "NoRecipientsFound" };

                return result;

            }
            catch
            {
                result.Status = new ServiceStatus { Code = 0, Message = "ServiceError" };
                result.ObjectResult = null;

                return result;
            }
        }

        private ServiceResponse<List<EmailAttachment>> ParseAttachments(AttachmentType attachmentType)
        {
            ServiceResponse<List<EmailAttachment>> result = new ServiceResponse<List<EmailAttachment>>();

            List<Attachment> attachmentList = new List<Attachment>();
            switch (attachmentType)
            {
                case AttachmentType.Image:
                    {
                        attachmentList = (mailNotificationModel.Images as List<Attachment>);
                        break;
                    }
                case AttachmentType.Attachment:
                    {
                        attachmentList = (mailNotificationModel.Attachments as List<Attachment>);
                        break;
                    }
            }

            try
            {
                if (attachmentList.Count > 0)
                {
                    result.ObjectResult = new List<EmailAttachment>();
                    attachmentList.ForEach(x => result.ObjectResult.Add
                    (
                        new EmailAttachment
                        {
                            Data = x.Data,
                            Name = x.Name,
                            Type = x.Type
                        }
                    ));

                    result.Status = new ServiceStatus { Code = 0, Message = "Success" };
                    return result;
                }

                return result;
            }
            catch
            {
                result.ObjectResult = null;
                result.Status = new ServiceStatus
                {
                    Code = 2,
                    Message = attachmentType == AttachmentType.Attachment ? "NoAttachmentsFound" : "NoImagesFound"
                };

                return result;
            }
        }

        #endregion

        #region Enums

        private enum AttachmentType
        {
            Image = 0,
            Attachment = 1
        }

        #endregion
    }
}