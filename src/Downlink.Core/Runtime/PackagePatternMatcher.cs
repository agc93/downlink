using System.Linq;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class PackagePatternMatcher : IPatternMatcher
    {
        private readonly IFormatParser _parser;
        private readonly bool _forceNameMatch;

        public string Name => "Package";

        public PackagePatternMatcher(IFormatParser formatParser, DownlinkMatchConventions conventions)
        {
            _parser = formatParser;
            _forceNameMatch = conventions.ForceNameMatching;
        }

        public bool Match(Path path, VersionSpec version)
        {
            var search = $"{version.ToString()}/{version.Platform}/";
            var versionMatch = path.FullPath.StartsWith(search);
            var archMatch = path.GetFilenameWithoutExtension().Contains(version.Architecture);
            var nameMatch = _forceNameMatch ? path.GetFilenameWithoutExtension().Contains(version.ToString()) : true;
            var formatMatch = string.IsNullOrWhiteSpace(version.Format)
                ? true
                : _parser.GetFormat(path).ToLower() == version.Format.ToLower();
            return versionMatch && archMatch && nameMatch && formatMatch;
        }
    }
}