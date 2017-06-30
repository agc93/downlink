using Microsoft.Extensions.DependencyInjection;
using Downlink.Core;
using Downlink.AzureStorage.Strategies;

namespace Downlink.AzureStorage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAzureStorage(this IServiceCollection services) {
            services.AddSingleton<IRemoteStorage, AzureStorage>();
            return services;
        }

        public static IServiceCollection AddAzureMatchStrategies(this IServiceCollection services) {
            //services.AddTransient<AzureMatchStrategy, HierarchicalMatchStrategy>();
            return services;
        }
    }
}