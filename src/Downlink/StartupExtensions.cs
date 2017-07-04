using Downlink.AzureStorage;
using Downlink.Core;
using Downlink.Core.Runtime;
using Downlink.GitHub;
using Downlink.Handlers;
using Downlink.Local;
using Downlink.S3;
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
        public static IMvcCoreBuilder AddMvcServices(this IServiceCollection services)
        {
            return services.AddMvcCore(opts =>
            {
                opts.AddFormatterMappings();
            })
            .AddJsonOptions(j =>
            {
                j.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            })
            .AddJsonFormatters()
            .AddApiExplorer()
            .AddDataAnnotations();
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
            services.AddMediatR(typeof(Hosting.DownlinkBuilder), typeof(Core.IRemoteStorage));
            return services;
        }
    }
}
