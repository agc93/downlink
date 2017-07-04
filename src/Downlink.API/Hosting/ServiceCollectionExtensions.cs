using System;
using Downlink.Core;
using Downlink.Core.Runtime;
using Downlink.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting
{
    public static class MvcBuilderExtensions
    {
        public static IDownlinkBuilder AddDownlink(this IMvcBuilder builder)
        {
            return builder.AddDownlink(null, DownlinkBuilderOptions.None);
        }

        public static IDownlinkBuilder AddDownlink(this IMvcBuilder builder, Action<IDownlinkBuilder> opts)
        {
            return builder.AddDownlink(opts, DownlinkBuilderOptions.None);
        }

        public static IDownlinkBuilder AddDownlink(this IMvcBuilder builder, DownlinkBuilderOptions opts)
        {
            return builder.AddDownlink(null, opts);
        }

        public static IDownlinkBuilder AddDownlink(this IMvcBuilder builder, Action<IDownlinkBuilder> configure, DownlinkBuilderOptions opts)
        {
            builder.AddApplicationPart(typeof(DownlinkBuilder).Assembly);
            var dBuilder = new DownlinkBuilder(builder.Services);
            DownlinkBuilder.AddDefaultServices(dBuilder, opts);
            return dBuilder;
        }

        internal static void AddDefaultPatternMatchers(this IServiceCollection services)
        {
            services.AddTransient<IPatternMatcher, HierarchicalPatternMatcher>();
            services.AddTransient<IPatternMatcher, RuntimePatternMatcher>();
            services.AddTransient<IPatternMatcher, FlatPatternMatcher>();
            services.AddTransient<IPatternMatcher, FlatVersionMatcher>();
            services.AddTransient<IPatternMatcher, FlatPlatformMatcher>();
        }
    }
}
