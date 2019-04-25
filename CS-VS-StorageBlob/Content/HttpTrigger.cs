using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CSharpPrecompiledStorageBlob
{
    public static class HttpTrigger
    {
        [FunctionName("HttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Clear container
            string containerName = "blob-test";
            StorageHelper sh = new StorageHelper(Environment.GetEnvironmentVariable("StorageConnection", EnvironmentVariableTarget.Process));
            await sh.ClearContainer("azure-webjobs-hosts", "blobreceipts");
            log.LogInformation($"azure-webjobs-hosts\blobreceipts cleared {DateTime.UtcNow}");
            await sh.ClearContainer(containerName, "folder");
            log.LogInformation($"{containerName} cleared {DateTime.UtcNow}");

            int.TryParse(req.Query["count"], out int count);
            BlobStorageTrigger.messagesLeft = count;
            if (count == 0)
            {
                return new BadRequestObjectResult("Count is not defined");
            }

            // Init
            await sh.AddBlobsAsync(containerName, "folder", count);
            log.LogInformation($"{count} messages added to the queue {DateTime.UtcNow}");

            // Wait until all messages are processed
            do
            {
                await Task.Delay(100);
            } while (BlobStorageTrigger.messagesLeft > 0);

            log.LogInformation($"{count} messages were processed {DateTime.UtcNow}");

            return new OkObjectResult($"Count: {count}");
        }
    }
}
