using System.Threading.Tasks;

namespace Downlink.Core
{
    public interface IRemoteStorage {
        Task<IFileSource> GetFileAsync(VersionSpec version);
    }

    public interface IRemoteStorage<TBuilder> {

    }
}
