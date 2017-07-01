using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Downlink.Storage
{
    public class LocalFileStorage : IRemoteStorage
    {
        private readonly ILogger<LocalFileStorage> _logger;
        private readonly bool _forceNameMatching;

        public LocalFileStorage(
            IConfiguration configuration,
            ILogger<LocalFileStorage> logger
            ) {
                _logger = logger;
            var path = configuration.GetValue(
                "LocalStorage:PackageRoot",
                Path.Combine(Directory.GetCurrentDirectory(), "Packages")
            );
            _forceNameMatching = configuration.GetValue("LocalStorage:ForceNameMatching", false);
            _logger.LogInformation("Starting local file storage using path '{0}'", path);
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
            if (!Directory.Exists(path)) {
                _logger.LogWarning($"Could not resolve path: '{path}'");
                throw new VersionNotFoundException(path);
            }
            var fileNames = Directory.GetFiles(path);
            if (!fileNames.Any()) {
                _logger.LogWarning("No files found in target root '{0}'", path);
                throw new VersionNotFoundException();
            }
            var files = fileNames.Select(f => new FileInfo(f));
            var file = _forceNameMatching
                ? files.FirstOrDefault(f => f.Name.Contains(version.ToString()))
                : files.FirstOrDefault();
            if (file == null) throw new VersionNotFoundException();
            return Task.FromResult(new LocalFileSource(version, file) as IFileSource);
        }
    }
}