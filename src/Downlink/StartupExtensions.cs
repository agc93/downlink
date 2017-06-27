using Downlink.AzureStorage;
using Downlink.GitHub;
using Downlink.Handlers;
using Downlink.Storage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMvcServices(this IServiceCollection services)
        {
            services.AddMvcCore(opts =>
            {
                opts.AddFormatterMappings();
            })
            .AddJsonOptions(j =>
            {
                j.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            })
            .AddJsonFormatters()
            .AddXmlSerializerFormatters()
            .AddApiExplorer()
            .AddDataAnnotations();
            return services;
        }

        public static MvcOptions AddFormatterMappings(this MvcOptions opts)
        {
            opts.FormatterMappings.SetMediaTypeMappingForFormat("xml", "application/xml");
            opts.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
            return opts;
        }

        public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
        {
            app.UseCors(c =>
                c.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin()
                .AllowCredentials());
            return app;
        }

        public static IServiceCollection AddMediatR(this IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup), typeof(Core.IRemoteStorage));
            return services;
        }

        public static IServiceCollection AddStorageBackend(this IServiceCollection services, IConfiguration config)
        {
            var backend = config.GetValue("Storage", "LocalStorage").ToLower().Trim();
            switch (backend)
            {
                case "localstorage":
                    services.AddLocalStorage();
                    break;
                case "github":
                    services.AddGitHubReleaseStorage(config);
                    break;
                case "azure":
                case "azurestorage":
                    services.AddAzureStorage();
                    break;
                default:
                    services.AddFallbackStorage();
                    break;
            }
            return services;
        }

        internal static IServiceCollection AddDownlinkServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<ISchemeClient, FileSchemeClient>();
            services.AddTransient<ISchemeClient, HttpSchemeClient>();
            services.AddResponseHandler(config);
            return services;
        }

        private static void AddResponseHandler(this IServiceCollection services, IConfiguration config)
        {
            var proxying = config.GetValue<bool>("ProxyRemoteFiles", false);
            if (proxying)
            {
                services.AddSingleton<IResponseHandler, ProxyingResponseHandler>();
            }
            else
            {
                services.AddSingleton<IResponseHandler, RedirectingResponseHandler>();
            }
        }
    }
}
