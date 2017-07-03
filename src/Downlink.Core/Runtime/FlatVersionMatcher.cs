using System;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class FlatVersionMatcher : IPatternMatcher
    {
        private readonly bool _forceNameMatch;

        public string Name => "FlatVersion";

        public FlatVersionMatcher(bool forceNameMatching = false)
        {
            _forceNameMatch = forceNameMatching;
        }
        public bool Match(Path path, VersionSpec version)
        {
            var search = $"{version.ToString()}/";
            var pathMatch = path.FullPath.StartsWith(search);
            var nameMatch = _forceNameMatch 
                ? path.GetFilenameWithoutExtension().Contains(version.ToString())
                : true;
            var platMatch = path.GetFilenameWithoutExtension().Contains(version.Architecture) &&
                    path.GetFilenameWithoutExtension().Contains(version.Platform);
            return pathMatch && nameMatch && platMatch;
        }
    }
}