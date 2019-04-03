using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpPrecompiledStorageQueue
{
    class StorageHelper
    {
        private readonly string _connectionString;
        private readonly CloudStorageAccount _storageAccount;

        public StorageHelper(string connectionString)
        {
            _connectionString = connectionString;
            _storageAccount = CloudStorageAccount.Parse(_connectionString);
        }

        public async Task AddMessagesAsync(string queueName, int count)
        {
            CloudQueue queue = _storageAccount.CreateCloudQueueClient().GetQueueReference(queueName);
            int max_outstanding = 100;
            int completed_count = 0;

            // Create a new instance of the SemaphoreSlim class to define the number of threads to use in the application.
            SemaphoreSlim sem = new SemaphoreSlim(max_outstanding, max_outstanding);

            List<Task> tasks = new List<Task>();

            // Iterate through the files
            for (int i = 0; i < count; i++)
            {
                
                await sem.WaitAsync();

                // Create tasks for each file that is uploaded. This is added to a collection that executes them all asyncronously.  
                tasks.Add(queue.AddMessageAsync(new CloudQueueMessage($"test{i}"), TimeSpan.FromHours(2), null, null, null).ContinueWith((t) =>
                {
                    sem.Release();
                    Interlocked.Increment(ref completed_count);
                }));
            }

            // Creates an asynchronous task that completes when all the uploads complete.
            await Task.WhenAll(tasks);
        }
    }
}
