using System.Collections.Generic;
using System.Threading.Tasks;
using Downlink.Core.IO;

namespace Downlink.Core
{
    public interface IMatchStrategy<TObject> where TObject : class
    {
        string Name { get; }
        Task<IFileSource> MatchAsync(IEnumerable<TObject> items, VersionSpec version);
    }
}