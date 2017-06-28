using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.AzureStorage.Strategies;
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
            ILogger logger,
            IEnumerable<IPatternMatcher> matchers,
            IEnumerable<AzureMatchStrategy> strategies)
        {
            _logger = logger;
            var connectionString = configuration.GetConnectionString("AzureStorage");
            Account = CloudStorageAccount.Parse(connectionString);
            Client = Account.CreateCloudBlobClient();
            ContainerName = configuration.GetSection("AzureStorage:Container").Value;
            var stratName = configuration.GetValue("AzureStorage:MatchStrategy", "Hierarchical");
            MatchStrategy = strategies.FirstOrDefault(s => s.Name == stratName);
            PatternMatcher = matchers.FirstOrDefault(m => m.Name == stratName) ?? new Downlink.Core.Runtime.HierarchicalPatternMatcher();
        }

        public CloudStorageAccount Account { get; private set; }
        public CloudBlobClient Client { get; private set; }

        private string ContainerName { get; }
        private AzureMatchStrategy MatchStrategy { get; }
        private IPatternMatcher PatternMatcher {get;}

        public async Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            var container = Client.GetContainerReference(ContainerName);
            await container.PrepContainer();
            var path = BuildPath(version);
            var blobs = await container.ListBlobsAsync();
            if (MatchStrategy == null) {
                var blob = blobs
                    .OfType<CloudBlockBlob>()
                    .FirstOrDefault(
                        b => PatternMatcher.Match(b.Name, version)
                    );
                    if (blob == null) throw new VersionNotFoundException();
                return blob.ToSource(version);
            }
            var match = await MatchStrategy.MatchAsync(blobs, version);
            return match;
        }

        private static string BuildPath(VersionSpec version)
        {
            var path = $"{version.Platform}/{version.Architecture}";
            return path;
        }
    }
}
