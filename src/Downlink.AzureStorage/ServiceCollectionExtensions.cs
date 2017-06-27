using Microsoft.Extensions.DependencyInjection;
using Downlink.Core;

namespace Downlink.AzureStorage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureStorage(this IServiceCollection services) {
            services.AddSingleton<IRemoteStorage, AzureStorage>();
            return services;
        }
    }
}