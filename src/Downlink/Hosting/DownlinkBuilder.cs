using System;
using Downlink.Composition;
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

        public void Build() {
            var provider = Services.BuildServiceProvider();
            var loader = provider.GetService<IPluginLoader>();
            loader.LoadPlugins(this, provider);
        }
    }
}
