using System;

namespace Downlink.Core
{
    public interface IFileSource
    {
        Uri FileUri { get; }
        VersionSpec Version { get; }
        bool Available { get; }

        FileMetadata Metadata { get; }
    }
}
