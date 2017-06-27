using System;
using System.IO;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace Downlink.Storage
{
    public class LocalFileStorage : IRemoteStorage
    {

        public LocalFileStorage(IConfiguration configuration) {
            var path = configuration.GetValue(
                "LocalStorage:PackageRoot",
                Path.Combine(Directory.GetCurrentDirectory(), "Packages")
            );
            PackageRoot = new DirectoryInfo(path);
        }

        private DirectoryInfo PackageRoot {get;}
        public Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            var path = Path.Combine(
                PackageRoot.FullName,
                version,
                version.Platform,
                version.Architecture
            );
            if (!File.Exists(path)) {
                throw new VersionNotFoundException();
            }
            return Task.FromResult(new LocalFileSource(version, new FileInfo(path)) as IFileSource);
        }
    }
}