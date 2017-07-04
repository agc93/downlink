using System;
using Downlink.Core;
using Downlink.Core.Runtime;
using Downlink.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting
{
    public class DownlinkBuilder : IDownlinkBuilder
    {
        internal DownlinkBuilder(IServiceCollection services)
        {
            Services = services;
        }
        public IServiceCollection Services { get; }

        internal static void AddDefaultServices(IDownlinkBuilder builder, DownlinkBuilderOptions opts)
        {
            if (!opts.HasFlag(DownlinkBuilderOptions.SkipDefaultHandlers))
            {
                builder.Services.AddSingleton<ProxyingResponseHandler>();
                builder.Services.AddSingleton<RedirectingResponseHandler>();
                builder.Services.AddSingleton<IResponseHandler>(Downlink.Hosting.Handlers.ServiceFactory.GetResponseHandler);
            }
            if (!opts.HasFlag(DownlinkBuilderOptions.SkipDefaultPatterns))
            {
                builder.Services.AddDefaultPatternMatchers();
            }
            if (!opts.HasFlag(DownlinkBuilderOptions.SkipDefaultSchemeClients))
            {
                builder.Services.AddTransient<ISchemeClient, HttpDownloadClient>();
                builder.Services.AddTransient<ISchemeClient, FileSchemeClient>();
            }
        }
    }
}
