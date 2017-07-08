using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Downlink.Core;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Downlink.Messaging
{
    public class AppVersionRequestHandler : IAsyncRequestHandler<AppVersionRequest, AppVersionResponseModel>
    {
        public AppVersionRequestHandler(
            IEnumerable<IRemoteStorage> storage,
            IConfiguration config) {
            AvailableProviders = storage;
            Configuration = config;
        }

        private IConfiguration Configuration { get; }
        private IEnumerable<IRemoteStorage> AvailableProviders { get; }

        public async Task<AppVersionResponseModel> Handle(AppVersionRequest message)
        {
            var backend = Configuration.GetValue("Storage", string.Empty).ToLower().Trim();
            var storage = AvailableProviders.GetStorageFor(backend);
            var source = await storage.GetFileAsync(message.Version);
            return new AppVersionResponseModel(source);
        }
    }
}