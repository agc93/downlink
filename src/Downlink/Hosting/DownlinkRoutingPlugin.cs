using System;
using System.Linq;
using Downlink.Composition;
using Downlink.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Downlink.Hosting
{
    public class DownlinkRoutingPlugin : IDownlinkPlugin
    {
        private readonly ILogger<DownlinkRoutingPlugin> _logger;

        public DownlinkRoutingPlugin(ILogger<DownlinkRoutingPlugin> logger) {
            _logger = logger;
        }
        public void AddServices(IDownlinkBuilder builder)
        {
            if (!builder.Services.Any(s => s.ServiceType == typeof(IRoutePrefixBuilder))) {
                _logger?.LogInformation("No route provider found, falling back to configuration!");
                // such a hack job
                builder.Services.AddSingleton<IRoutePrefixBuilder, ConfigurationRoutePrefixBuilder>();
            }
            if (!builder.Services.Any(s => s.ServiceType == typeof(IDownlinkRouteConvention))) {
                _logger?.LogDebug("No route convention provider found, falling back to default");
                builder.Services.AddSingleton<IDownlinkRouteConvention, DownlinkRouteConvention>();
            }
            builder.Services.Configure<MvcOptions>(opts => opts.Conventions.Add(builder.Services.BuildServiceProvider().GetService<IDownlinkRouteConvention>()));
        }
    }
}