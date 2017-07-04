using Downlink.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Local
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalStorage(this IServiceCollection services) {
            services.AddSingleton<IRemoteStorage, LocalFileStorage>();
            return services; 
        }        
    }
}
