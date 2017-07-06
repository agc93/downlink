using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Downlink
{
    public static class BuilderExtensions
    {
        internal static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder config, string fileName)
        {
            fileName = fileName.Replace(".json", string.Empty).Replace(".yml", string.Empty);
            config.AddJsonFile($"{fileName}.json", optional: true, reloadOnChange: true);
            config.AddYamlFile($"{fileName}.yml", optional: true, reloadOnChange: true);
            return config;
        }

        internal static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Hosting.DownlinkBuilder), typeof(Core.IRemoteStorage));
            return services;
        }
    }
}
