using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Downlink.Composition;
using Downlink.Core;
using Downlink.Handlers;

namespace Downlink.Infrastructure
{
    public class PluginScanner : IPluginScanner
    {
        private readonly IPluginLoader _loader;

        public PluginScanner(
            IPluginLoader loader
        ) {
            _loader = loader;
        }
        
        public IEnumerable<Type> LoadModulesFromAssemblies(IEnumerable<Assembly> assemblies) {
            var pluginTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var plugins = assembly.GetExportedTypes().Where(t => t.IsAssignableTo(typeof(IDownlinkPlugin)));
                if (plugins.Any()) {
                    pluginTypes.AddRange(plugins);
                    break;
                }
                var types = new List<Type>();
                types.AddRange(assembly.GetExportedTypes().Where(t => t.IsAssignableTo<IPatternMatcher>()));
                types.AddRange(assembly.GetExportedTypes().Where(t => t.IsAssignableTo<IRemoteStorage>()));
                types.AddRange(assembly.GetExportedTypes().Where(t => t.IsAssignableTo<IResponseHandler>()));
                types.AddRange(assembly.GetExportedTypes().Where(t => t.IsAssignableTo<ISchemeClient>()));
                pluginTypes.AddRange(types);
            }
            return pluginTypes;
        }
    }

    public interface IPluginScanner {
        IEnumerable<Type> LoadModulesFromAssemblies(IEnumerable<Assembly> assemblies);
    }
}