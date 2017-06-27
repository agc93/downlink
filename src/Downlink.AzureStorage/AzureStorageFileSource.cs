using System;
using System.Threading.Tasks;
using Downlink.Core;

namespace Downlink.AzureStorage
{
    public class AzureStorageFileSource : IFileSource
    {
        internal AzureStorageFileSource(Uri fileUri) {
            FileUri = fileUri;
        }
        public Uri FileUri  {get;}

        public VersionSpec Version {get;set;}

        public bool Available => true;

        public FileMetadata Metadata {get; internal set;}
    }
}