using System;
using System.IO;
using System.Threading.Tasks;
using APShared.Models;
using Azure.Storage.Blobs;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FunctionAppSerivceBusTopics
{
    public static class Function1
    {
        //[FunctionName("Function1")]
        //public async static Task Run([ServiceBusTrigger("demotopic", "SubscriptionA", Connection = "SBConnection")]string mySbMsg, IMessageSession messageSession,ILogger log)
        //{
        //    log.LogInformation($"Subscription A {mySbMsg}");
        //}
        //[FunctionName("Function2")]
        //public async static Task Run2([ServiceBusTrigger("demotopic", "SubscriptionB", Connection = "SBConnection")] string mySbMsg,IMessageSession messageSession, ILogger log)
        //{

        //    log.LogInformation($"Subscription B {mySbMsg}");
        //}
        ////Even Function
        //[FunctionName("Function3")]
        //public async static Task EvenFunction([ServiceBusTrigger("demotopic", "SubscriptionC", Connection = "SBConnection",IsSessionsEnabled =true)] string mySbMsg, Message newMsg, IMessageSession messageSession, MessageReceiver messageReceiver, ILogger log)
        //{
        //    if (messageSession.SessionId.Equals("Even"))
        //    {
        //        log.LogInformation($"Even Subscription {mySbMsg}");
        //        await messageSession.AbandonAsync(newMsg.SystemProperties.LockToken);
        //        //await messageSession.AbandonAsync(message.SystemProperties.LockToken);
        //        //await messageSession.CompleteAsync(message.SystemProperties.LockToken);
        //    }

        //}
        [StorageAccount("BlobStorageString")]
        [FunctionName("GenerateWelcomeEmail")]
        public static async Task Run([BlobTrigger("democontainer/{name}")] Stream myBlob, string name, ILogger log)
        {
            try
            {
                StreamReader reader = new StreamReader(myBlob);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                Employee vmModel = JsonConvert.DeserializeObject<Employee>(responseFromServer);
                //Send Email
                var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                var client = new SendGridClient(apiKey);
                var from = new EmailAddress("saad_naeem007@hotmail.com", "Demo User");
                var subject = "Employee Registeration";
                var to = new EmailAddress(vmModel.Email, vmModel.FirstName);
                var plainTextContent = "Sending Email Test";
                var htmlContent = "<strong>Hi, "+ vmModel.FirstName + " thank you for registering</strong>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            }
            catch (Exception ex)
            {

                log.LogError("Generating email failed", ex);
            }

        }
        //Save CSV file to blog
        [FunctionName("Function4")]
        public async static Task OddFunction([ServiceBusTrigger("demotopic", "SubscriptionD", Connection = "SBConnection", IsSessionsEnabled = true)] string mySbMsg, string lockToken , IMessageSession messageSession, ILogger log)
        {
            try
            {
                if (!string.IsNullOrEmpty(mySbMsg))
                {
                    string filename = Guid.NewGuid().ToString() + ".csv";

                    BlobServiceClient blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("StorageAccount"));
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("ContainerName"));
                    BlobClient blobClient = containerClient.GetBlobClient(filename);

                    await using var ms = new MemoryStream();
                    //var json = JsonConvert.SerializeObject(emp);
                    var writer = new StreamWriter(ms);
                    await writer.WriteAsync(mySbMsg);
                    await writer.FlushAsync();
                    ms.Position = 0;
                    await blobClient.UploadAsync(ms);
                    await messageSession.CompleteAsync(lockToken);
                }
            }
            catch (Exception ex)
            {
                await messageSession.AbandonAsync(lockToken);
            }

                //log.LogInformation($"Odd Subscription {mySbMsg}");
        }
    }
}
