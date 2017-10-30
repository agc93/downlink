using System;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class FlatVersionMatcher : IPatternMatcher
    {
        private readonly IFormatParser _parser;
        private readonly bool _forceNameMatch;

        public string Name => "FlatVersion";

        public FlatVersionMatcher(IFormatParser parser, bool forceNameMatching = false)
        {
            _parser = parser;
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
            var formatMatch = string.IsNullOrWhiteSpace(version.Format)
                ? true
                : _parser.GetFormat(path).ToLower() == version.Format.ToLower();
            return pathMatch && nameMatch && platMatch && formatMatch;
        }
    }
}