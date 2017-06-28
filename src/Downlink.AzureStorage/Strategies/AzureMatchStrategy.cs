using System.Collections.Generic;
using System.Threading.Tasks;
using Downlink.Core;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Downlink.AzureStorage.Strategies
{
    public abstract class AzureMatchStrategy : IMatchStrategy<IListBlobItem>
    {
        protected AzureMatchStrategy(string name) {
            Name = name;
        }
        public string Name { get; }

        public abstract Task<IFileSource> MatchAsync(IEnumerable<IListBlobItem> items, VersionSpec version);
    }
}
