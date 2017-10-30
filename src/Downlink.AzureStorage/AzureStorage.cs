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
        private readonly ILogger<AzureStorage> _logger;
        private readonly IConfiguration _configuration;

        public string Name => "Azure Storage";

        public AzureStorage(
            IConfiguration configuration,
            ILogger<AzureStorage> logger,
            IEnumerable<IPatternMatcher> matchers,
            IEnumerable<AzureMatchStrategy> strategies)
        {
            _logger = logger;
            _configuration = configuration;
            var stratName = configuration.GetValue("AzureStorage:MatchStrategy", "Hierarchical");
            MatchStrategy = strategies.GetFor<AzureMatchStrategy, IListBlobItem>(stratName);
            PatternMatcher = matchers.GetFor(stratName);
            _logger.LogDebug($"Using {MatchStrategy?.Name ?? PatternMatcher?.Name}");
        }

        public CloudStorageAccount Account { get; private set; }
        public CloudBlobClient Client { get; private set; }

        private string ContainerName { get; set; }
        private AzureMatchStrategy MatchStrategy { get; }
        private IPatternMatcher PatternMatcher {get;}

        private void BuildServices() {
            var connectionString = _configuration.GetConnectionString("AzureStorage") 
                ?? _configuration.GetValue<string>("AzureStorage:ConnectionString");
            Account = CloudStorageAccount.Parse(connectionString);
            Client = Account.CreateCloudBlobClient();
            ContainerName = _configuration.GetSection("AzureStorage:Container").Value;
            _logger.LogInformation($"Connected to '{ContainerName} using {MatchStrategy?.Name ?? PatternMatcher?.Name}");
        }

        public async Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            BuildServices();
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
                    if (blob == null) throw new VersionNotFoundException($"No matches found from {blobs.Count} files using {PatternMatcher?.Name} matching!");
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
