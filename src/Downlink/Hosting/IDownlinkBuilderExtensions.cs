using System;
using Downlink.Core;
using Downlink.Handlers;
using Downlink.Messaging;
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

        public static IDownlinkBuilder AddResponseAction(this IDownlinkBuilder builder, Action<AppVersionResponseModel> response) {
            builder.Services.AddTransient<MediatR.INotificationHandler<AppVersionResponseModel>>(provider => new Messaging.ActionNotification(response));
            return builder;
        }

        public static IDownlinkBuilder AddNotificationHandler<T>(this IDownlinkBuilder builder) where T : class, MediatR.INotificationHandler<AppVersionResponseModel> {
            builder.Services.AddScoped<MediatR.INotificationHandler<AppVersionResponseModel>, T>();
            return builder;
        }

        public static IDownlinkBuilder AddPlugin<T>(this IDownlinkBuilder builder) where T : class, Composition.IDownlinkPlugin {
            builder.Services.AddSingleton<Composition.IDownlinkPlugin, T>();
            return builder;
        }
     }
}
