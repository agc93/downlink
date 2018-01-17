using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class HierarchicalPatternMatcher : IPatternMatcher
    {
        private readonly IFormatParser _parser;
        private readonly bool _forceNameMatch;

        public string Name => "Hierarchical";

        public HierarchicalPatternMatcher(IFormatParser parser, DownlinkMatchConventions conventions)
        {
            _parser = parser;
            _forceNameMatch = conventions.ForceNameMatching;
        }

        public bool Match(Path path, VersionSpec version)
        {
            var search = $"{version.ToString()}/{version.Platform}/{version.Architecture}";
            var matching = path.FullPath.StartsWith(search) ||
                path.FullPath.TrimStart('.', '/').StartsWith(search);
            if (!matching && version.Architecture == "any") {
                matching = path.FullPath.TrimStart('.', '/').StartsWith($"{version}/{version.Platform}/");
            }
            if (!matching && version.Architecture == "any" && version.Platform == "any") {
                matching = path.FullPath.TrimStart('.', '/').StartsWith($"{version}/");
            }
            var nameMatch = _forceNameMatch ? path.GetFilename().Contains(version) : true;
            var formatMatch = string.IsNullOrWhiteSpace(version.Format)
                ? true
                : _parser.GetFormat(path).ToLower() == version.Format.ToLower();
            return matching && nameMatch && formatMatch;
        }
    }
}
