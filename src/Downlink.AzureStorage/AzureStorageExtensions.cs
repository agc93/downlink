using System.Collections.Generic;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Downlink.AzureStorage
{
    public static class AzureStorageExtensions
    {
        public static async Task PrepContainer(this CloudBlobContainer container)
        {
            if (!(await container.ExistsAsync()))
            {
                await container.CreateIfNotExistsAsync();
            }
            var perms = await container.GetPermissionsAsync();
            if (!(perms.PublicAccess == BlobContainerPublicAccessType.Blob || perms.PublicAccess == BlobContainerPublicAccessType.Container))
            {
                await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }
        }

        public static IFileSource ToSource(this CloudBlockBlob blob, VersionSpec version)
        {
            return new AzureStorageFileSource(blob.Uri)
            {
                Version = version,
                Metadata = new FileMetadata(blob.Properties.Length, blob.Name)
            };
        }

        public static async Task<List<IListBlobItem>> ListBlobsAsync(this CloudBlobContainer container)
        {
            BlobContinuationToken continuationToken = null;
            List<IListBlobItem> results = new List<IListBlobItem>();
            do
            {
                var response = await container.ListBlobsSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results;
        }

        internal static bool Matches(this IListBlobItem b, string path) {
            return b is CloudBlockBlob &&
            ((CloudBlockBlob)b).Name.StartsWith(path);
        }

        internal static bool IsVersion(this IListBlobItem b, VersionSpec version) {
            return b is CloudBlockBlob &&
            ((CloudBlockBlob)b).Name.Contains(version.ToString());
        }
    }
}
