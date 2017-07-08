using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Downlink.Hosting
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder ConfigureDownlink(this IWebHostBuilder builder) =>
            builder.ConfigureAppConfiguration((ctx, config) => 
            {
                config.AddConfigFile("downlink")
                    .AddConfigFile("./config/downlink")
                    .AddEnvironmentVariables("DOWNLINK__")
                    .AddEnvironmentVariables("DOWNLINK:")
                    .AddEnvironmentVariables("DOWNLINK_");
            });
    }
}