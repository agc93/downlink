using System;
using System.Linq;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class RuntimePatternMatcher : IPatternMatcher
    {
        private readonly IFormatParser _parser;
        private readonly bool _forceNameMatch;
        public string Name => "Runtime";
        public RuntimePatternMatcher(IFormatParser parser, DownlinkMatchConventions conventions) {
            _parser = parser;
            _forceNameMatch = conventions.ForceNameMatching;
        }
        public bool Match(Path path, VersionSpec version)
        {
            var search = $"{version.ToString()}/";
            var versionMatch = path.FullPath.StartsWith(search);
            var nextSegment = path.FullPath.Split('/').Skip(1).First();
            var platMatch = nextSegment.Contains(version.Platform) && nextSegment.Contains(version.Architecture);
            var nameMatch = _forceNameMatch ? path.GetFilenameWithoutExtension().Contains(version.ToString()) : true;
            var formatMatch = string.IsNullOrWhiteSpace(version.Format)
                ? true
                : _parser.GetFormat(path).ToLower() == version.Format.ToLower();
            return versionMatch && platMatch && nameMatch && formatMatch;
        }
    }
}