using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Downlink.Core;

namespace Downlink.S3
{
    public abstract class S3MatchStrategy : IMatchStrategy<S3Object>
    {
        protected S3MatchStrategy(string name)
        {
            Name = name;
        }
        public string Name { get; }

        public abstract Task<IFileSource> MatchAsync(IEnumerable<S3Object> items, VersionSpec version);
    }
}
