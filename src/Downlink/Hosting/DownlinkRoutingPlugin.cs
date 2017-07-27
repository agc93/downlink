using System;
using Downlink.Composition;
using Downlink.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting
{
    public class DownlinkRoutingPlugin : IDownlinkPlugin
    {
        public void AddServices(IDownlinkBuilder builder)
        {
            builder.Services.AddSingleton<DownlinkRouteConvention>();
            builder.Services.Configure<MvcOptions>(opts => opts.Conventions.Add(builder.Services.BuildServiceProvider().GetService<DownlinkRouteConvention>()));
        }
    }
}