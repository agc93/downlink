using System;
using Downlink.Composition;
using Downlink.Core;
using Downlink.Core.Runtime;
using Downlink.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting
{
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddDownlink(this IMvcBuilder builder)
        {
            builder.AddDownlink(null, DownlinkBuilderOptions.None);
            return builder;
        }

        public static IMvcCoreBuilder AddDownlink(this IMvcCoreBuilder builder) {
            builder.AddDownlink(null, DownlinkBuilderOptions.None);
            return builder;
        }

        public static IMvcBuilder AddDownlink(this IMvcBuilder builder, Action<IDownlinkBuilder> opts)
        {
            builder.AddDownlink(opts, DownlinkBuilderOptions.None);
            return builder;
        }

        public static IMvcCoreBuilder AddDownlink(this IMvcCoreBuilder builder, Action<IDownlinkBuilder> configure) {
            builder.AddDownlink(configure, DownlinkBuilderOptions.None);
            return builder;
        }

        public static IMvcBuilder AddDownlink(this IMvcBuilder builder, DownlinkBuilderOptions opts)
        {
            builder.AddDownlink(null, opts);
            return builder;
        }

        public static IMvcCoreBuilder AddDownlink(this IMvcCoreBuilder builder, DownlinkBuilderOptions opts) {
            builder.AddDownlink(null, opts);
            return builder;
        }

        public static IMvcBuilder AddDownlink(this IMvcBuilder builder, Action<IDownlinkBuilder> configure, DownlinkBuilderOptions opts)
        {
            builder.AddApplicationPart(typeof(DownlinkBuilder).Assembly);
            BuildDownlink(builder.Services, configure, opts);
            return builder;
        }

        public static IMvcCoreBuilder AddDownlink(this IMvcCoreBuilder builder, Action<IDownlinkBuilder> configure, DownlinkBuilderOptions opts) {
            builder.AddApplicationPart(typeof(DownlinkBuilder).Assembly);
            BuildDownlink(builder.Services, configure, opts);
            return builder;
        }

        private static void BuildDownlink(IServiceCollection services, Action<IDownlinkBuilder> configure, DownlinkBuilderOptions opts) {
            services.AddMediatR();
            var builder = new DownlinkBuilder(services);
            builder.Services.AddSingleton<IPluginLoader, PluginLoader>();
            builder.Services.AddSingleton<DownlinkBuilderDefaults>(new DownlinkBuilderDefaults(opts));
            builder.AddPlugin<DownlinkDefaultServices>();
            configure?.Invoke(builder);
            builder.Build();
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
