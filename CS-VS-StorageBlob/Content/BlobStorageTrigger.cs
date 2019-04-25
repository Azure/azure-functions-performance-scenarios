using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CSharpPrecompiledStorageBlob
{
    public static class BlobStorageTrigger
    {
        public static int messagesLeft = 0;

        [FunctionName("Function1")]
        public static void Run([BlobTrigger("blob-test/folder/{name}", Connection = "StorageConnection")]Stream myBlob, string name, ILogger log)
        {
            messagesLeft--;
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
