using System.Collections.Generic;
using System.Threading.Tasks;

namespace Downlink.Core
{
    public abstract class MatchStrategy : IMatchStrategy
    {
        protected MatchStrategy(string name) {
            Name = name;
        }
        public string Name { get; }

        public abstract Task<IFileSource> MatchAsync(IEnumerable<Path> items, VersionSpec version);
    }
}