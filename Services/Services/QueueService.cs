using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class QueueService : IQueueService
    {
        public async Task SendMessageASync<T>(string connectionString, T ServiceBusMessage, string queueName)
        {
            var queueClient = new QueueClient(connectionString, queueName);
            string messageBody = JsonSerializer.Serialize(ServiceBusMessage);
            var jsonMessage = new Message(Encoding.UTF8.GetBytes(messageBody));

            await queueClient.SendAsync(jsonMessage);

        }
    }
}
