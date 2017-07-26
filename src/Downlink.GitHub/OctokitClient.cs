using System;
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
    public class OctokitClient : IGitHubClient
    {
        private readonly ILogger<OctokitClient> _logger;

        public string Name => "GitHub";

        public OctokitClient(
            GitHubCredentials credentials,
            IConfiguration configuration,
            ILogger<OctokitClient> logger,
            IEnumerable<GitHubMatchStrategy> matchStrategies,
            IEnumerable<IPatternMatcher> patternMatchers)
        {
            _logger = logger;
            PatternMatcher = patternMatchers.GetFor("flat")
                ?? new Core.Runtime.FlatPatternMatcher();
            Credentials = credentials;
            var strategy = configuration.GetValue<string>("GitHubStorage:MatchStrategy", "Flat");
            MatchStrategy = matchStrategies.GetFor<GitHubMatchStrategy, Release>(strategy);
            var server = configuration.GetValue<string>("GitHubStorage:ServerUrl", string.Empty);
            Client = string.IsNullOrWhiteSpace(server)
                ? new GitHubClient(Header)
                : new GitHubClient(Header, new Uri(server));
        }

        private ProductHeaderValue Header => new ProductHeaderValue("Downlink");

        private GitHubCredentials Credentials { get; }
        public GitHubClient Client { get; private set; }
        private GitHubMatchStrategy MatchStrategy {get;}
        private IPatternMatcher PatternMatcher {get;}

        public async Task<IEnumerable<VersionSpec>> GetAllVersions()
        {
            var releases = await Client.Repository.Release.GetAll(Credentials.Owner, Credentials.Repo);
            var assetList = releases.ToDictionary(
                k => k.TagName,
                v => v.Assets.Select(a => FlatMatchStrategy.ParseSpec(a.Name)));

            return releases.Select(r => new VersionSpec(r.TagName, string.Empty, string.Empty));
        }

        public async Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            if (Credentials == null) {
                _logger.LogCritical("Repo details not found! Please add GitHubStorage to your app configuration and restart the server.");
                throw new UnauthorizedAccessException("Repository details not found in configuration!");
            }
            var releases = await Client.Repository.Release.GetAll(Credentials.Owner, Credentials.Repo);
            if (MatchStrategy == null) {
                foreach (var release in releases)
                {
                    var asset = release.Assets.FirstOrDefault(a => PatternMatcher.Match(a.Name, version));
                    if (asset != null) {
                        var source = new GitHubFileSource(version, !release.Draft);
                        source.Build(asset);
                        return source;
                    }
                }
                throw new VersionNotFoundException();
            }
            return await MatchStrategy.MatchAsync(releases, version);
        }
    }
}
