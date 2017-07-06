using System.IO;
using System.Linq;
using System.Reflection;
using Downlink.Composition;
using Downlink.Hosting;
using Downlink.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Downlink
{
    public static class BuilderExtensions
    {
        public static IWebHostBuilder ConfigureLogging(this IWebHostBuilder builder) =>
            builder.ConfigureLogging((ctx, factory) =>
            {
                factory.AddConfiguration(ctx.Configuration.GetSection("Logging"));
                factory.AddConsole();
                factory.AddDebug();
            });

        internal static IWebHostBuilder ConfigureAppConfiguration(this IWebHostBuilder builder, string[] args) =>
            builder.ConfigureAppConfiguration((ctx, config) =>
            {
                var env = ctx.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                      .AddConfigFile("config");

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });

        private static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder config, string fileName)
        {
            fileName = fileName.Replace(".json", string.Empty).Replace(".yml", string.Empty);
            config.AddJsonFile($"{fileName}.json", optional: true, reloadOnChange: true);
            config.AddYamlFile($"{fileName}.yml", optional: true, reloadOnChange: true);
            return config;
        }

        internal static IDownlinkBuilder AddLocalPlugins(this IDownlinkBuilder builder, bool forceEnable = false)
        {
            if (forceEnable)
            {
                try
                {
                    var scanner = new PluginScanner();
                    var root = new DirectoryInfo(Directory.GetCurrentDirectory());
                    var dlls = root.GetFiles("Downlink.Extensions.*.dll");
                    var assemblies = dlls.Select(f => Assembly.LoadFile(f.FullName));
                    var modules = scanner.LoadModulesFromAssemblies(assemblies);
                    foreach (var plugin in modules)
                    {
                        builder.AddPlugin(plugin);
                    }
                }
                catch (System.Exception)
                {
                    // ignored
                }
            }
            return builder;
        }

        internal static IDownlinkBuilder AddLocalPlugins(this IDownlinkBuilder builder, IConfiguration config) {
            var enable = config.GetValue("Experimental:EnableLocalPlugins", false);
            return builder.AddLocalPlugins(enable);
        }
    }
}
