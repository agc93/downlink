using Downlink.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Storage
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFallbackStorage(this IServiceCollection services) {
            services.AddSingleton<IRemoteStorage, NoneStorage>();
            return services;
        }
        
    }
}