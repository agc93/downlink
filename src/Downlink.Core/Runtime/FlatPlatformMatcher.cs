using System;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class FlatPlatformMatcher : IPatternMatcher
    {
        private readonly bool _forceNameMatch;

        public string Name => "FlatPlatform";

        public FlatPlatformMatcher(bool forceNameMatching = false)
        {
            _forceNameMatch = forceNameMatching;
        }
        public bool Match(Path path, VersionSpec version)
        {
            var search = $"{version.Platform}/";
            var pathMatch = path.FullPath.StartsWith(search);
            var nameMatch = _forceNameMatch 
                ? path.GetFilenameWithoutExtension().Contains(version.ToString())
                : true;
            var platMatch = path.GetFilenameWithoutExtension().Contains(version.Architecture) &&
                    path.GetFilenameWithoutExtension().Contains(version.ToString());
            return pathMatch && nameMatch && platMatch;
        }
    }
}