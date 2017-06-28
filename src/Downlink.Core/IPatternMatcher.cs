using Downlink.Core.IO;

namespace Downlink.Core
{
    public interface IPatternMatcher {
        string Name {get;}
        bool Match(Path path, VersionSpec version);
    }
}