using System;
using System.Collections.Generic;
using System.Reflection;

namespace Downlink.Composition
{
    public interface IPluginScanner {
        IEnumerable<Type> LoadModulesFromAssemblies(IEnumerable<Assembly> assemblies);
    }
}