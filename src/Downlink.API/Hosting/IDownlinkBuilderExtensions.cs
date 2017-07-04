using System;
using Downlink.Core;
using Downlink.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Downlink.Hosting
{
    public static class IDownlinkBuilderExtensions
    {
        public static IDownlinkBuilder AddPatternMatcher<T>(this IDownlinkBuilder builder) where T : class, IPatternMatcher {
            builder.Services.AddTransient<IPatternMatcher, T>();
            return builder;
        }

        public static IDownlinkBuilder AddStorage<T>(this IDownlinkBuilder builder) where T : class, IRemoteStorage {
            builder.Services.AddSingleton<IRemoteStorage, T>();
            return builder;
        }

        public static IDownlinkBuilder AddResponseHandler<T>(this IDownlinkBuilder builder) where T : class, IResponseHandler {
            builder.Services.AddSingleton<IResponseHandler, T>();
            return builder;
        }

        public static IDownlinkBuilder AddSchemeClient<T>(this IDownlinkBuilder builder) where T : class, ISchemeClient {
            builder.Services.AddTransient<ISchemeClient, T>();
            return builder;
        }
     }
}
