using APShared.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Services.Services;
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiverApplication
{
    class Program
    {
        static IQueueClient queueClient;
        static async Task Main(string[] args)
        {
            IConfiguration config = GetConfiguration();
            string ConnectionString = config.GetSection("ConnectionStrings:AzureConnectionString").Value;
            string queueName = config.GetSection("AzureQueues:TestQueue").Value;
            queueClient = new QueueClient(ConnectionString, queueName);

            var MessageHandlerOptions = new MessageHandlerOptions(Exceptionhandler)
            {
                MaxConcurrentCalls = 1, // Process 1 message at a time
                AutoComplete = false
            };
            queueClient.RegisterMessageHandler(ProcessMessagesAsync,MessageHandlerOptions);
            Console.ReadLine();
            await queueClient.CloseAsync();
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var jsonString = Encoding.UTF8.GetString(message.Body);
            Employee model = JsonSerializer.Deserialize<Employee>(jsonString);
            Console.WriteLine($"ServiceBusMessage Employee Detail-> First Name {model.FirstName} ,Last Name {model.LastName}, Email {model.Email}");

            await queueClient.CompleteAsync(message.SystemProperties.LockToken); //3 second lock configured on Azure
        }

        private static Task Exceptionhandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine($"Exception :{arg.Exception}");
            return Task.CompletedTask;
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();
        }
    }
}
