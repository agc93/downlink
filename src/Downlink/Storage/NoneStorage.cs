using System;
using System.Threading.Tasks;
using Downlink.Core;
using Downlink.Core.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Downlink.Storage
{
    public class NoneStorage : IRemoteStorage
    {
        private ILogger<NoneStorage> _logger;

        public NoneStorage(ILogger<NoneStorage> logger) {
            _logger = logger;
        }
        public Task<IFileSource> GetFileAsync(VersionSpec version)
        {
            var message = "There is no storage backend currently configured! Use the settings file or environment variables to enable a remote backend.";
            _logger.LogWarning(404, message);
            throw new VersionNotFoundException(message);
        }
    }
}