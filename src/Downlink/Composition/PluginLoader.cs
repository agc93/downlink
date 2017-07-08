using System;
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

        //this is scar tissue in the making right here vvv
        private void LoadPlugin<T>(IDownlinkBuilder builder) where T : class, IDownlinkPlugin, new()
        {
            var module = new T();
            try
            {
                module.AddServices(builder);
            }
            catch (System.Exception)
            {
                _logger.LogWarning($"Module {module.GetType().Name} failed to register!");
                // ignored for now
            }
        }

        public void LoadPlugins(IDownlinkBuilder builder, IServiceProvider provider)
        {
            var plugins = provider.GetServices<IDownlinkPlugin>();
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