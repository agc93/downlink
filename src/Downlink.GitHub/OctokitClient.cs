using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Octokit;

namespace Downlink.GitHub
{
    public class OctokitClient : IGitHubClient
    {

        public OctokitClient(
            GitHubCredentials credentials,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            Func<string, VersionSpec> patternMatcher = null)
        {
            PatternMatcher = patternMatcher ?? ParseSpec;
            Credentials = credentials;
            Client = new GitHubClient(new ProductHeaderValue("Downlink"));
        }

        private GitHubCredentials Credentials { get; }
        public GitHubClient Client { get; private set; }

        public Func<string, VersionSpec> PatternMatcher { get; }

        private async Task<Repository> GetRepo()
        {
            var repo = await Client.Repository.Get(Credentials.Owner, Credentials.Repo);
            return repo;
        }

        public async Task<IEnumerable<VersionSpec>> GetAllVersions()
        {
            var releases = await Client.Repository.Release.GetAll(Credentials.Owner, Credentials.Repo);
            var assetList = releases.ToDictionary(
                k => k.TagName,
                v => v.Assets.Select(a => PatternMatcher(a.Name)));

            return releases.Select(r => new VersionSpec(r.TagName, string.Empty, string.Empty));
        }

        public async Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            var releases = await Client.Repository.Release.GetAll(Credentials.Owner, Credentials.Repo);
            var release = releases.FirstOrDefault(r => r.TagName == version);
            if (release == null) throw new VersionNotFoundException($"Could not find version '{version}'");
            var opts = release.Assets.ToDictionary(a => PatternMatcher(a.Name), a => a);
            var platforms = opts.Where(v => v.Key.Platform == version.Platform);
            if (!platforms.Any()) throw new PlatformNotFoundException($"Could not find platform '{version.Platform}' for version '{version}'");
            var archs = platforms.Where(a => a.Key.Architecture == version.Architecture);
            if (!archs.Any()) throw new ArchitectureNotFoundException($"Could not find requested architecture '{version.Architecture}' for version '{version}'");
            var file = new GitHubFileSource(archs.First().Key, !release.Draft);
            file.Build(archs.First().Value);
            return file;
        }

        private VersionSpec ParseSpec(string s)
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
