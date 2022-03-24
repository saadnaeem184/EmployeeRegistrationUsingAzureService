using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using FileUploadDemo.Models;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace FileUploadDemo
{
    public static class Function1
    {
        [FunctionName("FileUploadFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,"post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string msg = string.Empty;
            try
            {
                Employee emp = new Employee();
                //param in
                string fname = req.Query["firstname"];
                string lname = req.Query["lastname"];
                string address = req.Query["address"];
                if (!string.IsNullOrEmpty(fname) && !string.IsNullOrEmpty(lname) && !string.IsNullOrEmpty(address))
                {
                    string filename = Guid.NewGuid().ToString() + ".csv";

                    emp.firstname = fname;
                    emp.lastname = lname;
                    emp.address = address;
                    BlobServiceClient blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("StorageAccount"));
                    BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("ContainerName"));
                    BlobClient blobClient = containerClient.GetBlobClient(filename);
                    
                    await using var ms = new MemoryStream();
                    var json = JsonConvert.SerializeObject(emp);
                    var writer = new StreamWriter(ms);
                    await writer.WriteAsync(json);
                    await writer.FlushAsync();
                    ms.Position = 0;
                    await blobClient.UploadAsync(ms);

                    msg = "Data Uploaded";
                }
                else
                {
                    msg = "Please add all params";
                }
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
            
            return new OkObjectResult(msg);
        }
    }
}
