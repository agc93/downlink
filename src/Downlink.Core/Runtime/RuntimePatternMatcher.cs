using System;
using System.Linq;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class RuntimePatternMatcher : IPatternMatcher
    {
        private readonly bool _forceNameMatch;
        public string Name => "Runtime";
        public RuntimePatternMatcher(bool forceNameMatching = false) {
            _forceNameMatch = forceNameMatching;
        }
        public bool Match(Path path, VersionSpec version)
        {
            var search = $"{version.ToString()}/";
            var versionMatch = path.FullPath.StartsWith(search) || path.FullPath.TrimStart('.', '/').StartsWith(search);
            var nextSegment = path.FullPath.TrimStart('/').Split('/').Skip(1).First();
            var platMatch = nextSegment.Contains(version.Platform) && nextSegment.Contains(version.Architecture);
            var nameMatch = _forceNameMatch ? path.GetFilenameWithoutExtension().Contains(version.ToString()) : true;
            return versionMatch && platMatch && nameMatch;
        }
    }
}