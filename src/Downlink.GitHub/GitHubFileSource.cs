using System;
using Downlink.Core;
using Octokit;

namespace Downlink.GitHub
{
    public class GitHubFileSource : IFileSource
    {
        internal GitHubFileSource()
        {

        }

        internal GitHubFileSource(VersionSpec version, bool available = true)
        {
            Version = version;
            Available = available;
        }

        internal void Build(ReleaseAsset asset)
        {
            FileUri = new Uri(asset.BrowserDownloadUrl);
            Metadata = new FileMetadata(asset.Size, asset.Name);
        }
        public Uri FileUri { get; private set; }

        public VersionSpec Version { get; set; }

        public bool Available { get; set; }

        public FileMetadata Metadata { get; private set; }
    }
}
