using System;
using Downlink.Core;

namespace Downlink.GitHub
{
    public interface IGitHubClient : IRemoteStorage
    {
        Func<string, VersionSpec> PatternMatcher {get;}
    }
}
