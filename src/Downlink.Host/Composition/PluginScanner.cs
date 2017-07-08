using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Downlink.Composition;
using Downlink.Infrastructure;
using Downlink.Core;
using Downlink.Handlers;

namespace Downlink.Composition
{
    public class PluginScanner : IPluginScanner
    {

        public PluginScanner() {
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
}