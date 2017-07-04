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

        internal static IRemoteStorage GetStorage(System.IServiceProvider provider) {
            var logger = provider.GetService<ILogger<Hosting.DownlinkBuilder>>();
            logger?.LogInformation("Running service factory for IRemoteStorage");
            var config = provider.GetService<IConfiguration>();
            var backend = config.GetValue("Storage", string.Empty).ToLower().Trim();
            switch (backend)
            {
                case "local":
                case "localstorage":
                    return provider.GetService<Local.LocalFileStorage>();
                case "github":
                    return provider.GetService<GitHub.IGitHubClient>();
                case "azure":
                case "azurestorage":
                    return provider.GetService<AzureStorage.AzureStorage>();
                case "aws":
                case "s3":
                    return provider.GetService<S3.S3Storage>();
                default:
                    return provider.GetService<Storage.NoneStorage>();
            }
        }
    }
}
