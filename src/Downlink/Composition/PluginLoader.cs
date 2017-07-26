using System;
using System.Linq;
using Downlink.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Downlink.Composition
{
    public class PluginLoader : IPluginLoader
    {
        private readonly ILogger<PluginLoader> _logger;

        public PluginLoader(
            ILogger<PluginLoader> logger
        )
        {
            _logger = logger;
        }

        public void LoadPlugins(IDownlinkBuilder builder, IServiceProvider provider)
        {
            var plugins = provider.GetServices<IDownlinkPlugin>();
            _logger.LogDebug("Resolved {0} plugins from provider.", plugins.Count());
            foreach (var plugin in plugins)
            {
                var name = plugin.GetType().Name;
                try
                {
                    _logger.LogDebug($"Registering '{name}' plugin");
                    plugin.AddServices(builder);
                    _logger.LogInformation($"Successfully loaded '{name}' plugin!");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Plugin {name} failed to register!");
                    _logger.LogError(500, ex, ex.Message);
                }
            }
        }
    }
}