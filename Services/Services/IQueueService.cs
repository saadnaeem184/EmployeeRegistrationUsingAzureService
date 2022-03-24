using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IQueueService
    {
        Task SendMessageASync<T>(string connectionString, T ServiceBusMessage, string queueName);
    }
}
