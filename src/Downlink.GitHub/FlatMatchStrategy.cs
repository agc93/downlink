using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Octokit;

namespace Downlink.GitHub
{
    public class FlatMatchStrategy : GitHubMatchStrategy
    {
        private readonly List<string> _splitCharacters;
        private readonly ILogger<FlatMatchStrategy> _logger;
        private readonly IFormatParser _formatParser;
        private readonly bool _enableLatestTag;

        public FlatMatchStrategy(
            IConfiguration config,
            ILogger<FlatMatchStrategy> logger,
            IFormatParser formatParser
        ) : base("flat")
        {
            _enableLatestTag = config.GetValue("Experimental:GitHubLatestVersion", false);
            _splitCharacters = config.GetList("GitHubStorage:SplitCharacters", new[] { "_" }).ToList();
            _logger = logger;
            _formatParser = formatParser;
            _logger.LogDebug("Parsing releases using {0}", string.Join("|", _splitCharacters));
        }

        public override Task<IFileSource> MatchAsync(IEnumerable<Release> releases, VersionSpec version)
        {
            var release = _enableLatestTag
                ? releases
                    .Where(r => !r.Draft)
                    .Where(r => !r.Prerelease)
                    .OrderByDescending(r => r.PublishedAt)
                    .FirstOrDefault()
                : releases.FirstOrDefault(r => r.TagName == version)
                    ?? releases.FirstOrDefault(r => r.Name == version);
            if (release == null) throw new VersionNotFoundException($"Could not find release for version '{version}'");
            _logger.LogDebug("Found release {0} with {1} assets", release.Name, release.Assets.Count);
            var opts = release.Assets.ToDictionary(a => ParseSpec(a.Name, _splitCharacters.ToArray()), a => a);
            _logger.LogDebug("Found releases: {0}", opts.Keys.Select(k => k.Summary));
            var platforms = opts.Where(v => v.Key.Platform.ToLower() == version.Platform.ToLower()).ToList();
            if (!platforms.Any()) throw new PlatformNotFoundException($"Could not find platform '{version.Platform}' for version '{version}'");
            _logger.LogDebug("Found platform match for {0} releases: {1}", platforms.Count, platforms.Select(p => p.Key.Summary));
            var archs = platforms.Where(a => a.Key.Architecture.ToLower() == version.Architecture.ToLower()).ToList();
            if (!archs.Any()) throw new ArchitectureNotFoundException($"Could not find requested architecture '{version.Architecture}' for version '{version}'");
            _logger.LogDebug("Found architecture match for {0} assets: {1}", archs.Count, archs.Select(a => a.Key.Summary));
            var match = string.IsNullOrWhiteSpace(version.Format)
                ? archs.First()
                : archs.First(r => (_formatParser.GetFormat(r.Value.Name).ToLower() == version.Format));
            if (match.Value == null) throw new FormatNotFoundException($"Could not find requested format '{version.Format}' for version '{version}'!");
            // var file = new GitHubFileSource(archs.First().Key, !release.Draft);
            var file = new GitHubFileSource(match.Key, !release.Draft);
            file.Build(match.Value);
            return Task.FromResult(file as IFileSource);
        }

        private static VersionSpec ParseSpec(string s, string[] chars)
        {
            s = System.IO.Path.GetFileNameWithoutExtension(s);
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

        internal static VersionSpec ParseSpec(string s)
        {
            return ParseSpec(s, new[] { "_" });
        }

    }
}
