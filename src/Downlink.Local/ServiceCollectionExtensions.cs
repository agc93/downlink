using Downlink.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Local
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalStorage(this IServiceCollection services) {
            services.AddSingleton<LocalFileStorage>();
            services.AddSingleton<IRemoteStorage>(provider => provider.GetService<LocalFileStorage>());
            return services; 
        }        
    }
}
