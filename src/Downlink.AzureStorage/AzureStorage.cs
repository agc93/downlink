using System;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Downlink.AzureStorage
{
    public class AzureStorage : IRemoteStorage
    {
        private readonly ILogger _logger;

        public AzureStorage(
            IConfiguration configuration,
            ILogger logger)
        {
            _logger = logger;
            var connectionString = configuration.GetConnectionString("AzureStorage");
            Account = CloudStorageAccount.Parse(connectionString);
            Client = Account.CreateCloudBlobClient();
            ContainerName = configuration.GetSection("AzureStorageContainer").Value;
        }

        public CloudStorageAccount Account { get; private set; }
        public CloudBlobClient Client { get; private set; }

        private string ContainerName { get; }

        public async Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            var container = Client.GetContainerReference(ContainerName);
            await container.PrepContainer();
            var path = BuildPath(version);
            var blobs = await container.ListBlobsAsync();
            var matching = blobs.Where(b => b.Matches(path));
            if (!matching.Any()) throw new PlatformNotFoundException();
            var blob = matching.FirstOrDefault(b => b.IsVersion(version));
            if (blob == null) throw new VersionNotFoundException();
            switch (blob)
            {
                case CloudBlockBlob b:
                    await b.FetchAttributesAsync();
                    var source = b.ToSource(version);
                    return source;
                default:
                    throw new NotSupportedException("Downlink does not support page blobs at this time!");
            }
        }

        private static string BuildPath(VersionSpec version)
        {
            var path = $"{version.Platform}/{version.Architecture}";
            return path;
        }
    }
}
