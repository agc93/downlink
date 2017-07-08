using System;
using Downlink.Composition;
using Downlink.Hosting;
using Downlink.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Downlink.Composition
{
    public interface IPluginLoader {
        //void LoadPlugin<T>(IDownlinkBuilder builder) where T : class, IDownlinkPlugin, new();
        void LoadPlugins(IDownlinkBuilder builder, IServiceProvider provider);
    }
}