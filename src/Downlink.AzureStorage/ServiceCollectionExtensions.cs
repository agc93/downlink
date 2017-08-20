using Microsoft.Extensions.DependencyInjection;
using Downlink.Core;
using Downlink.AzureStorage.Strategies;

namespace Downlink.AzureStorage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureStorage(this IServiceCollection services) {
            services.AddSingleton<AzureStorage>();
            services.AddSingleton<IRemoteStorage>(provider => provider.GetService<AzureStorage>());
            return services;
        }
    }
}