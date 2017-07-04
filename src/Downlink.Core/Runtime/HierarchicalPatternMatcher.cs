using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class HierarchicalPatternMatcher : IPatternMatcher
    {
        private readonly bool _forceNameMatch;

        public string Name => "Hierarchical";

        public HierarchicalPatternMatcher(bool forceNameMatching = false)
        {
            _forceNameMatch = forceNameMatching;
        }

        public bool Match(Path path, VersionSpec version)
        {
            var search = $"{version.ToString()}/{version.Platform}/{version.Architecture}";
            var matching = path.FullPath.StartsWith(search) ||
                path.FullPath.TrimStart('.', '/').StartsWith(search);
            var nameMatch = _forceNameMatch ? path.GetFilename().Contains(version) : true;
            return matching && nameMatch;
        }
    }
}
