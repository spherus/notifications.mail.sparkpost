# Spherus.Notifications.Mail.SparkPost
Notifications Provider for SparkPost email delivery service

## NuGet

[Link to the NuGet package](https://www.nuget.org/packages/Spherus.Notifications.Mail.SparkPost)

**Package Manager: PM>** Install-Package Spherus.Notifications.Mail.SparkPost -Version 1.0.1 <br />
**.NET CLI: >** dotnet add package Spherus.Notifications.Mail.SparkPost --version 1.0.1

### How to use:

```csharp

var model = new MailNotificationModel
{
    From = new Address
    {
        Email = "from@yourmail",
        Name = "Your Name"
    },
    Subject = "The subject of email here",
    To = new List<Address>
    {
         new Address
         {
             Email = "to@yourmail",
             Name = "Recipient Name",
             DestinationType = DestinationType.To //This is default, is not mandatory
         },
         new Address
         {
              Email = "cc_email@yourmail",
              Name = "CC Name",
              DestinationType = DestinationType.CC
         },
         new Address
         {
             Email = "bcc_email@yourmail",
             Name = "BCC Name",
             DestinationType = DestinationType.BCC
         }
    },
    Text = "<b>Hello</b> from SparkPost. This is <i>a message</i><img src='cid:ImageName' />",
    ReplyTo = new Address 
    { 
             Email = "reply@yourmail", 
             Name = "Reply Name" 
    }
};

//If attachments are needed
model.Attachments = new List<Attachment>
{
    new Attachment
    {
        Data = File.ReadAllBytes("Path to a pdf file"),
        Name = "PDF",
        Type = "application/pdf"
    }
};

//If inline images are needed
model.Images = new List<Attachment>
{
    new Attachment
    {
        Data = File.ReadAllBytes("Path to inline image"),
        Name = "ImageName", // Note that it should be unique
        Type = "image/png"
    }
};

model.Credentials.Add("ApiKey", "Your SparkPost API Key");
model.Credentials.Add("URI", new Uri("https://api.sparkpost.com/api/v1/transmissions"));

var result = await new NotificationProvider()
    .UseSparkPostEmailProvider(model)
    .NotificationService.NotifyAsync();

```
