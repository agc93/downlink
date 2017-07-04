using System.Threading.Tasks;

namespace Downlink.Core
{
    public interface IRemoteStorage {
        string Name {get;}
        Task<IFileSource> GetFileAsync(VersionSpec version);
    }
}
