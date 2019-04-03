using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSharpPrecompiledStorageBlob
{
    class StorageHelper
    {
        private const int max_outstanding = 100;
        private readonly string _connectionString;
        private readonly CloudStorageAccount _storageAccount;

        public StorageHelper(string connectionString)
        {
            _connectionString = connectionString;
            _storageAccount = CloudStorageAccount.Parse(_connectionString);
        }

        public async Task ClearContainer(string containerName, string path = null)
        {
            CloudBlobContainer container = _storageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
            var directory = container.GetDirectoryReference(path);
            List<IListBlobItem> results = new List<IListBlobItem>();
            var response = await directory.ListBlobsSegmentedAsync(true, BlobListingDetails.None, null, null, null, null);
            results.AddRange(response.Results);

            // Create a new instance of the SemaphoreSlim class to define the number of threads to use in the application.
            SemaphoreSlim sem = new SemaphoreSlim(max_outstanding, max_outstanding);
            List<Task> tasks = new List<Task>();
            int completed_count = 0;
            foreach (IListBlobItem item in results)
            {
                CloudBlockBlob blob = (CloudBlockBlob)item;
                await sem.WaitAsync();

                // Create tasks for each file that is uploaded. This is added to a collection that executes them all asyncronously.  
                tasks.Add(blob.DeleteAsync().ContinueWith((t) =>
                {
                    sem.Release();
                    Interlocked.Increment(ref completed_count);
                }));
            }
            // Creates an asynchronous task that completes when all the uploads complete.
            await Task.WhenAll(tasks);
        }

        public async Task AddBlobsAsync(string containerName, string path, int count)
        {
            CloudBlobContainer container = _storageAccount.CreateCloudBlobClient().GetContainerReference(containerName);
            int completed_count = 0;

            BlobRequestOptions options = new BlobRequestOptions
            {
                ParallelOperationThreadCount = 8,
                DisableContentMD5Validation = true,
                StoreBlobContentMD5 = false
            };
            // Create a new instance of the SemaphoreSlim class to define the number of threads to use in the application.
            SemaphoreSlim sem = new SemaphoreSlim(max_outstanding, max_outstanding);

            List<Task> tasks = new List<Task>();

            // Iterate through the files
            for (int i=0; i < count; i++)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{path}/test{i}.txt");
                await sem.WaitAsync();

                // Create tasks for each file that is uploaded. This is added to a collection that executes them all asyncronously.  
                tasks.Add(blockBlob.UploadTextAsync("test").ContinueWith((t) =>
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
