using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Downlink.Core;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Downlink.Messaging
{
    public class AppVersionRequestHandler : IAsyncRequestHandler<AppVersionRequest, AppVersionResponseModel>
    {
        private readonly ILogger<AppVersionRequestHandler> _logger;

        public AppVersionRequestHandler(
            IEnumerable<IRemoteStorage> storage,
            IConfiguration config,
            ILogger<AppVersionRequestHandler> logger) {
            AvailableProviders = storage;
            Configuration = config;
            _logger = logger;
        }

        private IConfiguration Configuration { get; }
        private IEnumerable<IRemoteStorage> AvailableProviders { get; }

        public async Task<AppVersionResponseModel> Handle(AppVersionRequest message)
        {
            _logger?.LogInformation($"Handling request for: {message.Version.Summary}");
            var backend = Configuration.GetValue("Storage", string.Empty).ToLower().Trim();
            var storage = AvailableProviders.GetStorageFor(backend);
            var source = await storage.GetFileAsync(message.Version);
            return new AppVersionResponseModel(source);
        }
    }
}