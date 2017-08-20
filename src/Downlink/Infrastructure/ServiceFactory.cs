using System.Collections.Generic;
using Downlink.Core;
using Downlink.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Downlink.Infrastructure
{
    public static class ServiceFactory
    {
        internal static IResponseHandler GetResponseHandler(System.IServiceProvider provider)
        {
            var config = provider.GetService<IConfiguration>();
            var proxying = config.GetValue<bool>("ProxyRemoteFiles", false);
            if (proxying)
            {
                return provider.GetService<ProxyingResponseHandler>();
            }
            else
            {
                return provider.GetService<RedirectingResponseHandler>();
            }
        }

        // this should be removed in a future release
        internal static IRemoteStorage GetStorage(System.IServiceProvider provider) {
            var logger = provider.GetService<ILogger<Hosting.DownlinkBuilder>>();
            logger?.LogDebug("Running service factory for IRemoteStorage");
            var config = provider.GetService<IConfiguration>();
            var backend = config.GetValue("Storage", string.Empty).ToLower().Trim();
            var providers = provider.GetServices<IRemoteStorage>();
            return providers.GetStorageFor(backend);
        }
    }
}
