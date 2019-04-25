using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ServiceBus
{
    public static class ServiceBusTrigger
    {
        public static int messagesLeft = 0;

        [FunctionName("ServiceBusTrigger")]
        public static void Run([ServiceBusTrigger("queue-input", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
        {
            messagesLeft--;
            log.LogInformation($"Left: {ServiceBusTrigger.messagesLeft}");
        }
    }
}
