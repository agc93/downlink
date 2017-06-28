using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.Extensions.Configuration;
using Octokit;

namespace Downlink.GitHub
{
    public class FlatMatchStrategy : GitHubMatchStrategy
    {
        private readonly List<string> _splitCharacters;
        public FlatMatchStrategy(
            IConfiguration config
        ) : base("flat")
        {
            _splitCharacters = config.GetValue<IEnumerable<string>>("GitHubStorage:SplitCharacters", new[] {"_"}).ToList();
        }

        public override Task<IFileSource> MatchAsync(IEnumerable<Release> releases, VersionSpec version)
        {
            var release = releases.FirstOrDefault(r => r.TagName == version);
            if (release == null) throw new VersionNotFoundException($"Could not find version '{version}'");
            var opts = release.Assets.ToDictionary(a => ParseSpec(a.Name, _splitCharacters.ToArray()), a => a);
            var platforms = opts.Where(v => v.Key.Platform == version.Platform);
            if (!platforms.Any()) throw new PlatformNotFoundException($"Could not find platform '{version.Platform}' for version '{version}'");
            var archs = platforms.Where(a => a.Key.Architecture == version.Architecture);
            if (!archs.Any()) throw new ArchitectureNotFoundException($"Could not find requested architecture '{version.Architecture}' for version '{version}'");
            var file = new GitHubFileSource(archs.First().Key, !release.Draft);
            file.Build(archs.First().Value);
            return Task.FromResult(file as IFileSource);
        }

        private static VersionSpec ParseSpec(string s, string[] chars) {
	var parts = s.Split(chars, System.StringSplitOptions.RemoveEmptyEntries);
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

        internal static VersionSpec ParseSpec(string s) {
            return ParseSpec(s, new[] { "_"});
        }

    }
}
