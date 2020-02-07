using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Remax.WebJobs
{
    public class CloudQueueClientProvider
    {

        private readonly CloudStorageAccount _storageAccount;

        public CloudQueueClientProvider(StorageAccountProvider storageAccountProvider)
        {
            _storageAccount = storageAccountProvider.GetHost().SdkObject;
        }

        public CloudQueueClient Create()
        {
            return _storageAccount.CreateCloudQueueClient();
        }

    }
}