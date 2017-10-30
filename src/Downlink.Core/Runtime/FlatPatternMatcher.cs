using Downlink.Core.Diagnostics;
using Downlink.Core.IO;

namespace Downlink.Core.Runtime
{
    public class FlatPatternMatcher : IPatternMatcher
    {
        public string Name => "Flat";

        public bool Match(Path path, VersionSpec version)
        {
            var name = path.GetFilename();
            var nameMatch = name.Contains(version.Architecture) &&
                name.Contains(version.Platform) &&
                name.Contains(version.ToString()) &&
                name.EndsWith(version.Format ?? string.Empty);

            //this is a bit heavy-handed since other backends likely won't adopt the GitHub conventions..
            var segMatch = SpecParser.Parse(name).Equals(version);
            return nameMatch && segMatch;
        }
    }
}
