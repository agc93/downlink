using System.IO;

namespace Downlink.Core
{
    public interface IStreamingStorage
    {
        IFileSource GetFile(VersionSpec version);
        Stream GetFileContents(VersionSpec version);
    }
}
