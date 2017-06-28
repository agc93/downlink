using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Downlink.Core;
using Octokit;

namespace Downlink.GitHub
{
    public abstract class GitHubMatchStrategy : IMatchStrategy<Octokit.Release>
    {
        protected GitHubMatchStrategy(string name)
        {
            Name = name;
        }
        public string Name { get; }

        public abstract Task<IFileSource> MatchAsync(IEnumerable<Release> items, VersionSpec version);
    }
}
