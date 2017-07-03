using Downlink.Core.Diagnostics;

namespace Downlink.Core.Runtime
{
    internal static class SpecParser
    {
        internal static VersionSpec Parse(string s)
        {
            s = System.IO.Path.GetFileNameWithoutExtension(s);
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