using System;
using Downlink.AzureStorage;
using Downlink.Composition;
using Downlink.Core;
using Downlink.GitHub;
using Downlink.Handlers;
using Downlink.Infrastructure;
using Downlink.Local;
using Downlink.S3;
using Downlink.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Downlink.Hosting
{
    public class DownlinkDefaultServices : IDownlinkPlugin
    {
        public void AddServices(IDownlinkBuilder builder)
        {
            var provider = builder.Services.BuildServiceProvider();
            var opts = provider?.GetService<DownlinkBuilderDefaults>() 
                ?? new DownlinkBuilderDefaults(DownlinkBuilderOptions.None);
            var logger = provider?.GetService<ILogger<DownlinkDefaultServices>>();

            if (opts.RegisterDefaultHandlers) {
                logger?.LogDebug("Registering default handlers");
                builder.Services.AddSingleton<ProxyingResponseHandler>();
                builder.Services.AddSingleton<RedirectingResponseHandler>();
                builder.Services.AddSingleton<IResponseHandler>(ServiceFactory.GetResponseHandler);
                builder.Services.AddSingleton<IExceptionHandler, ExceptionHandler>();
            }
            if (opts.RegisterDefaultPatterns) {
                logger?.LogDebug("Registering default pattern matchers");
                builder.Services.AddDefaultPatternMatchers();
            }
            if (opts.RegisterDefaultSchemeClients) {
                logger?.LogDebug("Registering default scheme clients");
                builder.Services.AddTransient<ISchemeClient, HttpDownloadClient>();
                builder.Services.AddTransient<ISchemeClient, FileSchemeClient>();
            }
            if (opts.RegisterDefaultStorage) {
                logger?.LogDebug("Registering default storage backends");
                builder.Services.AddGitHubReleaseStorage();
                builder.Services.AddS3Storage();
                builder.Services.AddAzureStorage();
                builder.Services.AddLocalStorage();
            }
            if (opts.RegisterDefaultFormatParsers) {
                logger?.LogDebug("Registering default format parser");
                builder.Services.AddTransient<IFormatParser, PathFormatParser>();
            }
            builder.Services.AddFallbackStorage();
            builder.Services.AddSingleton<DownlinkMatchConventions>(ServiceFactory.GetConventions);
        }
    }
}