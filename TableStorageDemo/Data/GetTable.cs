using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TableStorageDemo.Data
{
    public static class GetTable
    {
        public static async Task<CloudTable> GetTableAsync()
        {
            //Get Config from appSettings.json
            IConfiguration config = GetConfiguration();
            //Account
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["ConnectionStrings:TableStorage"]);
            //Client
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            //Table
            CloudTable table = tableClient.GetTableReference(config["AzureTable:Name"]);
            return table;
        }
        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();
        }
    }
}
