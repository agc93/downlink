using System;
using System.IO;
using Downlink.Core;

namespace Downlink.Local
{
    public class LocalFileSource : IFileSource
    {
        public LocalFileSource(VersionSpec version, FileInfo fileInfo) {
            var builder = new UriBuilder() {
                Host = string.Empty,
                Scheme = "file",
                Path = fileInfo.FullName
            };
            FileUri = builder.Uri;
            Metadata = new FileMetadata(fileInfo.Length, fileInfo.Name) { Public = false };
        }
        public Uri FileUri { get; private set; }

        public VersionSpec Version { get; private set; }

        public bool Available { get; private set; }

        public FileMetadata Metadata { get; set; }
    }
}