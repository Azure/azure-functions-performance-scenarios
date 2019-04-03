using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure.ServiceBus;
using System.Text;

namespace ServiceBus
{
    public static class HttpTrigger
    {
        [FunctionName("HttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"Function started {DateTime.UtcNow}");

            // Clear queue items
            int deletedMessagesCount = await ServiceBusHelper.CleanUpEntity("queue-input");
            log.LogInformation($"The queue is empty. Deleted {deletedMessagesCount}. {DateTime.UtcNow}");

            int.TryParse(req.Query["count"], out int count);
            ServiceBusTrigger.messagesLeft = count;

            if (count == 0)
            {
                return new BadRequestObjectResult("Count is not defined");
            }

            // Init
            await ServiceBusHelper.AddMessages("queue-input", count);
            log.LogInformation($"{count} messages added to the queue {DateTime.UtcNow}");

            // Wait until all messages are processed
            do
            {
                await Task.Delay(100);
            } while (ServiceBusTrigger.messagesLeft > 0);

            log.LogInformation($"{count} messages were processed {DateTime.UtcNow}");

            return new OkObjectResult($"Count: {count}");
        }
    }
}
