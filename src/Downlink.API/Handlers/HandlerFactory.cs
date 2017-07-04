using Downlink.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting.Handlers
{
    public static class ServiceFactory
    {
        public static IResponseHandler GetResponseHandler(System.IServiceProvider provider) {
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
    }
}