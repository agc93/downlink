using System;
using Downlink.Core;
using Downlink.Core.Runtime;
using Downlink.GitHub;
using Downlink.Handlers;
using Downlink.Infrastructure;
using Downlink.S3;
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
                builder.Services.AddSingleton<IResponseHandler>(ServiceFactory.GetResponseHandler);
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
            if (!opts.HasFlag(DownlinkBuilderOptions.SkipDefaultStorage)) {
                builder.Services.AddGitHubReleaseStorage();
                builder.Services.AddS3Storage();
                builder.Services.AddSingleton<AzureStorage.AzureStorage>();
                builder.Services.AddSingleton<S3.S3Storage>();
                builder.Services.AddSingleton<Local.LocalFileStorage>();
                builder.Services.AddSingleton<Storage.NoneStorage>();
                //builder.Services.AddSingleton<IRemoteStorage>(p => ServiceFactory.GetStorage(p));
            }
        }
    }
}
