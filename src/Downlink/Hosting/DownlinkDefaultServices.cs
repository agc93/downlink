using System;
using Downlink.AzureStorage;
using Downlink.Composition;
using Downlink.GitHub;
using Downlink.Handlers;
using Downlink.Infrastructure;
using Downlink.Local;
using Downlink.S3;
using Downlink.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting
{
    public class DownlinkDefaultServices : IDownlinkPlugin
    {
        public void AddServices(IDownlinkBuilder builder)
        {
            var opts = builder.Services.BuildServiceProvider()?.GetService<DownlinkBuilderDefaults>()
                ?? new DownlinkBuilderDefaults(DownlinkBuilderOptions.None);
            if (opts.RegisterDefaultHandlers) {
                builder.Services.AddSingleton<ProxyingResponseHandler>();
                builder.Services.AddSingleton<RedirectingResponseHandler>();
                builder.Services.AddSingleton<IResponseHandler>(ServiceFactory.GetResponseHandler);
            }
            if (opts.RegisterDefaultPatterns) {
                builder.Services.AddDefaultPatternMatchers();
            }
            if (opts.RegisterDefaultSchemeClients) {
                builder.Services.AddTransient<ISchemeClient, HttpDownloadClient>();
                builder.Services.AddTransient<ISchemeClient, FileSchemeClient>();
            }
            if (opts.RegisterDefaultStorage) {
                builder.Services.AddGitHubReleaseStorage();
                builder.Services.AddS3Storage();
                builder.Services.AddAzureStorage();
                builder.Services.AddLocalStorage();
                builder.Services.AddFallbackStorage();
            }
        }
    }
}