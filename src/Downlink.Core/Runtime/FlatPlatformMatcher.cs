using System;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class FlatPlatformMatcher : IPatternMatcher
    {
        private readonly IFormatParser _parser;
        private readonly bool _forceNameMatch;

        public string Name => "FlatPlatform";

        public FlatPlatformMatcher(IFormatParser parser, DownlinkMatchConventions conventions)
        {
            _parser = parser;
            _forceNameMatch = conventions.ForceNameMatching;
        }
        public bool Match(Path path, VersionSpec version)
        {
            var search = $"{version.Platform}/";
            var pathMatch = path.FullPath.StartsWith(search);
            var nameMatch = _forceNameMatch 
                ? path.GetFilenameWithoutExtension().Contains(version.ToString())
                : true;
            var platMatch = path.GetFilenameWithoutExtension().Contains(version.Architecture);
            var formatMatch = string.IsNullOrWhiteSpace(version.Format)
                ? true
                : _parser.GetFormat(path).ToLower() == version.Format.ToLower();
            return pathMatch && nameMatch && platMatch && formatMatch;
        }
    }
}