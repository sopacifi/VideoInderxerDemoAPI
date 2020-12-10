using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VideoIndexerApiClient
{
    public class MediaAssetsContainer
    {
        private CloudBlobContainer m_BlobContainer;

        public MediaAssetsContainer(string storageConnection, string containerName)
        {
            m_BlobContainer = CloudStorageAccount.Parse(storageConnection).CreateCloudBlobClient().GetContainerReference(containerName);
        }

        public Task UploadFileAsync(string path, string blobName)
        {
            var blob = m_BlobContainer.GetBlockBlobReference(blobName);
            return blob.UploadFromFileAsync(path);
        }


        public string GetBlobReadSas(string blobName)
        {
            var sharedAccessBlobPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
            };
            var blob = m_BlobContainer.GetBlockBlobReference(blobName);
            var sas = blob.GetSharedAccessSignature(sharedAccessBlobPolicy);
            var sasUrl = blob.StorageUri.PrimaryUri.AbsoluteUri + sas;

            return sasUrl;
        }

    }
}
