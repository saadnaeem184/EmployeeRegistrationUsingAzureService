using APShared.Models;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppServiceBusTopics
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await SendMessage();

        }
        private async static Task SendMessage()
        {
            Employee emp = new Employee();
            Console.WriteLine("Enter First Name");
            emp.FirstName = Console.ReadLine();
            Console.WriteLine("Enter Last Name");
            emp.LastName = Console.ReadLine();
            string EvenOrOdd = GetNumber();
            //string empJson = JsonConvert.SerializeObject(emp);
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(emp)));
            message.SessionId = EvenOrOdd;
            IConfiguration config = GetConfiguration();
            var topicClient = new TopicClient(config.GetSection("ConnectionStrings:AzureConnectionString").Value, config.GetSection("AzureTopics:TopicName").Value);
            //var msg = new BrokeredMessage(empJson);
            await topicClient.SendAsync(message);
            await SendMessage();
        }
        private static string CheckEvenOddNumber(int no)
        {
            no = no % 2;
            return (no == 0) ? "Even" : "Odd";
        }
        private static string GetNumber()
        {
            Console.WriteLine("Enter Number");
            string evenOddCheck = Console.ReadLine();
            int convertedNo = EvenOddNumberCheck(evenOddCheck);
            if (convertedNo == 0)
            {
                Console.WriteLine("InValid No");
                GetNumber();
            }
            return CheckEvenOddNumber(convertedNo);
        }
        private static int EvenOddNumberCheck(string no)
        {
            try
            {
                return int.Parse(no);
            }
            catch (Exception)
            {

                return 0;
            }
        }
        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();
        }
    }
}
