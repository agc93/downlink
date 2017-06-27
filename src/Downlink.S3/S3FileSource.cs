using System;
using Downlink.Core;

namespace Downlink.S3
{
    public class S3FileSource : IFileSource
    {
        internal S3FileSource(Uri fileUri) {
            FileUri = fileUri;
        }
        public Uri FileUri { get; }

        public VersionSpec Version { get; set; }

        public bool Available => true;

        public FileMetadata Metadata { get; internal set; }
    }
}
