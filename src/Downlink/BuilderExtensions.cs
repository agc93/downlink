using System.Reflection;
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
                factory.UseConfiguration(ctx.Configuration.GetSection("Logging"));
                factory.AddConsole();
                factory.AddDebug();
            });

        public static IWebHostBuilder ConfigureAppConfiguration(this IWebHostBuilder builder, string[] args) =>
            builder.ConfigureAppConfiguration((ctx, config) =>
            {
                var env = ctx.HostingEnvironment;

                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                      .AddJsonFile("downlink.json", optional: true, reloadOnChange: true)
                      .AddYamlFile("downlink.yml", optional: true, reloadOnChange: true);
                      //.AddYamlFile("/etc/downlink/config.yml", optional: true, reloadOnChange: true);

                if (env.IsDevelopment())
                {
                    var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                    if (appAssembly != null)
                    {
                        config.AddUserSecrets(appAssembly, optional: true);
                    }
                }

                config.AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            });
    }
}
