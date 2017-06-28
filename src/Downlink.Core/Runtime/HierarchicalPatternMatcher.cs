using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core.Diagnostics;
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
            var matching = path.FullPath.StartsWith(search);
            var nameMatch = _forceNameMatch ? path.GetFilename().Contains(version) : true;
            return matching && nameMatch;
        }
    }

    public class FlatPatternMatcher : IPatternMatcher
    {
        public string Name => "Flat";

        public bool Match(Path path, VersionSpec version)
        {
            var name = path.GetFilename();
            var nameMatch = name.Contains(version.Architecture) &&
                name.Contains(version.Platform) &&
                name.Contains(version.ToString());
            
            //this is a bit heavy-handed since other backends likely won't adopt the GitHub conventions..
            var segMatch = ParseSpec(name).Equals(version);
            return nameMatch && segMatch;
        }
        internal static VersionSpec ParseSpec(string s)
        {
            var parts = s.Split('_');
            if (parts.Length == 1)
            {
                throw new VersionParseException($"Failed to parse version from '{s}'");
            }
            switch (parts.Length)
            {
                case 2:
                    return new VersionSpec(parts[1], null, null);
                case 3:
                    return new VersionSpec(parts[1], parts[2], null);
                case 4:
                    return new VersionSpec(parts[1], parts[2], parts[3]);
                default:
                    return new VersionSpec(s, null, null);
            }
        }


    }
}
