using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CSharpPrecompiledStorageQueue
{
    public static class QueueStorageTrigger
    {
        public static int messagesLeft = 0;

        [FunctionName("Function1")]
        public static void Run([QueueTrigger("queue-test", Connection = "StorageConnection")]string myQueueItem, ILogger log)
        {
            messagesLeft--;
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
