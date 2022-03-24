using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace BlobTriggerFunction
{
    [StorageAccount("BlobStorageString")]
    public static class Function1
    {
        [FunctionName("GenerateWelcomeEmail")]
        public static void Run([BlobTrigger("samples-workitems/{name}")]Stream myBlob, string name, ILogger log)
        {
            try
            {
                log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            }
            catch (Exception ex)
            {

                log.LogError("Generating email failed",ex);
            }
            
        }
    }
}
