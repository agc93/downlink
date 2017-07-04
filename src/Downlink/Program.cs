using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Downlink.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Downlink
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseDefaultServiceProvider((ctx, opts) => { })
                .ConfigureServices(s => { s.AddTransient<IConfigureOptions<KestrelServerOptions>, KestrelServerOptionsSetup>(); })
                .ConfigureLogging()
                .ConfigureDownlink()
                .ConfigureAppConfiguration(args)
                .UseStartup<Startup>()
                .Build();
    }
}
